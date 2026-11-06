// Places BoardSpaceType in the HMJT.Models namespace.
namespace HMJT.Models;

// Defines the different types of spaces
// that can exist on the game board.
public enum BoardSpaceType
{
    // A normal colored space that gives
    // the player a category question.
    Category,

    // A black space that allows the same
    // player to roll the dice again.
    RollAgain,

    // The center of the board where the player
    // can choose a question category.
    Center
}