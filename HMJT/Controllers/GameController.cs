using HMJT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMJT.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HMJT.Controllers;

// Only authenticated users can access
// and manage saved custom games.
//[Authorize]
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
        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        IEnumerable<Game> games =
            await _gameRepository
                .GetAllForUser(userId);

        return View(games);
    }



    // GET: /Game/Details/5
    // Shows a game only if it belongs
    // to the currently logged-in user.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }
        var game =
            await _gameRepository
                .GetByIdForUser(id, userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Game not found or access denied for game ID: {GameId}",
                id);

            return NotFound();
        }

        return View(game);
    }


    // GET: /Game/Questions/5
    // Shows all questions belonging to a custom game,
    // but only if the logged-in user owns the game.
    [HttpGet]
    public async Task<IActionResult> Questions(int id)
    {
        string? userId =
            _userManager.GetUserId(User);

     if (userId == null)
        {
            return Challenge();
        }

        var game =
            await _gameRepository
                .GetByIdForUser(id, userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Game not found or access denied for game ID: {GameId}",
                id);

            return NotFound();
        }

        return View(game);
    }


    // GET: /Game/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }


    // POST: /Game/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Game game)
    {
        if (!ModelState.IsValid)
        {
            return View(game);
        }

        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // The owner is always taken from Identity.
        // Never trust an OwnerUserId sent from the browser.
        game.OwnerUserId =
            userId;

        game.CreatedAt =
            DateTime.UtcNow;

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
            _logger.LogError(
                ex,
                "Error creating game.");

            ModelState.AddModelError(
                string.Empty,
                "An error occurred while creating the game. Please try again.");

            return View(game);
        }
    }


    // GET: /Game/Update/5
    // The edit page can only be opened
    // by the owner of the custom game.
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        var game =
            await _gameRepository
                .GetByIdForUser(id, userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Game not found or access denied for game ID: {GameId}",
                id);

            return NotFound();
        }

        return View(game);
    }


    // POST: /Game/Update/5
    // The game is loaded again from the database
    // using both GameId and OwnerUserId.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Game game)
    {
        if (!ModelState.IsValid)
        {
            return View(game);
        }

        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        try
        {
            var existingGame =
                await _gameRepository
                    .GetByIdForUser(
                        game.GameId,
                        userId);

            if (existingGame == null)
            {
                _logger.LogWarning(
                    "Game not found or access denied for game ID: {GameId}",
                    game.GameId);

                return NotFound();
            }

            // Only update properties that the user
            // should be allowed to change.
            existingGame.Name =
                game.Name;

            await _gameRepository
                .Update(existingGame);

            _logger.LogInformation(
                "Game '{Name}' updated.",
                existingGame.Name);

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Error editing game.");

            ModelState.AddModelError(
                string.Empty,
                "An error occurred while editing the game. Please try again.");
        }

        return View(game);
    }


    // GET: /Game/Delete/5
    // The delete confirmation page is only shown
    // if the current user owns the game.
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        var game =
            await _gameRepository
                .GetByIdForUser(id, userId);

        if (game == null)
        {
            _logger.LogWarning(
                "Game not found or access denied for game ID: {GameId}",
                id);

            return NotFound();
        }

        return View(game);
    }


    // POST: /Game/Delete/5
    // Deletes the game only if it belongs
    // to the currently logged-in user.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        string? userId =
            _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        try
        {
            bool deleted =
                await _gameRepository
                    .Delete(id, userId);

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
            _logger.LogError(
                ex,
                "Error deleting game.");

            TempData["ErrorMessage"] =
                "An error occurred while deleting the game. Please try again.";

            return RedirectToAction(
                nameof(Delete),
                new { id });
        }
    }
}