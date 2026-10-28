using HMJT.Models;
namespace HMJT.DAL;

public interface IQuestionRepository
{
    Task<IEnumerable<Question>> GetAll();
    Task<Question?> GetQuestionById(int id);
    Task Create(Question question);
    Task Update(Question question);
    Task<bool> Delete(int id);
}