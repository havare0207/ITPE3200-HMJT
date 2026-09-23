// Gives this test file access to GameMechanicsService
// from the main HMJT.Services namespace.

using System.Reflection;
using HMJT.Services;

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

}