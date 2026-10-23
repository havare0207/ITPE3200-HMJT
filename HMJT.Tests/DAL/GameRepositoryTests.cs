using HMJT.DAL;
using HMJT.Models;
using Microsoft.EntityFrameworkCore;

namespace HMJT.Tests.DAL;

public class GameRepositoryTests
{
    // Tests that a user cannot access
    // a game owned by another user.
    [Fact]
    public async Task GetByIdForUser_ShouldNotReturnAnotherUsersGame()
    {
        // Arrange
        var options =
            new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString()
                )
                .Options;

        await using var db =
            new GameDbContext(options);

        var game =
            new Game
            {
                GameId = 1,
                Name = "Alice's Game",
                OwnerUserId = "user-alice"
            };

        db.Games.Add(game);
        await db.SaveChangesAsync();

        var repository =
            new GameRepository(db);

        // Act
        Game? result =
            await repository.GetByIdForUser(
                1,
                "user-bob"
            );

        // Assert
        Assert.Null(result);
    }
}