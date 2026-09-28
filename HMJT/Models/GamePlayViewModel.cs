// ViewModel used to send game data from GameController to Play.cshtml.
// It combines information from the current player, turn, question,
// wedge progress, and game state into one object for the user interface.


// Places GamePlayViewModel in the HMJT.Models namespace.
namespace HMJT.Models;

// Contains the data needed by the Play view.
public class GamePlayViewModel
{
    // Stores the name of the player whose turn it currently is.
    public string PlayerName { get; set; } = string.Empty;

    // Stores the player's fixed number in the turn order.
    public int PlayerNumber { get; set; }

    // Stores the dice result for a normal turn.
    public int DiceRoll { get; set; }

    // Stores the category selected for the current turn.
    public string Category { get; set; } = string.Empty;

    // Stores the question shown to the player.
    public Question Question { get; set; } = new();

    // Stores the wedges already collected by the player.
    public HashSet<string> Wedges { get; set; } = new();

    // Stores the wedges the player still needs.
    public List<string> MissingWedges { get; set; } = new();

    // Stores whether the current turn is a final question turn.
    public bool IsFinalTurn { get; set; }

    // Stores whether the game has ended.
    public bool IsGameOver { get; set; }

    // Stores the name of the winner if the game has ended.
    public string WinnerName { get; set; } = string.Empty;
}