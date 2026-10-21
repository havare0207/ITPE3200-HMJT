namespace HMJT.Models;

public class PresetQuestion
{
    public int PresetQuestionId { get; set; }

    public string Category { get; set; } = string.Empty;

    public string QuestionText { get; set; } = string.Empty;

    public Difficulty Difficulty { get; set; }
    public string AnswerA { get; set; } = string.Empty;
    public string AnswerB { get; set; } = string.Empty;
    public string AnswerC { get; set; } = string.Empty;
    public string AnswerD { get; set; } = string.Empty;
    public AnswerOption CorrectAnswer { get; set; }

    // Setting up a many-to-many relationship with Game
    public ICollection<Game> Games { get; set; } = new List<Game>();
}