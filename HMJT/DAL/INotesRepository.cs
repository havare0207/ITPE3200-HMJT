using HMJT.Models;
namespace HMJT.DAL;

public interface INotesRepository
{
    Task<IEnumerable<Note>> GetAll();
    Task<Note?> GetNoteById(int id);
    Task Create(Note note);
    Task Update(Note note);
    Task<bool> Delete(int id);
}