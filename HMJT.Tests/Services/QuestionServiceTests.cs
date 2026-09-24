// Gives this test file access to QuestionService.
using HMJT.Services;

// Gives this test file access to Difficulty and Question.
using HMJT.Models;

// Organizes tests for service classes.
namespace HMJT.Tests.Services;

public class QuestionServiceTests
{
    // Theory is used because the same test should run
    // several times with different categories and difficulties.
    [Theory]
    [InlineData("Java", Difficulty.Easy)]
    [InlineData("JavaScript", Difficulty.Medium)]
    [InlineData("HTML/CSS", Difficulty.Hard)]
    [InlineData("Python", Difficulty.Easy)]
    [InlineData("Game History", Difficulty.Medium)]
    [InlineData("C#", Difficulty.Hard)]

    //test to see if both category and difficulity are correct

    public void GetRandomQuestion_ShouldMatchCategoryAndDifficulty(
        string category,
        Difficulty difficulty)

    {
        // Create QuestionService so its question selection can be tested.
        var questionService = new QuestionService();

        // Ask the service for a question matching
        // the category and difficulty from InlineData.
        Question result =
            questionService.GetRandomQuestion(category, difficulty);
        
        // Check that the returned question belongs
        // to the requested category.
        Assert.Equal(category, result.Category);

        // Check that the returned question has
        // the requested difficulty.
        Assert.Equal(difficulty, result.Difficulty);

    }

    //Exeptiontest
    [Fact]
    public void GetRandomQuestion_ShouldThrowExceptionWhenNoQuestionsMatch()
    {
        // Create QuestionService so the method can be tested.
        var questionService = new QuestionService();

        // Check that requesting a category that does not exist
        // causes an InvalidOperationException.
        Assert.Throws<InvalidOperationException>(() =>
            questionService.GetRandomQuestion(
                "NonExistingCategory",
                Difficulty.Easy
            ));

    }

}