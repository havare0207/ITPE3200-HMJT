using System;
using System.Linq;
using System.Threading.Tasks;
using HMJT.DAL;
using HMJT.Models;
using HMJT.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMJT.Controllers;

// Only authenticated users can access
// and manage saved custom games.
[Authorize]
public class GameController : Controller
{
    private readonly IGameRepository _gameRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GameController> _logger;

    public GameController(
        IGameRepository gameRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<GameController> logger)
    {
        _gameRepository = gameRepository;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: /Game
    // Shows only the custom games belonging
    // to the currently logged-in user.
    public async Task<IActionResult> Index()
    {
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        IEnumerable<Game> games =
            await _gameRepository.GetAllForUser(userId);

        // Convert the database entities into ViewModels.
        // This prevents the view from depending directly on Game.
        var vm = new GameIndexViewModel
        {
            Games = games.Select(ToViewModel).ToList()
        };

        return View(vm);
    }

    // GET: /Game/Details/5
    // Shows a game only if it belongs
    // to the currently logged-in user.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        var game = await _gameRepository.GetByIdForUser(id, userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Game not found or access denied for game ID: {GameId}",
                id);

            return NotFound();
        }

        // Pass a ViewModel to the Details view instead of the entity.
        return View(ToViewModel(game));
    }

    // GET: /Game/Questions/5
    // Shows all questions belonging to a custom game,
    // but only if the logged-in user owns the game.
    [HttpGet]
    public async Task<IActionResult> Questions(int id)
    {
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        var game = await _gameRepository.GetByIdForUser(id, userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Game not found or access denied for game ID: {GameId}",
                id);

            return NotFound();
        }

        // Build a ViewModel containing the game information
        // and the questions displayed on the Manage Questions page.
        var vm = new GameQuestionsViewModel
        {
            GameId = game.GameId,
            GameName = game.Name,

            // Convert each Question entity into a QuestionViewModel.
            Questions = game.Questions.Select(question =>
                new QuestionViewModel
                {
                    QuestionId = question.QuestionId,
                    GameId = question.GameId,
                    QuestionText = question.QuestionText,
                    Category = question.Category,
                    Difficulty = question.Difficulty
                }).ToList()
        };

        return View(vm);
    }

    // GET: /Game/Create
    [HttpGet]
    public IActionResult Create()
    {
        // Supply an empty ViewModel to the Create view.
        return View(new GameViewModel());
    }

    // POST: /Game/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GameViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            // Return the submitted ViewModel so validation errors appear.
            return View(vm);
        }

        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Create a new entity from the submitted ViewModel.
        // Do not bind the entire database entity directly from the browser.
        var game = new Game
        {
            Name = vm.Name,

            // The owner is always taken from Identity.
            // Never trust an OwnerUserId sent from the browser.
            OwnerUserId = userId,

            // Set the creation date on the server.
            CreatedAt = DateTime.UtcNow,

            // Set the initial status on the server.
            Status = GameStatus.NotStarted
        };

        try
        {
            await _gameRepository.Create(game);

            _logger.LogInformation(
                "Game '{Name}' created with ID {GameId}.",
                game.Name,
                game.GameId);

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error creating game.");

            ModelState.AddModelError(
                string.Empty,
                "An error occurred while creating the game. Please try again.");

            return View(vm);
        }
    }

    // GET: /Game/Update/5
    // The edit page can only be opened
    // by the owner of the custom game.
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        var game = await _gameRepository.GetByIdForUser(id, userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Game not found or access denied for game ID: {GameId}",
                id);

            return NotFound();
        }

        // Convert the entity to a ViewModel for the Edit view.
        return View(ToViewModel(game));
    }

    // POST: /Game/Update/5
    // The game is loaded again from the database
    // using both GameId and OwnerUserId.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(GameViewModel vm)
    {
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Check ownership before allowing an update or returning
        // the submitted form with validation errors.
        var existingGame =
            await _gameRepository.GetByIdForUser(vm.GameId, userId);

        if (existingGame == null)
        {
            _logger.LogWarning(
                "Game not found or access denied for game ID: {GameId}",
                vm.GameId);

            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            // Return the ViewModel so validation messages are displayed.
            return View(vm);
        }

        try
        {
            // Only update properties that the user
            // should be allowed to change.
            existingGame.Name = vm.Name;

            // Do not overwrite OwnerUserId, Status, CreatedAt,
            // or TimesPlayed using values submitted by the browser.
            await _gameRepository.Update(existingGame);

            _logger.LogInformation(
                "Game '{Name}' updated.",
                existingGame.Name);

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error editing game.");

            ModelState.AddModelError(
                string.Empty,
                "An error occurred while editing the game. Please try again.");
        }

        return View(vm);
    }

    // GET: /Game/Delete/5
    // The delete confirmation page is only shown
    // if the current user owns the game.
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        var game = await _gameRepository.GetByIdForUser(id, userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Game not found or access denied for game ID: {GameId}",
                id);

            return NotFound();
        }

        // Only send the fields needed by the confirmation page.
        return View(ToViewModel(game));
    }

    // POST: /Game/Delete/5
    // Deletes the game only if it belongs
    // to the currently logged-in user.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        try
        {
            bool deleted = await _gameRepository.Delete(id, userId);

            if (!deleted)
            {
                _logger.LogWarning(
                    "Game not found or access denied for game ID: {GameId}",
                    id);

                return NotFound();
            }

            _logger.LogInformation(
                "Game with ID {GameId} deleted.",
                id);

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error deleting game.");

            TempData["ErrorMessage"] =
                "An error occurred while deleting the game. Please try again.";

            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    // Converts a Game entity into the ViewModel used by individual Game views.
    // Keeping this mapping in one place avoids repeating the same assignments.
    private static GameViewModel ToViewModel(Game game)
    {
        return new GameViewModel
        {
            GameId = game.GameId,
            Name = game.Name,
            Status = game.Status,
            CreatedAt = game.CreatedAt,
            TimesPlayed = game.TimesPlayed
        };
    }
}