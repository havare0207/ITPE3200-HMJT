// Places GameTurnResult in the HMJT.Models namespace.
namespace HMJT.Models;

// Represents the result of a completed game turn.
public class GameTurnResult
{
    // Stores the answer given during the turn.
    public GameAnswer GameAnswer { get; set; } = new();

    // Stores whether the player received a new wedge.
    public bool WedgeAwarded { get; set; }
}