// Gives this test file access to classes inside the HMJT.Services namespace,
// for example DiceService.
using HMJT.Services;

// Organizes tests that test classes from the Services part of the application.
// For example DiceServiceTests and GameMechanicsServiceTests.
// The full name of this class is:
// HMJT.Tests.Services.DiceServiceTests

namespace HMJT.Tests.Services;

public class DiceServiceTests
{
    //[Fact] comes from xUnit and means this is a testmethod, 
    // of which xUnit will run:
    [Fact]
    public void RollDice_ShouldReturnNumberBetweenOneAndSix()
    {
        // Create an instance of DiceService so we can test RollDice().
        var diceService = new DiceService();

        //A for loop rolling 100 dices, and saving each roll to int result.
        // We want to roll the dice multiple times, because the result is random.
        for (int i = 0; i < 100; i++)
        {
            //saves the result of the dicerolls
            int result = diceService.RollDice();

            // Every result must be between 1 and 6.
            //Assert checks if the result is in the range between 1 and 6. 
            // If not, the test fails. 
            Assert.InRange(result, 1, 6);
        }
    }
}