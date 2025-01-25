using RockPaperScissors.Models;

namespace RockPaperScissors.Services;

public interface IGameService
{
    Game CreateGame(string player1);
    Game GetGame(string id);
    bool JoinGame(string id, string player2);
    void MakeMove(string id, string playerName, string move);
}