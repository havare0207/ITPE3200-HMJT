// Gives this service access to the game models.
using HMJT.Models;

// Places GameStateService in the HMJT.Services namespace.
namespace HMJT.Services;

// Stores the current game state while using the mock game flow.
// This allows the game to remember players, difficulty and the
// current turn between different requests from the Play page.
//
// This is temporary in-memory storage and can later be replaced
// by database or session-based storage.
public class GameStateService
{
    // Stores the active game session.
    public GameSessionState? GameSession { get; set; }

    // Stores the currently active turn.
    public GameTurnState? CurrentTurn { get; set; }

    // Stores the difficulty selected when the game starts.
    public Difficulty Difficulty { get; set; } = Difficulty.Easy;

    // Shows whether a game has been started.
    public bool HasActiveGame =>
        GameSession != null;

    // Stores temporary feedback that can be shown
    // to the players on the Play page.
    public string Message { get; set; } = string.Empty;


    // Stores the current board movement while
    // the active player is choosing a destination.
    public BoardMoveState? CurrentMove { get; set; }
    
    // Shows whether the current player has just moved
    // into the center and must choose a category.
    public bool IsCenterChoicePending { get; set; }
}