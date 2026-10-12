using HMJT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace HMJT.Controllers;

public class QuestionController : Controller
{
    // Gives the controller access to the database. 
    private readonly GameDbContext _gameDbcontext;

    // Used to write messages to the log (terminal output).
    private readonly ILogger<QuestionController> _logger;

    // Gets the database context and logger from Program.cs.
    // The logger is optional, so existing unit tests that only pass the database still work.
    public QuestionController(GameDbContext gameDbcontext, ILogger<QuestionController>? logger = null)
    {
        _gameDbcontext = gameDbcontext;
        _logger = logger ?? NullLogger<QuestionController>.Instance;
    }

     // Shows all questions.
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var questions = await _gameDbcontext.Questions
     .Include(q => q.Game)
     .ToListAsync();

        return View(questions);
    }

    // Shows one question.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var question = await _gameDbcontext.Questions.FindAsync(id);

        // If the question does not exist, show "Not Found".
        if (question == null)
        {
            _logger.LogWarning("Question {QuestionId} was not found (Details)", id);
            return NotFound();
        }

        return View(question);
    }

    // Shows the form for creating a new question for a specific game.
    [HttpGet]
    public async Task<IActionResult> Create(int gameId)
    {
        var game = await _gameDbcontext.Games.FindAsync(gameId);

        // If the game does not exist, show "Not Found".
        if (game == null)
        {
            return NotFound();
        }

        // Passes the game to the Create view.
        ViewBag.Game = game;

        return View();
    }

    // Saves a new question to the database.
    [HttpPost]
    [ValidateAntiForgeryToken]
    
    public async Task<IActionResult> Create(Question question)
    {
        // Find the game that the question belongs to.
        var game = await _gameDbcontext.Games.FindAsync(question.GameId);

        // If the game does not exist, return "Not Found".
        if (game == null)
        {
            return NotFound();
        }

        // Check if the question passes the validation rules.
        if (!ModelState.IsValid)
        {
            // Pass the game back to the view so the page
            // still knows which game the question belongs to.
            ViewBag.Game = game;

            _logger.LogWarning(
                "Creating a question failed validation"
            );

            return View(question);
        }

        // Add the question to the database.
        _gameDbcontext.Questions.Add(question);

        // Save the changes.
        await _gameDbcontext.SaveChangesAsync();

        _logger.LogInformation(
            "Question {QuestionId} was created for Game {GameId}",
            question.QuestionId,
            question.GameId
        );

        // Go back to the questions for this game.
        return RedirectToAction(
            "Questions",
            "Game",
            new { id = question.GameId }
        );
    }

    // Shows the form for editing a question.
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var question = await _gameDbcontext.Questions.FindAsync(id);

        // If the question does not exist, show "Not Found".
        if (question == null)
        {
            _logger.LogWarning("Question {QuestionId} was not found (Edit)", id);
            return NotFound();
        }

        return View(question);
    }

    // Saves the edited question.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Question question)
    {
        // Check if the edited question is valid.
        if (!ModelState.IsValid)
        {
            _logger.LogWarning(
                "Editing question {QuestionId} failed validation",
                question.QuestionId);

            return View(question);
        }

        // Update the question in the database.
        _gameDbcontext.Questions.Update(question);

        // Save the changes.
        await _gameDbcontext.SaveChangesAsync();

        _logger.LogInformation(
            "Question {QuestionId} was updated",
            question.QuestionId);

        // Go back to the questions for this game.
        return RedirectToAction(
            "Questions",
            "Game",
            new { id = question.GameId });
    }

    // Shows the delete confirmation page.
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var question = await _gameDbcontext.Questions.FindAsync(id);

        // If the question does not exist, show "Not Found".
        if (question == null)
        {
            _logger.LogWarning("Question {QuestionId} was not found (Delete)", id);
            return NotFound();
        }

        return View(question);
    }

    // Deletes the question from the database.
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var question = await _gameDbcontext.Questions.FindAsync(id);

        // If the question does not exist, show "Not Found".
        if (question == null)
        {
            _logger.LogWarning(
                "Question {QuestionId} was not found (DeleteConfirmed)",
                id);

            return NotFound();
        }

        // Remember which game the question belongs to.
        var gameId = question.GameId;

        // Remove the question from the database.
        _gameDbcontext.Questions.Remove(question);

        // Save the changes.
        await _gameDbcontext.SaveChangesAsync();

        _logger.LogInformation("Question {QuestionId} was deleted", id);

        // Go back to the questions for this game.
        return RedirectToAction(
            "Questions",
            "Game",
            new { id = gameId });
    }
}