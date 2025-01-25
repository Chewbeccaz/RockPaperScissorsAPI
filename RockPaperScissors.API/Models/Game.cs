namespace RockPaperScissors.Models;

public class Game
{
    public string Id { get; set; } = string.Empty;
    public string Player1 { get; set; } = string.Empty;
    public string? Player2 { get; set; } = string.Empty;
    public string? Player1Move { get; set; } = string.Empty;
    public string? Player2Move { get; set; } = string.Empty;
    public string? Winner { get; set; } = string.Empty; 
    public bool IsComplete => !string.IsNullOrEmpty(Winner); 
}