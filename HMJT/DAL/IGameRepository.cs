using HMJT.Models;

namespace HMJT.DAL;

public interface IGameRepository
{
    Task<IEnumerable<Game>> GetAll();
    Task<Game?> GetGameById(int id);
    Task Create(Game game);
    Task Update(Game game);
    Task<bool> Delete(int id);
    Task SetPresetQuestion(int gameId, IEnumerable<int> presetQuestionIds);
}