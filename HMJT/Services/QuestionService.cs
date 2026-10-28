// Gives this service access to the Question
// and Difficulty models.
using HMJT.Models;

// Used to read and deserialize
// the preset question JSON file.
using System.Text.Json;

// Places QuestionService in the
// HMJT.Services namespace.
namespace HMJT.Services;

// Handles the selection of preset questions
// used by Standard Game.
public class QuestionService
{
    // Stores all preset questions loaded
    // from preset-questions.json.
    private readonly List<Question> _questions;


    // The constructor runs whenever a new
    // QuestionService object is created.
    public QuestionService()
    {
        _questions =
            LoadPresetQuestions();
    }


    // Loads the Standard Game questions
    // from the JSON file.
    private static List<Question> LoadPresetQuestions()
    {
        // The JSON file is copied to the application's
        // output directory when the project is built.
        string filePath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                "preset-questions.json"
            );

        // Stop immediately if the preset question
        // file cannot be found.
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "The preset question file could not be found.",
                filePath
            );
        }

        // Read the entire JSON file.
        string json =
            File.ReadAllText(filePath);

        // Deserialize the JSON into temporary objects.
        List<PresetQuestionJson>? jsonQuestions =
            JsonSerializer.Deserialize<List<PresetQuestionJson>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

        if (jsonQuestions == null ||
            jsonQuestions.Count == 0)
        {
            throw new InvalidOperationException(
                "No preset questions were found in preset-questions.json."
            );
        }

        var questions =
            new List<Question>();

        // Convert each JSON object into
        // the Question model used by gameplay.
        for (int index = 0;
             index < jsonQuestions.Count;
             index++)
        {
            PresetQuestionJson jsonQuestion =
                jsonQuestions[index];

            // Convert the difficulty text
            // such as "Easy" into the enum.
            if (!Enum.TryParse(
                    jsonQuestion.Difficulty,
                    true,
                    out Difficulty difficulty))
            {
                throw new InvalidOperationException(
                    $"Invalid difficulty '{jsonQuestion.Difficulty}' " +
                    $"in preset-questions.json."
                );
            }

            // Convert A, B, C or D
            // into the AnswerOption enum.
            if (!Enum.TryParse(
                    jsonQuestion.CorrectAnswer,
                    true,
                    out AnswerOption correctAnswer))
            {
                throw new InvalidOperationException(
                    $"Invalid correct answer " +
                    $"'{jsonQuestion.CorrectAnswer}' " +
                    $"in preset-questions.json."
                );
            }

            questions.Add(
                new Question
                {
                    // Preset questions are not stored
                    // as database questions, but a unique
                    // ID is still useful during gameplay.
                    QuestionId = index + 1,

                    Category =
                        jsonQuestion.Category,

                    QuestionText =
                        jsonQuestion.QuestionText,

                    Difficulty =
                        difficulty,

                    AnswerA =
                        jsonQuestion.AnswerA,

                    AnswerB =
                        jsonQuestion.AnswerB,

                    AnswerC =
                        jsonQuestion.AnswerC,

                    AnswerD =
                        jsonQuestion.AnswerD,

                    CorrectAnswer =
                        correctAnswer
                }
            );
        }

        return questions;
    }


    // Returns a random preset question that matches
    // the selected category and difficulty.
    public Question GetRandomQuestion(
        string category,
        Difficulty difficulty)
    {
        // Find every question matching both
        // the category and difficulty.
        List<Question> matchingQuestions =
            _questions
                .Where(question =>
                    question.Category == category &&
                    question.Difficulty == difficulty)
                .ToList();

        // Standard Game cannot continue if
        // this combination has no questions.
        if (matchingQuestions.Count == 0)
        {
            throw new InvalidOperationException(
                "No questions were found for the selected " +
                "category and difficulty."
            );
        }

        // Randomly choose one of the matching questions.
        int randomIndex =
            Random.Shared.Next(
                matchingQuestions.Count
            );

        return matchingQuestions[randomIndex];
    }


    // Represents the structure used inside
    // preset-questions.json.
    private sealed class PresetQuestionJson
    {
        public string Category { get; set; } =
            string.Empty;

        public string QuestionText { get; set; } =
            string.Empty;

        public string Difficulty { get; set; } =
            string.Empty;

        public string AnswerA { get; set; } =
            string.Empty;

        public string AnswerB { get; set; } =
            string.Empty;

        public string AnswerC { get; set; } =
            string.Empty;

        public string AnswerD { get; set; } =
            string.Empty;

        public string CorrectAnswer { get; set; } =
            string.Empty;
    }
}