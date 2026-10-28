using HMJT.Models;
using HMJT.DAL;
using HMJT.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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

    // Fixed category options: users cannot add
    // new categories through these question forms.
    private static readonly List<string> CategoryOptions = new()
    {
        "Java",
        "JavaScript",
        "HTML/CSS",
        "Python",
        "Game History",
        "C#"
    };

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
    // Displays questions and their search filters.
    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchString,
        string? category,
        string? difficulty,
        string? gameName)
    {
        // Retrieve all questions from the repository.
        var allQuestions =
            (await _questionRepository.GetAll()).ToList();

        // Get the available game names before filtering
        // so the dropdown retains all available choices.
        var gameNames = allQuestions
            .Where(q => q.Game != null)
            .Select(q => q.Game!.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        // Start with all questions.
        var questions = allQuestions;

        // Filter by question text.
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            questions = questions
                .Where(q => q.QuestionText.Contains(
                    searchString,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Filter by category.
        if (!string.IsNullOrWhiteSpace(category))
        {
            questions = questions
                .Where(q => string.Equals(
                    q.Category,
                    category,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Filter by difficulty.
        if (!string.IsNullOrWhiteSpace(difficulty) &&
            Enum.TryParse<Difficulty>(
                difficulty,
                true,
                out var selectedDifficulty) &&
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
                    q.Game?.Name,
                    gameName,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Create the Index ViewModel.
        // This passes both the filtered questions
        // and the filter selections to the frontend.
        var vm = new QuestionIndexViewModel
        {
            Questions = questions
                .Select(ToViewModel)
                .ToList(),

            SearchString = searchString,
            Category = category,
            Difficulty = difficulty,
            GameName = gameName,

            Categories = CategoryOptions
                .Select(c => new SelectListItem
                {
                    Text = c,
                    Value = c,
                    Selected = c == category
                })
                .ToList(),

            Difficulties = Enum.GetNames(typeof(Difficulty))
                .Select(d => new SelectListItem
                {
                    Text = d,
                    Value = d,
                    Selected = d == difficulty
                })
                .ToList(),

            GameNames = gameNames
                .Select(g => new SelectListItem
                {
                    Text = g,
                    Value = g,
                    Selected = g == gameName
                })
                .ToList()
        };

        // Send the ViewModel to Index.cshtml.
        return View(vm);
    }

    // GET: /Question/Details/5
    //
    // Shows one question only if the
    // logged-in user owns its game.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        // Retrieve the question from the database.
        var question =
            await _questionRepository.GetQuestionById(id);

        if (question == null)
        {
            _logger.LogWarning(
                "Question {QuestionId} was not found (Details)",
                id);

            return NotFound();
        }

        // Identify the currently logged-in user.
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Verify that the game containing
        // this question belongs to the user.
        var game = await _gameRepository.GetByIdForUser(
            question.GameId,
            userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Access denied to Question {QuestionId}",
                id);

            return NotFound();
        }

        // Convert the database model into a ViewModel.
        var vm = ToViewModel(question);
        vm.GameName = game.Name;

        // Send the ViewModel to Details.cshtml.
        return View(vm);
    }

    // GET: /Question/Create?gameId=5
    //
    // Shows the form for creating
    // a question for a specific game.
    [HttpGet]
    public async Task<IActionResult> Create(int gameId)
    {
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // The game must belong to
        // the currently logged-in user.
        var game = await _gameRepository.GetByIdForUser(
            gameId,
            userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Game {GameId} was not found or access was denied",
                gameId);

            return NotFound();
        }

        // Create a ViewModel for the form.
        var vm = new QuestionViewModel
        {
            GameId = gameId,
            GameName = game.Name
        };

        // Populate dropdowns for the Create form.
        PopulateFormOptions(vm);

        // Send the ViewModel to Create.cshtml.
        return View(vm);
    }

    // POST: /Question/Create
    //
    // Saves a new question to
    // a user-owned custom game.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuestionViewModel vm)
    {
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Never trust only the GameId sent
        // by the browser.
        //
        // Verify that the selected game
        // actually belongs to this user.
        var game = await _gameRepository.GetByIdForUser(
            vm.GameId,
            userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Game {GameId} was not found or access was denied",
                vm.GameId);

            return NotFound();
        }

        // Check if the ViewModel passes validation.
        if (!ModelState.IsValid)
        {
            // Restore display information and dropdowns
            // before returning the form to the user.
            vm.GameName = game.Name;
            PopulateFormOptions(vm);

            _logger.LogWarning(
                "Creating a question failed validation");

            return View(vm);
        }

        // Convert the ViewModel into the database model.
        var question = ToModel(vm);

        // Add the question to the database.
        await _questionRepository.Create(question);

        _logger.LogInformation(
            "Question {QuestionId} was created for Game {GameId}",
            question.QuestionId,
            question.GameId);

        // Go back to the questions
        // belonging to this game.
        return RedirectToAction(
            "Questions",
            "Game",
            new { id = question.GameId });
    }

    // GET: /Question/Edit/5
    //
    // Shows the edit form only if
    // the user owns the question's game.
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        // Retrieve the question from the database.
        var question =
            await _questionRepository.GetQuestionById(id);

        if (question == null)
        {
            _logger.LogWarning(
                "Question {QuestionId} was not found (Edit)",
                id);

            return NotFound();
        }

        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Verify ownership of the associated game.
        var game = await _gameRepository.GetByIdForUser(
            question.GameId,
            userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Access denied to Question {QuestionId}",
                id);

            return NotFound();
        }

        // Convert the database model into a ViewModel.
        var vm = ToViewModel(question);
        vm.GameName = game.Name;

        // Populate dropdowns for the Edit form.
        PopulateFormOptions(vm);

        // Send the ViewModel to Edit.cshtml.
        return View(vm);
    }

    // POST: /Question/Edit
    //
    // Saves the edited question only
    // if the user owns its game.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(QuestionViewModel vm)
    {
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Load the existing question from
        // the database instead of trusting
        // all values sent by the browser.
        var existingQuestion =
            await _questionRepository.GetQuestionById(
                vm.QuestionId);

        if (existingQuestion == null)
        {
            _logger.LogWarning(
                "Question {QuestionId} was not found (Edit)",
                vm.QuestionId);

            return NotFound();
        }

        // Verify ownership using the GameId
        // stored in the database.
        var game = await _gameRepository.GetByIdForUser(
            existingQuestion.GameId,
            userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Access denied to Question {QuestionId}",
                vm.QuestionId);

            return NotFound();
        }

        // Check whether the submitted ViewModel is valid.
        if (!ModelState.IsValid)
        {
            // Preserve the original game relationship
            // and restore the dropdown options.
            vm.GameId = existingQuestion.GameId;
            vm.GameName = game.Name;

            PopulateFormOptions(vm);

            _logger.LogWarning(
                "Editing question {QuestionId} failed validation",
                vm.QuestionId);

            return View(vm);
        }

        // Update the tracked question instead of
        // attaching a second instance with the same ID.

        // Update the existing database model using the submitted ViewModel.
        existingQuestion.QuestionText = vm.QuestionText;
        existingQuestion.Category = vm.Category;
        existingQuestion.Difficulty = vm.Difficulty;
        existingQuestion.AnswerA = vm.AnswerA;
        existingQuestion.AnswerB = vm.AnswerB;
        existingQuestion.AnswerC = vm.AnswerC;
        existingQuestion.AnswerD = vm.AnswerD;
        existingQuestion.CorrectAnswer = vm.CorrectAnswer;

        // Keep the original GameId from the database.
        await _questionRepository.Update(existingQuestion);

        _logger.LogInformation(
            "Question {QuestionId} was updated",
            vm.QuestionId);

        // Return to the questions belonging to this game.
        return RedirectToAction(
            "Questions",
            "Game",
            new { id = existingQuestion.GameId });
    }
    // GET: /Question/Delete/5
    //
    // Shows the delete confirmation only
    // if the user owns the question's game.
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        // Retrieve the question from the database.
        var question =
            await _questionRepository.GetQuestionById(id);

        if (question == null)
        {
            _logger.LogWarning(
                "Question {QuestionId} was not found (Delete)",
                id);

            return NotFound();
        }

        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Verify ownership of the associated game.
        var game = await _gameRepository.GetByIdForUser(
            question.GameId,
            userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Access denied to Question {QuestionId}",
                id);

            return NotFound();
        }

        // Convert the database model into a ViewModel.
        var vm = ToViewModel(question);
        vm.GameName = game.Name;

        // Send the ViewModel to Delete.cshtml.
        return View(vm);
    }

    // POST: /Question/Delete/5
    //
    // Deletes the question only if
    // the user owns its game.
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Retrieve the question from the database.
        var question =
            await _questionRepository.GetQuestionById(id);

        if (question == null)
        {
            _logger.LogWarning(
                "Question {QuestionId} was not found (DeleteConfirmed)",
                id);

            return NotFound();
        }

        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Verify ownership before deleting.
        var game = await _gameRepository.GetByIdForUser(
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
        int gameId = question.GameId;

        // Delete the question from the database.
        await _questionRepository.Delete(id);

        _logger.LogInformation(
            "Question {QuestionId} was deleted",
            id);

        // Go back to the questions
        // belonging to this game.
        return RedirectToAction(
            "Questions",
            "Game",
            new { id = gameId });
    }

    // Converts a database Question model into
    // a QuestionViewModel for displaying in a view.
    private static QuestionViewModel ToViewModel(Question question)
    {
        return new QuestionViewModel
        {
            QuestionId = question.QuestionId,
            QuestionText = question.QuestionText,
            Category = question.Category,
            Difficulty = question.Difficulty,
            AnswerA = question.AnswerA,
            AnswerB = question.AnswerB,
            AnswerC = question.AnswerC,
            AnswerD = question.AnswerD,
            CorrectAnswer = question.CorrectAnswer,
            GameId = question.GameId,
            GameName = question.Game?.Name
        };
    }

    // Converts a QuestionViewModel submitted by the frontend
    // into a Question model that can be saved to the database.
    //
    // The navigation property Game is deliberately not mapped.
    // The game relationship is checked separately by the controller.
    private static Question ToModel(QuestionViewModel vm)
    {
        return new Question
        {
            QuestionId = vm.QuestionId,
            QuestionText = vm.QuestionText,
            Category = vm.Category,
            Difficulty = vm.Difficulty,
            AnswerA = vm.AnswerA,
            AnswerB = vm.AnswerB,
            AnswerC = vm.AnswerC,
            AnswerD = vm.AnswerD,
            CorrectAnswer = vm.CorrectAnswer,
            GameId = vm.GameId
        };
    }

    // Populates dropdown options used by the Create and Edit forms.
    //
    // This helper must also be called when ModelState is invalid,
    // because dropdown options are not automatically retained
    // when a form is returned after validation errors.
    private static void PopulateFormOptions(QuestionViewModel vm)
    {
        vm.Categories = CategoryOptions
            .Select(category => new SelectListItem
            {
                Text = category,
                Value = category,
                Selected = category == vm.Category
            })
            .ToList();

        vm.Difficulties = Enum.GetNames(typeof(Difficulty))
            .Select(difficulty => new SelectListItem
            {
                Text = difficulty,
                Value = difficulty,
                Selected = difficulty == vm.Difficulty.ToString()
            })
            .ToList();
    }
}
