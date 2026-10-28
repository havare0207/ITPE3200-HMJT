using HMJT.Models;
using Microsoft.EntityFrameworkCore;

namespace HMJT.DAL;

public class GameRepository : IGameRepository
{
    private readonly GameDbContext _db;

    public GameRepository(
        GameDbContext db)
    {
        _db = db;
    }

    // Returns only games belonging
    // to the logged-in user.
    public async Task<IEnumerable<Game>>
        GetAllForUser(string userId)
    {
        return await _db.Games
            .Where(game =>
                game.OwnerUserId == userId)
            .OrderByDescending(game =>
                game.CreatedAt)
            .ToListAsync();
    }

    // Returns the game only if the current
    // user is the owner.
    public async Task<Game?>
        GetByIdForUser(
            int id,
            string userId)
    {
        return await _db.Games
            .Include(game =>
                game.Questions)
            .Include(game =>
                game.PresetQuestions)
            .FirstOrDefaultAsync(game =>
                game.GameId == id &&
                game.OwnerUserId == userId);
    }

    // Creates a new custom game.
    public async Task Create(
        Game game)
    {
        _db.Games.Add(game);

        await _db.SaveChangesAsync();
    }

    // Updates an existing custom game.
    public async Task Update(
        Game game)
    {
        _db.Games.Update(game);

        await _db.SaveChangesAsync();
    }

    // Deletes a custom game only if it
    // belongs to the logged-in user.
    public async Task<bool> Delete(
        int id,
        string userId)
    {
        Game? game =
            await _db.Games
                .FirstOrDefaultAsync(game =>
                    game.GameId == id &&
                    game.OwnerUserId == userId);

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