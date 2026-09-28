// Places GamePlayViewModel in the HMJT.Models namespace.
namespace HMJT.Models;

// ViewModel used to send game data from GameController to Play.cshtml.
// It combines information from the current player, turn, question,
// wedge progress, and game state into one object for the user interface.
public class GamePlayViewModel
{
    // Shows whether a game session is currently active.
    public bool HasActiveGame { get; set; }

    // Stores the current player's name.
    public string PlayerName { get; set; } = string.Empty;

    // Stores the current player's number.
    public int PlayerNumber { get; set; }

    // Stores the result of the current dice roll.
    public int DiceRoll { get; set; }

    // Stores the category for the current turn.
    public string Category { get; set; } = string.Empty;

    // Stores the current question.
    public Question Question { get; set; } = new();

    // Stores the wedges collected by the current player.
    public HashSet<string> Wedges { get; set; } = new();

    // Stores the wedges the current player is still missing.
    public List<string> MissingWedges { get; set; } = new();

    // Shows whether a question is currently active.
    public bool HasActiveTurn { get; set; }

    // Shows whether the current player can roll the dice.
    public bool CanRollDice { get; set; }

    // Shows whether the other players must choose
    // a category for the final question.
    public bool NeedsFinalCategory { get; set; }

    // Shows whether the current turn is a final turn.
    public bool IsFinalTurn { get; set; }

    // Shows whether the game has ended.
    public bool IsGameOver { get; set; }

    // Stores the winner's name after the game ends.
    public string WinnerName { get; set; } = string.Empty;

    // Stores temporary feedback for the players.
    public string Message { get; set; } = string.Empty;
}