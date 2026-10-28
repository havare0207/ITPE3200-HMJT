using System;
using System.ComponentModel.DataAnnotations;
using HMJT.Models;

namespace HMJT.ViewModels;

// Represents the data displayed or submitted
// by the individual Game views.
public class GameViewModel
{
    // Identifies the game being edited, displayed, or deleted.
    public int GameId { get; set; }

    // Validates the game name when a user creates or edits a game.
    [Required(ErrorMessage = "Please enter a game name.")]
    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "Game name must be between 2 and 50 characters.")]
    public string Name { get; set; } = string.Empty;

    // Displays the current game status.
    // The controller does not accept this as a user-controlled update.
    public GameStatus Status { get; set; } = GameStatus.NotStarted;

    // Displays when the game was created.
    // The controller sets this value when creating a game.
    public DateTime CreatedAt { get; set; }

    // Displays how many times the game has been played.
    // The controller does not update this from submitted form data.
    public int TimesPlayed { get; set; }
}