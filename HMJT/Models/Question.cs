// Places Question in the HMJT.Models namespace.
using System.ComponentModel.DataAnnotations;

namespace HMJT.Models;

// Represents one question used in the game.
public class Question
{
    // Unique identifier for the question.
    public int QuestionId { get; set; }

    // The question text shown to the player.
    [Required(ErrorMessage = "Question text is required.")] //Question text validation
    [StringLength(500, ErrorMessage = "Question text cannot exceed 500 characters.")]
    public string QuestionText { get; set; } = string.Empty;

    // The category the question belongs to.
    [Required(ErrorMessage = "Category is required.")]
    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
    public string Category { get; set; } = string.Empty;

    // The difficulty level of the question.

    public Difficulty Difficulty { get; set; }

    // Answer option A.
    [Required(ErrorMessage = "Answer A is required.")]
    [StringLength(300, ErrorMessage = "Answer A cannot exceed 300 characters.")]
    public string AnswerA { get; set; } = string.Empty;

    // Answer option B.
    [Required(ErrorMessage = "Answer B is required.")]
    [StringLength(300, ErrorMessage = "Answer B cannot exceed 300 characters.")]
    public string AnswerB { get; set; } = string.Empty;

    // Answer option C.
    [Required(ErrorMessage = "Answer C is required.")]
    [StringLength(300, ErrorMessage = "Answer C cannot exceed 300 characters.")]
    public string AnswerC { get; set; } = string.Empty;

    // Answer option D.
    [Required(ErrorMessage = "Answer D is required.")]
    [StringLength(300, ErrorMessage = "Answer D cannot exceed 300 characters.")]
    public string AnswerD { get; set; } = string.Empty;

    // Stores which answer option is correct: A, B, C or D.
   public AnswerOption CorrectAnswer { get; set; }
}