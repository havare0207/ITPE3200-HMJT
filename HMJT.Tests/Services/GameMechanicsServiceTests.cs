// Gives this test file access to GameMechanicsService
// from the main HMJT.Services namespace.

using HMJT.Services;

using HMJT.Models;

// Organizes tests for service classes.
// The full class name is:
// HMJT.Tests.Services.GameMechanicsServiceTests

namespace HMJT.Tests.Services;

public class GameMechanicsServiceTests
{
    // Theory is used when the same test should run
    // multiple times with different input values.

    [Theory]
    
    // Each InlineData contains:
    // dice roll -> expected category
    [InlineData(1, "Java")]
    [InlineData(2, "JavaScript")]
    [InlineData(3, "HTML/CSS")]
    [InlineData(4, "Python")]
    [InlineData(5, "Game History")]
    [InlineData(6, "C#")]

    public void GetCategoryFromDiceRoll_ShouldReturnCorrectCategory(
        int diceRoll,
        string expectedCategory)
    {
        // Create an instance of GameMechanicsService
        // so the method can be tested.
        var gameMechanicsService = new GameMechanicsService();

        // Call the method using the dice roll provided
        // by the current InlineData.

        string result =
            gameMechanicsService.GetCategoryFromDiceRoll(diceRoll);

        // Check that the returned category is the category
        // we expected for this dice roll.
        Assert.Equal(expectedCategory, result);
    }
    
    //test for numbers out of range:
    [Theory]
    
    [InlineData(0)]
    [InlineData(7)] 

    public void GetCategoryFromDiceRoll_ShouldThrowExceptionForInvalidRoll(
    int diceRoll)
    {   
        var gameMechanicsService = new GameMechanicsService();

        // Check that an invalid dice roll causes
        // an ArgumentOutOfRangeException.

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            gameMechanicsService.GetCategoryFromDiceRoll(diceRoll));

    }

    // Test to see if the answer is correct, then true. 
    [Fact]
    public void IsAnswerCorrect_ShouldReturnTrueForCorrectAnswer()
    {
        // Create a question where the correct answer is C.
        var question = new Question
        {
            QuestionId = 1,
            CorrectAnswer = AnswerOption.C
        };

        var gameMechanicsService = new GameMechanicsService();

        // Select the same answer as the question's correct answer.
        bool result = gameMechanicsService.IsAnswerCorrect(
            question,
            AnswerOption.C
        );

        // The answer should be marked as correct.
        Assert.True(result);
    }

    // And a test for wrong answer, should return false
    [Fact]
    public void IsAnswerCorrect_ShouldReturnFalseForWrongAnswer()
    {
        // Create a question where the correct answer is C.
        var question = new Question
        {
            QuestionId = 1,
            CorrectAnswer = AnswerOption.C
        };

        var gameMechanicsService = new GameMechanicsService();

        // Select a different answer than the correct answer.
        bool result = gameMechanicsService.IsAnswerCorrect(
            question,
            AnswerOption.A
        );

        // The answer should be marked as incorrect.
        Assert.False(result);
    }

    // Test for
    // QuestionId     
    // PlayerName     
    // SelectedAnswer 
    // IsCorrect       
    [Fact]
    public void CreateGameAnswer_ShouldStoreCorrectAnswerData()
    {
        // Create a question where the correct answer is B.
        var question = new Question
        {
            QuestionId = 20,
            CorrectAnswer = AnswerOption.B
        };

        var gameMechanicsService = new GameMechanicsService();

        // Create a GameAnswer where the player selects
        // the correct answer.
        GameAnswer result = gameMechanicsService.CreateGameAnswer(
            question,
            "Player 1",
            AnswerOption.B
        );

        // Check that the GameAnswer contains the correct question ID.
        Assert.Equal(20, result.QuestionId);

        // Check that the player's name was stored correctly.
        Assert.Equal("Player 1", result.PlayerName);

        // Check that the selected answer was stored correctly.
        Assert.Equal(AnswerOption.B, result.SelectedAnswer);

        // The selected answer matches the correct answer.
        Assert.True(result.IsCorrect);
    }

    // Test for ShouldMarkWrongAnswerAsIncorrect
    [Fact]
    public void CreateGameAnswer_ShouldMarkWrongAnswerAsIncorrect()
    {
        // Create a question where the correct answer is C.
        var question = new Question
        {
            QuestionId = 21,
            CorrectAnswer = AnswerOption.C
        };

        var gameMechanicsService = new GameMechanicsService();

        // The player selects A even though C is correct.
        GameAnswer result = gameMechanicsService.CreateGameAnswer(
            question,
            "Player 2",
            AnswerOption.A
        );

        // The answer should be marked as incorrect.
        Assert.False(result.IsCorrect);
    }

    // Test for wedge, only get for correctanswer:

    [Fact]
    public void AwardWedge_ShouldAddWedgeForCorrectAnswer()
    {
        // Create a player with no wedges.
        var player = new PlayerGameState
        {
            PlayerName = "Player 1"
        };

        // Create a question in the Python category.
        var question = new Question
        {
            QuestionId = 30,
            Category = "Python"
        };

        // Create a correct game answer.
        var gameAnswer = new GameAnswer
        {
            QuestionId = 30,
            IsCorrect = true
        };

        var gameMechanicsService = new GameMechanicsService();

        // Try to award the Python wedge.
        bool result = gameMechanicsService.AwardWedge(
            player,
            question,
            gameAnswer
        );

        // A new wedge should have been awarded.
        Assert.True(result);

        // The player's wedges should now contain Python.
        Assert.Contains("Python", player.Wedges);
    }

    // Test for wedge, should not get for wrong answer:

    [Fact]
    public void AwardWedge_ShouldNotAddWedgeForWrongAnswer()
    {
        // Create a player with no wedges.
        var player = new PlayerGameState
        {
            PlayerName = "Player 1"
        };

        var question = new Question
        {
            QuestionId = 31,
            Category = "Java"
        };

        // The player's answer was incorrect.
        var gameAnswer = new GameAnswer
        {
            QuestionId = 31,
            IsCorrect = false
        };

        var gameMechanicsService = new GameMechanicsService();

        bool result = gameMechanicsService.AwardWedge(
            player,
            question,
            gameAnswer
        );

        // No wedge should have been awarded.
        Assert.False(result);

        // Java should not exist in the player's wedge collection.
        Assert.DoesNotContain("Java", player.Wedges);
    }

    // Test for wedge, should not be able to have duplicate of wedges:

    [Fact]
    public void AwardWedge_ShouldNotAddDuplicateWedge()
    {
        // Create a player who already owns the C# wedge.
        var player = new PlayerGameState
        {
            PlayerName = "Player 1",
            Wedges = new HashSet<string> { "C#" }
        };

        var question = new Question
        {
            QuestionId = 32,
            Category = "C#"
        };

        var gameAnswer = new GameAnswer
        {
            QuestionId = 32,
            IsCorrect = true
        };

        var gameMechanicsService = new GameMechanicsService();

        // Try to award the same wedge again.
        bool result = gameMechanicsService.AwardWedge(
            player,
            question,
            gameAnswer
        );

        // No new wedge should have been added.
        Assert.False(result);

        // The player should still only have one wedge.
        Assert.Single(player.Wedges);
    }

}