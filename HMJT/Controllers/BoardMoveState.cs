namespace HMJT.Models;

// Stores information about a player's current
// movement on the game board.
public class BoardMoveState
{
    // Stores the result of the current dice roll.
    public int DiceRoll { get; set; }

    // Stores the IDs of all board spaces that
    // the player is currently allowed to move to.
    public List<int> LegalDestinationIds { get; set; } = new();

    // Indicates whether the game is waiting for
    // the player to choose a destination on the board.
    public bool IsWaitingForMove { get; set; }
}