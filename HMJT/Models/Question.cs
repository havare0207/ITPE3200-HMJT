// Places Question in the HMJT.Models namespace.
namespace HMJT.Models;

// Represents one question used in the game.
public class Question
{
    // Unique identifier for the question.
    public int QuestionId { get; set; }

    // The question text shown to the player.
    public string QuestionText { get; set; } = string.Empty;

    // The category the question belongs to.
    public string Category { get; set; } = string.Empty;

    // The difficulty level of the question.
    public Difficulty Difficulty { get; set; }

    // Answer option A.
    public string AnswerA { get; set; } = string.Empty;

    // Answer option B.
    public string AnswerB { get; set; } = string.Empty;

    // Answer option C.
    public string AnswerC { get; set; } = string.Empty;

    // Answer option D.
    public string AnswerD { get; set; } = string.Empty;

    // Stores which answer option is correct: A, B, C or D.
   public AnswerOption CorrectAnswer { get; set; }
}