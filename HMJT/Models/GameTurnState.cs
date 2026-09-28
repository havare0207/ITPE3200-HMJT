// Places GameTurnState in the HMJT.Models namespace.
namespace HMJT.Models;

// Represents the current turn of a player during a game.
public class GameTurnState
{
    // Stores the player whose turn it is.
    public PlayerGameState Player { get; set; } = new();

    // Stores the result of the dice roll.
    public int DiceRoll { get; set; }

    // Stores the category selected by the dice roll.
    public string Category { get; set; } = string.Empty;

    // Stores the question selected for this turn.
    public Question Question { get; set; } = new();

    // Stores whether this turn is a final question turn.
    public bool IsFinalTurn { get; set; }
    
}