using HMJT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using HMJT.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMJT.Controllers;

// Only authenticated users can access
// and manage custom game questions.
[Authorize]
public class QuestionController : Controller
{
    // Gives the controller access to
    // questions stored in the database.
    private readonly IQuestionRepository _questionRepository;

    // Gives the controller access to
    // the games that questions belong to.
    private readonly IGameRepository _gameRepository;

    // Used to identify the currently
    // logged-in ASP.NET Core Identity user.
    private readonly UserManager<ApplicationUser> _userManager;

    // Used to write messages to
    // the application log.
    private readonly ILogger<QuestionController> _logger;


    // Gets the repositories, Identity UserManager
    // and logger through dependency injection.
    public QuestionController(
        IQuestionRepository questionRepository,
        IGameRepository gameRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<QuestionController>? logger = null)
    {
        _questionRepository = questionRepository;
        _gameRepository = gameRepository;
        _userManager = userManager;

        _logger =
            logger ??
            NullLogger<QuestionController>.Instance;
    }


    // GET: /Question
[HttpGet]
public async Task<IActionResult> Index(
    string? searchString,
    string? category,
    string? difficulty,
    string? gameName)
{
    var questions = (await _questionRepository.GetAll()).ToList();

    // Fixed category options: users cannot add new categories here.
    var categories = new List<string>
    {
        "Java",
        "JavaScript",
        "HTML/CSS",
        "Python",
        "Game History",
        "C#"
    };

    // Populate the dropdowns and keep the selected filters.
    ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
        categories, category);

    ViewBag.Difficulties = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
        Enum.GetNames(typeof(Difficulty)), difficulty);

    ViewBag.GameNames = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
        questions
            .Where(q => q.Game != null)
            .Select(q => q.Game!.Name)
            .Distinct()
            .OrderBy(name => name),
        gameName);

    ViewBag.SearchString = searchString;
    ViewBag.Category = category;
    ViewBag.Difficulty = difficulty;
    ViewBag.GameName = gameName;

    // Filter by question text.
    if (!string.IsNullOrWhiteSpace(searchString))
    {
        questions = questions
            .Where(q => q.QuestionText.Contains(
                searchString, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // Filter by category.
    if (!string.IsNullOrWhiteSpace(category))
    {
        questions = questions
            .Where(q => string.Equals(
                q.Category, category,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // Filter by difficulty.
    if (!string.IsNullOrWhiteSpace(difficulty) &&
        Enum.TryParse<Difficulty>(
            difficulty, true, out var selectedDifficulty) &&
        Enum.IsDefined(typeof(Difficulty), selectedDifficulty))
    {
        questions = questions
            .Where(q => q.Difficulty == selectedDifficulty)
            .ToList();
    }

    // Filter by game name.
    if (!string.IsNullOrWhiteSpace(gameName))
    {
        questions = questions
            .Where(q => string.Equals(
                q.Game?.Name, gameName,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    return View(questions);
}



    // GET: /Question/Details/5
    //
    // Shows one question only if the
    // logged-in user owns its game.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var question =
            await _questionRepository
                .GetQuestionById(id);

        if (question == null)
        {
            _logger.LogWarning(
                "Question {QuestionId} was not found (Details)",
                id);

            return NotFound();
        }

        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Verify that the game containing
        // this question belongs to the user.
        var game =
            await _gameRepository
                .GetByIdForUser(
                    question.GameId,
                    userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Access denied to Question {QuestionId}",
                id);

            return NotFound();
        }

        return View(question);
    }


    // GET: /Question/Create?gameId=5
    //
    // Shows the form for creating
    // a question for a specific game.
    [HttpGet]
    public async Task<IActionResult> Create(
        int gameId)
    {
        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // The game must belong to
        // the currently logged-in user.
        var game =
            await _gameRepository
                .GetByIdForUser(
                    gameId,
                    userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Game {GameId} was not found or access was denied",
                gameId);

            return NotFound();
        }

        // Pass the game to the Create view.
        ViewBag.Game = game;

        return View();
    }


    // POST: /Question/Create
    //
    // Saves a new question to
    // a user-owned custom game.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        Question question)
    {
        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Never trust only the GameId sent
        // by the browser.
        //
        // Verify that the selected game
        // actually belongs to this user.
        var game =
            await _gameRepository
                .GetByIdForUser(
                    question.GameId,
                    userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Game {GameId} was not found or access was denied",
                question.GameId);

            return NotFound();
        }

        // Check if the question passes
        // the validation rules.
        if (!ModelState.IsValid)
        {
            // Pass the game back to the view
            // so it still knows which game
            // the question belongs to.
            ViewBag.Game = game;

            _logger.LogWarning(
                "Creating a question failed validation");

            return View(question);
        }

        // Add the question to the database.
        await _questionRepository
            .Create(question);

        _logger.LogInformation(
            "Question {QuestionId} was created for Game {GameId}",
            question.QuestionId,
            question.GameId);

        // Go back to the questions
        // belonging to this game.
        return RedirectToAction(
            "Questions",
            "Game",
            new
            {
                id = question.GameId
            });
    }


    // GET: /Question/Edit/5
    //
    // Shows the edit form only if
    // the user owns the question's game.
    [HttpGet]
    public async Task<IActionResult> Edit(
        int id)
    {
        var question =
            await _questionRepository
                .GetQuestionById(id);

        if (question == null)
        {
            _logger.LogWarning(
                "Question {QuestionId} was not found (Edit)",
                id);

            return NotFound();
        }

        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        var game =
            await _gameRepository
                .GetByIdForUser(
                    question.GameId,
                    userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Access denied to Question {QuestionId}",
                id);

            return NotFound();
        }

        return View(question);
    }


    // POST: /Question/Edit
    //
    // Saves the edited question only
    // if the user owns its game.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Question question)
    {
        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Load the existing question from
        // the database instead of trusting
        // all values sent by the browser.
        var existingQuestion =
            await _questionRepository
                .GetQuestionById(
                    question.QuestionId);

        if (existingQuestion == null)
        {
            _logger.LogWarning(
                "Question {QuestionId} was not found (Edit)",
                question.QuestionId);

            return NotFound();
        }

        // Verify ownership using the GameId
        // stored in the database.
        var game =
            await _gameRepository
                .GetByIdForUser(
                    existingQuestion.GameId,
                    userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Access denied to Question {QuestionId}",
                question.QuestionId);

            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning(
                "Editing question {QuestionId} failed validation",
                question.QuestionId);

            return View(question);
        }

        // Update the tracked question instead of
        // attaching a second instance with the same ID.
        existingQuestion.QuestionText =
            question.QuestionText;

        existingQuestion.Category =
            question.Category;

        existingQuestion.Difficulty =
            question.Difficulty;

        existingQuestion.AnswerA =
            question.AnswerA;

        existingQuestion.AnswerB =
            question.AnswerB;

        existingQuestion.AnswerC =
            question.AnswerC;

        existingQuestion.AnswerD =
            question.AnswerD;

        existingQuestion.CorrectAnswer =
            question.CorrectAnswer;

        // Keep the original game relationship automatically,
        // because existingQuestion already has the correct GameId.
        await _questionRepository
            .Update(existingQuestion);

        _logger.LogInformation(
            "Question {QuestionId} was updated",
            question.QuestionId);

        // Go back to the questions
        // belonging to this game.
        return RedirectToAction(
            "Questions",
            "Game",
            new
            {
                id = question.GameId
            });
    }


    // GET: /Question/Delete/5
    //
    // Shows the delete confirmation only
    // if the user owns the question's game.
    [HttpGet]
    public async Task<IActionResult> Delete(
        int id)
    {
        var question =
            await _questionRepository
                .GetQuestionById(id);

        if (question == null)
        {
            _logger.LogWarning(
                "Question {QuestionId} was not found (Delete)",
                id);

            return NotFound();
        }

        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        var game =
            await _gameRepository
                .GetByIdForUser(
                    question.GameId,
                    userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Access denied to Question {QuestionId}",
                id);

            return NotFound();
        }

        return View(question);
    }


    // POST: /Question/Delete/5
    //
    // Deletes the question only if
    // the user owns its game.
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        int id)
    {
        var question =
            await _questionRepository
                .GetQuestionById(id);

        if (question == null)
        {
            _logger.LogWarning(
                "Question {QuestionId} was not found (DeleteConfirmed)",
                id);

            return NotFound();
        }

        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Verify ownership before deleting.
        var game =
            await _gameRepository
                .GetByIdForUser(
                    question.GameId,
                    userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Access denied to Question {QuestionId}",
                id);

            return NotFound();
        }

        // Remember the game before
        // deleting the question.
        int gameId =
            question.GameId;

        await _questionRepository
            .Delete(id);

        _logger.LogInformation(
            "Question {QuestionId} was deleted",
            id);

        // Go back to the questions
        // belonging to this game.
        return RedirectToAction(
            "Questions",
            "Game",
            new
            {
                id = gameId
            });
    }
}