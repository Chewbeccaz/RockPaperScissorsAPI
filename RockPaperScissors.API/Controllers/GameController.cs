using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Common;
using RockPaperScissors.Models;
using RockPaperScissors.Services;

namespace RockPaperScissors.Controllers;
[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    public IActionResult CreateGame([FromBody] PlayerRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            throw new ValidationException("Player name cannot be null or empty.");
        }
        var game = _gameService.CreateGame(request.Name);

        return Ok(new
        {
            Id = game.Id, 
            Instructions = "Share this game ID with Player 2 to start the game.",
            Message = "Game created successfully."
        });
    }
    
    [HttpPost("{id}/join")]
    public IActionResult JoinGame(string id, [FromBody] PlayerRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            throw new ValidationException("Player 2 name is required.");
        }
        _gameService.JoinGame(id, request.Name);
        return Ok(new { Message = "Player 2 joined successfully." });
    }
    
    [HttpPost("{id}/move")]
    public IActionResult MakeMove(string id, [FromBody] MoveRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            throw new ValidationException("Player name cannot be null or empty.");
        }
        if (string.IsNullOrEmpty(request.Move))
        {
            throw new ValidationException("Move cannot be null or empty.");
        }
        _gameService.MakeMove(id, request.Name, request.Move);
        return Ok(new { Message = "Move registered successfully." });
    }
    
    [HttpGet("{id}")]
    public IActionResult GetGame(string id)
    {
        var game = _gameService.GetGame(id);
        return Ok(game);
    }
}
