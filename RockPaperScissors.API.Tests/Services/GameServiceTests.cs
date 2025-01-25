using FluentAssertions;
using RockPaperScissors.Services;
using RockPaperScissors.Common;

namespace RockPaperScissors.API.Tests.Services;

[TestFixture]
public class GameServiceTests
{
    private GameService _service;

    [SetUp]
    public void SetUp()
    {
        _service = new GameService();
    }

    // CREATE GAME - Happy Path and Failure Case
    [Test]
    public void CreateGame_ValidPlayerName_ShouldReturnGame()
    {
        // Arrange
        var playerName = "Player1";

        // Act
        var game = _service.CreateGame(playerName);

        // Assert
        game.Should().NotBeNull();
        game.Id.Should().NotBeNullOrEmpty();
        game.Player1.Should().Be(playerName);
        game.Player2.Should().BeNull();
        game.Player1Move.Should().BeNull();
        game.Player2Move.Should().BeNull();
        game.Winner.Should().BeNull();
    }
    
    [Test]
    public void CreateGame_InvalidPlayerName_ShouldThrowValidationException()
    {
        // Arrange
        string? invalidPlayerName = null;

        // Act 
        Action act = () => _service.CreateGame(invalidPlayerName!);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Player 1 name cannot be null or empty.");
    }
    
    // JOIN GAME - Happy Path, Failure Case and If Player already Exists. 
    [Test]
    public void JoinGame_ValidInputs_ShouldAddPlayer2()
    {
        // Arrange
        var game = _service.CreateGame("Player1");

        // Act
        var result = _service.JoinGame(game.Id, "Player2");

        // Assert
        result.Should().BeTrue();
        game.Player2.Should().Be("Player2");
    }
    
    [Test]
    public void JoinGame_InvalidGameId_ShouldThrowNotFoundException()
    {
        // Arrange
        var invalidGameId = "invalid-id";
        var player2Name = "Player2";

        // Act
        Action act = () => _service.JoinGame(invalidGameId, player2Name);

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage($"Game with ID '{invalidGameId}' was not found.");
    }
    
    [Test]
    public void JoinGame_Player2AlreadyExists_ShouldThrowValidationException()
    {
        // Arrange
        var game = _service.CreateGame("Player1");
        _service.JoinGame(game.Id, "Player2");

        // Act
        Action act = () => _service.JoinGame(game.Id, "AnotherPlayerName");

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Player 2 has already joined the game.");
    }
    
    // MAKE MOVE - Happy Path, Failure Case
    [Test]
    public void MakeMove_ValidMoves_ShouldDetermineWinner()
    {
        // Arrange
        var game = _service.CreateGame("Player1");
        _service.JoinGame(game.Id, "Player2");

        // Act
        _service.MakeMove(game.Id, "Player1", "rock");
        _service.MakeMove(game.Id, "Player2", "scissors");

        // Assert
        game.Player1Move.Should().Be("rock");
        game.Player2Move.Should().Be("scissors");
        game.Winner.Should().Be("Player1");
    }
    
    [Test]
    public void MakeMove_InvalidMove_ShouldThrowValidationException()
    {
        // Arrange
        var game = _service.CreateGame("Player1");
        _service.JoinGame(game.Id, "Player2");

        // Act
        var act = () => _service.MakeMove(game.Id, "Player1", "applepie");

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Invalid move. Valid moves are: Rock, Paper, or Scissors.");
    }
    
    // GET GAME - Happy Path initial state, HappyPath fully populated and Failure Case
    [Test]
    public void GetGame_ValidGameId_InitialState_ShouldReturnGame()
    {
        // Arrange
        var playerName = "Player1";
        var game = _service.CreateGame(playerName);

        // Act
        var result = _service.GetGame(game.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(game.Id);
        result.Player1.Should().Be(playerName);
        result.Player2.Should().BeNull();
        result.Player1Move.Should().BeNull();
        result.Player2Move.Should().BeNull();
        result.Winner.Should().BeNull();
    }
    
    [Test]
    public void GetGame_ValidGameId_FullyPopulated_ShouldReturnGame()
    {
        // Arrange
        var player1Name = "Player1";
        var player2Name = "Player2";
        var game = _service.CreateGame(player1Name);
        _service.JoinGame(game.Id, player2Name);
        _service.MakeMove(game.Id, player1Name, "Rock");
        _service.MakeMove(game.Id, player2Name, "Scissors");

        // Act
        var result = _service.GetGame(game.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(game.Id);
        result.Player1.Should().Be(player1Name);
        result.Player2.Should().Be(player2Name);
        result.Player1Move.Should().Be("Rock");
        result.Player2Move.Should().Be("Scissors");
        result.Winner.Should().Be("Player1"); 
    }
    
    [Test]
    public void GetGame_InvalidGameId_ShouldThrowNotFoundException()
    {
        // Act
        var act = () => _service.GetGame("invalid-id");

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage("Game with ID 'invalid-id' was not found.");
    }
    
    //Test for Determine Player one as winner
    [TestCase("rock", "scissors", "Player1Name", "Player2Name", "Player1Name")]
    [TestCase("scissors", "paper", "Alice", "Anna", "Alice")]
    [TestCase("paper", "rock", "Axel", "Bob", "Axel")]
    public void DetermineWinner_Player1Wins_ShouldReturnPlayer1(string player1Move, string player2Move, string player1Name, 
        string player2Name, string expectedWinner)
    {
        // Act
        var result = _service.DetermineWinner(player1Move, player2Move,player1Name, player2Name);

        // Assert
        result.Should().Be(expectedWinner);
    }
    
    //Test for Determine Player Two as winner
    [TestCase("scissors", "rock", "Player1Name", "Player2Name", "Player2Name")]
    [TestCase("paper", "scissors", "Alice", "Anna", "Anna")]
    [TestCase("rock", "paper", "Axel", "Bob", "Bob")]
    public void DetermineWinner_Player2Wins_ShouldReturnPlayer2(string player1Move, string player2Move, string player1Name, 
        string player2Name, string expectedWinner)
    {
        // Act
        var result = _service.DetermineWinner(player1Move, player2Move, player1Name, player2Name);

        // Assert
        result.Should().Be(expectedWinner);
    }
    
    // Test for Determine Winner - Tie
    [TestCase("rock", "rock", "Player1Name", "Player2Name", "Tie")]
    [TestCase("paper", "paper", "Alice", "Anna", "Tie")]
    [TestCase("scissors", "scissors", "Axel", "Bob", "Tie")]
    public void DetermineWinner_Tie_ShouldReturnTie(string player1Move, string player2Move,  string player1Name, 
        string player2Name, string expectedResult)
    {
        // Act
        var result = _service.DetermineWinner(player1Move, player2Move, player1Name, player2Name);

        // Assert
        result.Should().Be(expectedResult);
    }
}