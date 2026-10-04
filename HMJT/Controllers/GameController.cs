using HMJT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

namespace HMJT.Controllers;

public class GameController : Controller
{
    private readonly GameDbContext _gameDbcontext;

    public GameController(GameDbContext gameDbcontext)
    {
        _gameDbcontext = gameDbcontext;
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
    public async Task<IActionResult> Create(Game game)
    {
        if (ModelState.IsValid)
        {
            game.Status = GameStatus.InProgress;
            _gameDbcontext.Games.Add(game);
            await _gameDbcontext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
    public async Task<IActionResult> Update(Game game)
    {
        if (ModelState.IsValid)
        {
            _gameDbcontext.Games.Update(game);
            await _gameDbcontext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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