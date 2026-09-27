using System.ComponentModel.DataAnnotations;

namespace HMJT.Models;

public class Game
{
    public int GameId { get; set; }

    [Required(ErrorMessage = "Please enter a game name.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Game name must be between 2 and 50 characters.")]
    public string Name { get; set; } = string.Empty;
    public GameStatus Status { get; set; } = GameStatus.NotStarted;
    public int CurrentPosition { get; set; } = 0;
}