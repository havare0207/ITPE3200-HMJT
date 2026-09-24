// Gives this service access to the Question and Difficulty models.
using HMJT.Models;

// Gives this service access to the temporary mock question data.
using HMJT.MockData;

// Places QuestionService in the HMJT.Services namespace.
namespace HMJT.Services;

// Handles the selection of questions for the game.
public class QuestionService
{
    // Stores all available Question objects used by QuestionService.
    // The list is private, so it can only be accessed inside this class.
    // readonly prevents the _questions variable from being assigned
    // to/replaced by a different list after it has been initialized.
    private readonly List<Question> _questions;

    // The constructor runs when a new QuestionService object is created.
    public QuestionService()

    {
        // Loads the temporary mock questions.
        _questions = MockQuestionData.GetQuestions();
    }

    // Returns a random question that matches
    // the selected category and difficulty.
    public Question GetRandomQuestion(string category, Difficulty difficulty)
    {
        // Find all questions that match both
        // the category and the difficulty.
        // Lambda expression:question =>
        // "question" represents each Question object in the list.
        // The expression after => decides whether that question
        // should be kept in the filtered result.
        List<Question> matchingQuestions = _questions
            .Where(question =>
                question.Category == category &&
                question.Difficulty == difficulty)
            .ToList();

        // If no matching questions exist, the game cannot continue
        // with this category and difficulty combination.

        if (matchingQuestions.Count == 0)
        {
            throw new InvalidOperationException(
                "No questions were found for the selected category and difficulty."
            );
        }

        // Generate a random position inside the list
        // of matching questions.
        int randomIndex = Random.Shared.Next(matchingQuestions.Count);

        // Return the question stored at the randomly selected position.
        return matchingQuestions[randomIndex];

    }

}