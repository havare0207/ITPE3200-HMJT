using HMJT.Models;
using Microsoft.EntityFrameworkCore;
namespace HMJT.DAL;

public class QuestionRepository : IQuestionRepository
{
    private readonly GameDbContext _db;

    public QuestionRepository(GameDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Question>> GetAll()
    {
        return await _db.Questions.Include(q => q.Game).ToListAsync();
    }

    public async Task<Question?> GetQuestionById(int id)
    {
        return await _db.Questions.FindAsync(id);
    }

    public async Task Create(Question question)
    {
        _db.Questions.Add(question);
        await _db.SaveChangesAsync();
    }

    public async Task Update(Question question)
    {
        _db.Questions.Update(question);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var question = await _db.Questions.FindAsync(id);
        if (question == null)
        {
            return false;
        }

        _db.Questions.Remove(question);
        await _db.SaveChangesAsync();
        return true;
    }
}