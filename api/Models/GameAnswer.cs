// Places GameAnswer in the HMJT.Models namespace.
namespace HMJT.Models;

// Represents one answer given by a player during a game.
public class GameAnswer
{
    // Unique identifier for this game answer.
    public int GameAnswerId { get; set; }

    // Identifies which question the player answered.
    // This can be a foreign key when the database has been made.
    public int QuestionId { get; set; }

    // Stores the name of the player who gave the answer.
    // This can later be replaced or connected to a PlayerId
    // when the database models are implemented.
    public string PlayerName { get; set; } = string.Empty;

    // Stores which answer option the player selected: A, B, C or D.
    public AnswerOption SelectedAnswer { get; set; }

    // Stores whether the selected answer was correct.
    public bool IsCorrect { get; set; }
}