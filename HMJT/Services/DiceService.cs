//To avoid collution between classes, namespace is used 
// to organise classes with the same name, f.ex:
//namespace HMJT.Services;
//public class Game { }
//&
//namespace HMJT.Models;
//public class Game { }

//The full name of this class is HMJT.Services.DiceService

namespace HMJT.Services;

public class DiceService
{
    //Make a diceroll between 1 and 7, excluding 7. Therefore roll a 6 sided dice.
    public int RollDice()
    {
        return Random.Shared.Next(1, 7);
    }
}