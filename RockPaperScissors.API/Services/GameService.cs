using System.Collections.Concurrent;
using RockPaperScissors.Common;
using RockPaperScissors.Models;

namespace RockPaperScissors.Services;

public class GameService : IGameService
{
    private readonly ConcurrentDictionary<string, Game> _games = new();

    public Game CreateGame(string player1)
    {
        if (string.IsNullOrWhiteSpace(player1))
        {
            throw new ValidationException("Player 1 name cannot be null or empty.");
        }
        
        var gameId = Guid.NewGuid().ToString();
        var game = new Game
        {
            Id = gameId,
            Player1 = player1,
            Player2 = null, 
            Player1Move = null,
            Player2Move = null,
            Winner = null
        };

        _games[gameId] = game; 
        return game;
    }

    public bool JoinGame(string id, string player2)
    {
        if (!_games.TryGetValue(id, out var game))
        {
            throw new NotFoundException($"Game with ID '{id}' was not found.");
        }
        
        if (string.IsNullOrEmpty(player2))
        {
            throw new ValidationException("Player 2 name is required.");
        }
        
        if (!string.IsNullOrEmpty(game.Player2))
        {
            throw new ValidationException("Player 2 has already joined the game.");
        }
        game.Player2 = player2;
        return true;
    }
    
    public void MakeMove(string id, string playerName, string move)
    {
        var game = GetGame(id);
        
        var validMoves = new[] { "rock", "paper", "scissors" };
        if (!validMoves.Contains(move.ToLower()))
        {
            throw new ValidationException("Invalid move. Valid moves are: Rock, Paper, or Scissors.");
        }
        
        if (game.Player1 == playerName && string.IsNullOrEmpty(game.Player1Move))
        {
            game.Player1Move = move;
        }
        else if (game.Player2 == playerName && string.IsNullOrEmpty(game.Player2Move))
        {
            game.Player2Move = move;
        }
        else
        {
            throw new ValidationException(
                game.Player1 == playerName || game.Player2 == playerName
                    ? $"{playerName} has already made a move."
                    : "Player is not part of this game."
            );
        }

        if (!string.IsNullOrEmpty(game.Player1Move) && 
            !string.IsNullOrEmpty(game.Player2Move) && 
            !string.IsNullOrEmpty(game.Player2))
        {
            game.Winner = DetermineWinner(game.Player1Move, game.Player2Move, game.Player1, game.Player2);
        }
    }
    
    public Game GetGame(string id)
    {
        if (!_games.TryGetValue(id, out var game))
        {
            throw new NotFoundException($"Game with ID '{id}' was not found.");
        }

        return game;
    }
    
    //Method for checking the winner
    public string DetermineWinner(string player1Move, string player2Move, string player1Name, string player2Name)
    {
        player1Move = player1Move.ToLower();
        player2Move = player2Move.ToLower();

        if (player1Move == player2Move)
        {
            return "Tie";
        }

        return (player1Move, player2Move) switch
        {
            ("rock", "scissors") => player1Name,
            ("scissors", "paper") => player1Name,
            ("paper", "rock") => player1Name,
            ("scissors", "rock") => player2Name,
            ("paper", "scissors") => player2Name,
            ("rock", "paper") => player2Name,
            _ => throw new ValidationException($"Unexpected moves: {player1Move}, {player2Move}") 
        };
    }
}
