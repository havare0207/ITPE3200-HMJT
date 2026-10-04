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
        var questions = await _gameDbcontext.Questions.ToListAsync();
        
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

    // Shows the form for creating a new question.
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // Saves a new question to the database.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Question question)
    {
        // Check if the question passes the validation rules.
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Creating a question failed validation");
            return View(question);
        }

        // Add the question to the database.
        _gameDbcontext.Questions.Add(question);

        // Save the changes.
        await _gameDbcontext.SaveChangesAsync();

        _logger.LogInformation("Question {QuestionId} was created", question.QuestionId);

        // Go back to the question list.
        return RedirectToAction(nameof(Index));
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
            _logger.LogWarning("Editing question {QuestionId} failed validation", question.QuestionId);
            return View(question);
        }

        // Update the question in the database.
        _gameDbcontext.Questions.Update(question);

        // Save the changes.
        await _gameDbcontext.SaveChangesAsync();

        _logger.LogInformation("Question {QuestionId} was updated", question.QuestionId);

        // Go back to the question list.
        return RedirectToAction(nameof(Index));
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
    [ActionName("Delete")] // Maps this POST method to the "Delete" action name used by the form.
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var question = await _gameDbcontext.Questions.FindAsync(id);

        // If the question does not exist, show "Not Found".
        if (question == null)
        {
            _logger.LogWarning("Question {QuestionId} was not found (DeleteConfirmed)", id);
            return NotFound();
        }

        // Remove the question from the database.
        _gameDbcontext.Questions.Remove(question);

        // Save the changes.
        await _gameDbcontext.SaveChangesAsync();

        _logger.LogInformation("Question {QuestionId} was deleted", id);

        // Go back to the question list.
        return RedirectToAction(nameof(Index));
    }
}
