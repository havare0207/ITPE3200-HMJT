using HMJT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMJT.Controllers;

public class QuestionController : Controller
{
    // Gives the controller access to the database. 
    private readonly GameDbContext _gameDbcontext;
    // Gets the database context from Program.cs. 
    public QuestionController(GameDbContext gameDbcontext)
    {
        _gameDbcontext = gameDbcontext;

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
            return View(question);
        }

        // Add the question to the database.
        _gameDbcontext.Questions.Add(question);

        // Save the changes.
        await _gameDbcontext.SaveChangesAsync();

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
            return View(question);
        }

        // Update the question in the database.
        _gameDbcontext.Questions.Update(question);

        // Save the changes.
        await _gameDbcontext.SaveChangesAsync();

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
            return NotFound();
        }

        // Remove the question from the database.
        _gameDbcontext.Questions.Remove(question);

        // Save the changes.
        await _gameDbcontext.SaveChangesAsync();

        // Go back to the question list.
        return RedirectToAction(nameof(Index));
    }
}

 