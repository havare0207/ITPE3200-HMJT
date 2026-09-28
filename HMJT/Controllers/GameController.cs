// Gives this controller access to ASP.NET Core MVC.
using Microsoft.AspNetCore.Mvc;

// Gives this controller access to the game services.
using HMJT.Services;

using HMJT.Models;

// Places GameController in the HMJT.Controllers namespace.
namespace HMJT.Controllers;

// Handles requests related to the game page.
public class GameController : Controller
{
    // Service used for starting and completing game turns.
    private readonly GameTurnService _gameTurnService;

    // Constructor for GameController.
    // GameTurnService is provided through dependency injection.
    public GameController(GameTurnService gameTurnService)
    {
        _gameTurnService = gameTurnService;
    }

    // Displays the main game page with temporary game data.
    // This will later be replaced with data from the active game session.
    public IActionResult Play()
    {
        // Create temporary data so the ViewModel
        // can be tested before the full game setup is connected.
        var viewModel = new GamePlayViewModel
        {
            PlayerName = "Player 1",
            PlayerNumber = 1,
            DiceRoll = 4,
            Category = "Python",

            Question = new Question
            {
                QuestionId = 1,
                QuestionText = "Which keyword is used to define a function in Python?",
                Category = "Python",
                Difficulty = Difficulty.Easy,

                AnswerA = "function",
                AnswerB = "func",
                AnswerC = "def",
                AnswerD = "method",

                CorrectAnswer = AnswerOption.C
            },

            Wedges = new HashSet<string>
            {
                "Java",
                "Python"
            },

            MissingWedges = new List<string>
            {
                "JavaScript",
                "HTML/CSS",
                "Game History",
                "C#"
            },

            IsFinalTurn = false,
            IsGameOver = false
        };

        // Sends the ViewModel to Views/Game/Play.cshtml.
        return View(viewModel);
    }
}