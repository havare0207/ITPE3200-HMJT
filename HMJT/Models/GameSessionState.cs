// Places GameSessionState in the HMJT.Models namespace.
namespace HMJT.Models;

// Represents the current state of an active game session.
public class GameSessionState
{
    // Stores all players taking part in the current game.
    public List<PlayerGameState> Players { get; set; } = new();

    // Stores the index of the player whose turn it currently is.
    // This is only used for turn order, not board position.
    public int CurrentPlayerIndex { get; set; }
}