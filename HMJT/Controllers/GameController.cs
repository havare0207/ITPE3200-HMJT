using HMJT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMJT.DAL;

namespace HMJT.Controllers;

public class GameController : Controller
{
    private readonly IGameRepository _gameRepository;
    private readonly ILogger<GameController> _logger;

    public GameController(IGameRepository gameRepository, ILogger<GameController> logger)
    {
        _gameRepository = gameRepository;
        _logger = logger;
    }
    
    //GET: /Game
    [HttpGet]
    public async Task<IActionResult> Index() 
    {
        var games = await _gameRepository.GetAll();
        return View(games);
    }

    //GET: /Game/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var game = await _gameRepository.GetGameById(id);
        if (game == null)
        {
            _logger.LogWarning("Game not found with ID: {GameId}", id);           
            return NotFound();
        }
        return View(game);
    }

    // GET: /Game/Questions/5
    // Shows all questions belonging to a specific game.
    [HttpGet]
    public async Task<IActionResult> Questions(int id)
    {
        var game = await _gameRepository.GetGameById(id);
        if (game == null)
        {
            _logger.LogWarning("Game not found with ID: {GameId}", id);
            return NotFound();
        }

        return View(game);
    }

    //GET: /Game/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
}

    //POST: /Game/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Game game)
    {
        if (ModelState.IsValid)
        {
            try
            {
            game.Status = GameStatus.NotStarted;
            await _gameRepository.Create(game);
            _logger.LogInformation("Game '{Name}' created.", game.Name);
            return RedirectToAction(nameof(Index));
            
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating game.");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the game. Please try again.");
            }
        }
        return View(game);
    }

    //GET: /Game/Update/5
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var game = await _gameRepository.GetGameById(id);
        if (game == null)
        {
            _logger.LogWarning("Game not found with ID: {GameId}", id);
            return NotFound();
        }
        return View(game);
    }

    //POST: /Game/Update/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Game game)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var existingGame = await _gameRepository.GetGameById(game.GameId);
                if (existingGame == null)
                {
                    _logger.LogWarning("Game not found with ID: {GameId}", game.GameId);
                    return NotFound();
                }
                
            existingGame.Name = game.Name;
            await _gameRepository.Update(existingGame);
            _logger.LogInformation("Game '{Name}' updated.", existingGame.Name);
            return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error editing game.");
                ModelState.AddModelError(string.Empty, "An error occurred while editing the game. Please try again.");
            }
        }
        return View(game);
    }

    //GET: /Game/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var game = await _gameRepository.GetGameById(id);
        if (game == null)
        {
            _logger.LogWarning("Game not found with ID: {GameId}", id);
            return NotFound(); 
        }
        return View(game);
    }

    //POST: /Game/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var deleted = await _gameRepository.Delete(id);
            if (!deleted)
            {
                _logger.LogWarning("Game not found with ID: {GameId}", id);
                return NotFound();
            }

            _logger.LogInformation("Game with ID {GameId} deleted.", id);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error deleting game.");
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the game. Please try again.");
            return RedirectToAction(nameof(Delete), new { id });
        }
        
    }

}