// Models/Game.cs

using System.ComponentModel.DataAnnotations;

namespace HMJT.Models;

public class Game
{
    public int GameId { get; set; }

    [Required(ErrorMessage = "Please enter a game name.")]
    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "Game name must be between 2 and 50 characters.")]
    public string Name { get; set; } = string.Empty;

    public GameStatus Status { get; set; } =
        GameStatus.NotStarted;

    public DateTime CreatedAt { get; set; } =
        DateTime.UtcNow;

    // Stores the Identity user ID of the user
    // who created and owns this custom game.
    public string OwnerUserId { get; set; } =
        string.Empty;

    public ICollection<Question> Questions { get; set; } =
        new List<Question>();

    public ICollection<PresetQuestion> PresetQuestions { get; set; } =
        new List<PresetQuestion>();

    public int TimesPlayed { get; set; } = 0;
}