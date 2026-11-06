using HMJT.Models;
namespace HMJT.DAL;

public interface IPresetQuestionRepository
{
    Task<IEnumerable<PresetQuestion>> GetAll();
    Task<PresetQuestion?> GetPresetQuestionById(int id);
    Task<IEnumerable<PresetQuestion>> GetByIds(IEnumerable<int> ids);
    Task<IEnumerable<PresetQuestion>> Search(string? category, Difficulty? difficulty);
}