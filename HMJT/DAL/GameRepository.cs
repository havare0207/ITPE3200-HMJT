using Microsoft.EntityFrameworkCore;
using HMJT.Models;

namespace HMJT.DAL;
public class GameRepository : IGameRepository
{
    private readonly GameDbContext _db;

    public GameRepository(GameDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Game>> GetAll()
    {
        return await _db.Games.ToListAsync();
    }

    public async Task<Game?> GetGameById(int id)
    {
        return await _db.Games
        .Include(g => g.PresetQuestions)
        .FirstOrDefaultAsync(g => g.GameId == id);
    }

    public async Task Create(Game game)
    {
        _db.Games.Add(game);
        await _db.SaveChangesAsync();
    }

    public async Task Update(Game game)
    {
        _db.Games.Update(game);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var game = await _db.Games.FindAsync(id);
        if (game == null)
        {
            return false;
        }

        _db.Games.Remove(game);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task SetPresetQuestion(int gameId, IEnumerable<int> presetQuestionIds)
{
    var game = await _db.Games.Include(g => g.PresetQuestions).FirstOrDefaultAsync(g => g.GameId == gameId);
    if (game == null) return;

    var selected = await _db.PresetQuestion.Where(pq => presetQuestionIds.Contains(pq.PresetQuestionId)).ToListAsync();
    game.PresetQuestions.Clear();
    foreach (var pq in selected)
    {
        game.PresetQuestions.Add(pq);
    }

    await _db.SaveChangesAsync();
}
}