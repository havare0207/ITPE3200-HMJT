using HMJT.Models;

namespace HMJT.DAL;

public interface IGameRepository
{
    // Returns all custom games belonging
    // to one specific logged-in user.
    Task<IEnumerable<Game>> GetAllForUser(
        string userId);

    // Returns one custom game only if
    // it belongs to the specified user.
    Task<Game?> GetByIdForUser(
        int id,
        string userId);

    // Creates a new custom game.
    Task Create(
        Game game);

    // Updates an existing custom game.
    Task Update(
        Game game);

    // Deletes a custom game only if
    // it belongs to the specified user.
    Task<bool> Delete(
        int id,
        string userId);
}