using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using RockPaperScissors.Controllers;
using RockPaperScissors.Models;
using RockPaperScissors.Services;
using RockPaperScissors.Common;

namespace RockPaperScissors.API.Tests.Controllers;

[TestFixture]
public class GameControllerTests
{
    private IGameService _mockService;

    [OneTimeSetUp]
    public void Setup()
    {
        _mockService = A.Fake<IGameService>();
    }

    private GameController GetSut()
    {
        return new GameController(_mockService);
    }
    
    [Test]
    public void CreateGame_ValidRequest_ShouldReturnOkWithGameId()
    {
        // Arrange
        var playerName = "Player1";
        var game = new Game { Id = "test-id", Player1 = playerName };
        A.CallTo(() => _mockService.CreateGame(playerName)).Returns(game);
        var sut = GetSut();

        // Act
        var result = sut.CreateGame(new PlayerRequest { Name = playerName }) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(new
        {
            Id = "test-id",
            Instructions = "Share this game ID with Player 2 to start the game.",
            Message = "Game created successfully."
        });
    }

    [Test]
    public void GetGame_ValidId_ShouldReturnOkWithGameDetails()
    {
        // Arrange
        var gameId = "test-id";
        var game = new Game { Id = gameId, Player1 = "Player1", Player2 = "Player2" };
        A.CallTo(() => _mockService.GetGame(gameId)).Returns(game);
        var sut = GetSut();

        // Act
        var result = sut.GetGame(gameId) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(game);
    }

    [Test]
    public void GetGame_InvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var gameId = "invalid-id";
        A.CallTo(() => _mockService.GetGame(gameId)).Throws(new NotFoundException($"Game with ID '{gameId}' was not found."));
        var sut = GetSut();

        // Act
        Action act = () => sut.GetGame(gameId);

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage($"Game with ID '{gameId}' was not found.");
    }

    [Test]
    public void JoinGame_ValidRequest_ShouldReturnOk()
    {
        // Arrange
        var gameId = "test-id";
        var player2Name = "Player2";
        A.CallTo(() => _mockService.JoinGame(gameId, player2Name)).Returns(true);
        var sut = GetSut();

        // Act
        var result = sut.JoinGame(gameId, new PlayerRequest { Name = player2Name }) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(new { Message = "Player 2 joined successfully." });
    }

    [Test]
    public void JoinGame_InvalidGameId_ShouldThrowNotFoundException()
    {
        // Arrange
        var gameId = "invalid-id";
        var player2Name = "Player2";
        A.CallTo(() => _mockService.JoinGame(gameId, player2Name))
            .Throws(new NotFoundException($"Game with ID '{gameId}' was not found."));
        var sut = GetSut();

        // Act
        Action act = () => sut.JoinGame(gameId, new PlayerRequest { Name = player2Name });

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage($"Game with ID '{gameId}' was not found.");
    }

    [Test]
    public void MakeMove_InvalidMove_ShouldThrowValidationException()
    {
        // Arrange
        var gameId = "test-id";
        var playerName = "Player1";
        var invalidMove = "applepie";
        A.CallTo(() => _mockService.MakeMove(gameId, playerName, invalidMove))
            .Throws(new ValidationException("Invalid move. Valid moves are: Rock, Paper, or Scissors."));
        var sut = GetSut();

        // Act
        Action act = () => sut.MakeMove(gameId, new MoveRequest { Name = playerName, Move = invalidMove });

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Invalid move. Valid moves are: Rock, Paper, or Scissors.");
    }
}