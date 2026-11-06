using HMJT.Models;
using Microsoft.EntityFrameworkCore;
namespace HMJT.DAL;

public class PresetQuestionRepository : IPresetQuestionRepository
{
    private readonly GameDbContext _db;

    public PresetQuestionRepository(GameDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<PresetQuestion>> GetAll()
    {
        return await _db.PresetQuestion.ToListAsync();
    }

    public async Task<PresetQuestion?> GetPresetQuestionById(int id)
    {
        return await _db.PresetQuestion.FindAsync(id);
    }

    public async Task<IEnumerable<PresetQuestion>> GetByIds(IEnumerable<int> ids)
    {
        return await _db.PresetQuestion.Where(pq => ids.Contains(pq.PresetQuestionId)).ToListAsync();
    }

    public async Task<IEnumerable<PresetQuestion>> Search(string? category, Difficulty? difficulty)
    {
        var query = _db.PresetQuestion.AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(pq => pq.Category == category);
        }

        if (difficulty.HasValue)
        {
            query = query.Where(pq => pq.Difficulty == difficulty.Value);
        }

        return await query.ToListAsync();
    }
}