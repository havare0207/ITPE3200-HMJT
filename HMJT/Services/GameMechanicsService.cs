// Organizes this class inside the Services namespace.
// The full class name is HMJT.Services.GameMechanicsService.
//See DiceService.cs for more info about namespace.

namespace HMJT.Services;

public class GameMechanicsService
{
    //makes a variable GetCategoryFromDiceRoll of type string
    //which takes in a number (int diceRoll) and returns category of type string.
    public string GetCategoryFromDiceRoll(int diceRoll)
    {
        string category;

        //if statement to deside which category to return based on diceRoll:

        if (diceRoll == 1)
        {
            category = "Java";
        }
        else if (diceRoll == 2)
        {
            category = "JavaScript";
        }
        else if (diceRoll == 3)
        {
            category = "HTML/CSS";
        }
        else if (diceRoll == 4)
        {
            category = "Python";
        }
        else if (diceRoll == 5)
        {
            category = "Game History";
        }
        else if (diceRoll == 6)
        {
            category = "C#";
        }

        //if the diceRoll number is not between 1 and 6, 
        // throw a ArgumentOutOfRangeExeption.

        else
        {
            throw new ArgumentOutOfRangeException(
                nameof(diceRoll),
                "Dice roll must be between 1 and 6."
            );
        }

        return category;
    }
}
