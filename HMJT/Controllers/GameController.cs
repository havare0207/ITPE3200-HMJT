using HMJT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

namespace HMJT.Controllers;

public class GameController : Controller
{
    private readonly GameDbContext _gameDbcontext;
    private readonly ILogger<GameController> _logger;

    public GameController(GameDbContext gameDbcontext, ILogger<GameController> logger)
    {
        _gameDbcontext = gameDbcontext;
        _logger = logger;
    }
    
    //GET: /Game
    [HttpGet]
    public async Task<IActionResult> Index() 
    {
        var games = await _gameDbcontext.Games.ToListAsync();
        return View(games);
    }

    //GET: /Game/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var game = await _gameDbcontext.Games.FindAsync(id);
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
            _gameDbcontext.Games.Add(game);
            await _gameDbcontext.SaveChangesAsync();
            _logger.LogInformation("Game '{Name}' created.", game.Name);
            return RedirectToAction(nameof(Index));
            
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating game.");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the game. Please try again.");
                return View(game);
            }
        }
        return View(game);
    }

    //GET: /Game/Update/5
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var game = await _gameDbcontext.Games.FindAsync(id);
        if (game == null)
        {
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
            _gameDbcontext.Games.Update(game);
            await _gameDbcontext.SaveChangesAsync();
            _logger.LogInformation("Game '{Name}' updated.", game.Name);
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
        var game = await _gameDbcontext.Games.FindAsync(id);
        if (game == null)
        {
            return NotFound(); 
        }
        return View(game);
    }

    //POST: /Game/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var game = await _gameDbcontext.Games.FindAsync(id);
        if (game == null)
        {
            return NotFound();
        }
        _gameDbcontext.Games.Remove(game);
        await _gameDbcontext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

}