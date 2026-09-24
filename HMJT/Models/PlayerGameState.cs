// Places PlayerGameState in the HMJT.Models namespace.
namespace HMJT.Models;

// Represents a player's current progress during a game.
public class PlayerGameState
{
    // Stores the player's name.
    public string PlayerName { get; set; } = string.Empty;

    // Stores the category wedges the player has collected.
    // HashSet prevents the same category from being added more than once.
    public HashSet<string> Wedges { get; set; } = new();
}