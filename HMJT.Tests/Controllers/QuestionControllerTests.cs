using System.Security.Claims;
using HMJT.Controllers;
using HMJT.DAL;
using HMJT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace HMJT.Tests;

public class QuestionControllerTests
{
    // User ID used by the authenticated test user.
    private const string TestUserId = "test-user-1";

    // Creates a test database for each test.
    private GameDbContext GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GameDbContext(options);
    }

    // Creates a valid test question.
    private Question GetTestQuestion(int questionId, int gameId)
    {
        return new Question
        {
            QuestionId = questionId,
            QuestionText = "What is 2 + 2?",
            Category = "Math",
            Difficulty = Difficulty.Easy,
            AnswerA = "3",
            AnswerB = "4",
            AnswerC = "5",
            AnswerD = "6",
            CorrectAnswer = AnswerOption.B,
            GameId = gameId
        };
    }

    // Creates a test game owned by the
    // authenticated test user.
    private Game GetTestGame(int gameId)
    {
        return new Game
        {
            GameId = gameId,
            Name = "Test Game",
            OwnerUserId = TestUserId
        };
    }

    // Creates a UserManager that is sufficient
    // for reading the authenticated user's ID.
    private UserManager<ApplicationUser> GetUserManager()
    {
        return new UserManager<ApplicationUser>(
            new TestUserStore(),
            Options.Create(new IdentityOptions()),
            new PasswordHasher<ApplicationUser>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null!,
            NullLogger<UserManager<ApplicationUser>>.Instance
        );
    }

    // Creates the QuestionController with repositories,
    // a test UserManager and an authenticated test user.
    private QuestionController GetController(GameDbContext context)
    {
        var questionRepository =
            new QuestionRepository(context);

        var gameRepository =
            new GameRepository(context);

        var controller =
            new QuestionController(
                questionRepository,
                gameRepository,
                GetUserManager()
            );

        // Simulate an authenticated Identity user.
        var identity =
            new ClaimsIdentity(
                new[]
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        TestUserId
                    ),

                    new Claim(
                        ClaimTypes.Name,
                        "test@example.com"
                    )
                },
                "TestAuthentication"
            );

        controller.ControllerContext =
            new ControllerContext
            {
                HttpContext =
                    new DefaultHttpContext
                    {
                        User =
                            new ClaimsPrincipal(identity)
                    }
            };

        return controller;
    }

    // Tests that Index shows all questions.
    [Fact]
    public async Task Index_ReturnsAllQuestions()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var game = GetTestGame(1);

        context.Games.Add(game);

        context.Questions.AddRange(
            GetTestQuestion(1, 1),
            GetTestQuestion(2, 1)
        );

        await context.SaveChangesAsync();

        var controller = GetController(context);

        // Act
        var result = await controller.Index(
            null,
            null,
            null,
            null
        );

        // Assert
        var viewResult =
            Assert.IsType<ViewResult>(result);

        var questions =
            Assert.IsAssignableFrom<List<Question>>(
                viewResult.Model
            );

        Assert.Equal(2, questions.Count);
    }

    // Tests that Index loads the game belonging to each question.
    [Fact]
    public async Task Index_LoadsGameForQuestions()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var game = GetTestGame(1);

        context.Games.Add(game);
        context.Questions.Add(
            GetTestQuestion(1, 1)
        );

        await context.SaveChangesAsync();

        var controller = GetController(context);

        // Act
        var result = await controller.Index(
            null,
            null,
            null,
            null
        );

        // Assert
        var viewResult =
            Assert.IsType<ViewResult>(result);

        var questions =
            Assert.IsAssignableFrom<List<Question>>(
                viewResult.Model
            );

        Assert.NotNull(
            questions[0].Game
        );

        Assert.Equal(
            "Test Game",
            questions[0].Game!.Name
        );
    }

    // Tests that Details shows a question when it exists.
    [Fact]
    public async Task Details_ReturnsQuestion_WhenQuestionExists()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var game = GetTestGame(1);
        var question =
            GetTestQuestion(1, 1);

        context.Games.Add(game);
        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var controller = GetController(context);

        // Act
        var result =
            await controller.Details(1);

        // Assert
        var viewResult =
            Assert.IsType<ViewResult>(result);

        var model =
            Assert.IsType<Question>(
                viewResult.Model
            );

        Assert.Equal(
            1,
            model.QuestionId
        );

        Assert.Equal(
            1,
            model.GameId
        );
    }

    // Tests that Details returns NotFound when
    // the question does not exist.
    [Fact]
    public async Task Details_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var controller = GetController(context);

        // Act
        var result =
            await controller.Details(999);

        // Assert
        Assert.IsType<NotFoundResult>(
            result
        );
    }

    // Tests that Create adds a new question
    // to the correct game.
    [Fact]
    public async Task Create_AddsQuestionToGame()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var game = GetTestGame(1);

        context.Games.Add(game);

        await context.SaveChangesAsync();

        var controller = GetController(context);

        var question =
            GetTestQuestion(1, 1);

        // Act
        var result =
            await controller.Create(question);

        // Assert
        var savedQuestion =
            await context.Questions.FindAsync(1);

        Assert.NotNull(savedQuestion);

        Assert.Equal(
            1,
            savedQuestion.GameId
        );

        Assert.IsType<RedirectToActionResult>(
            result
        );
    }

    // Tests that Create returns NotFound
    // when the game does not exist.
    [Fact]
    public async Task Create_ReturnsNotFound_WhenGameDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var controller = GetController(context);

        var question =
            GetTestQuestion(1, 999);

        // Act
        var result =
            await controller.Create(question);

        // Assert
        Assert.IsType<NotFoundResult>(
            result
        );
    }

    // Tests that Edit updates an existing question.
    [Fact]
    public async Task Edit_UpdatesQuestion()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var game = GetTestGame(1);
        var question =
            GetTestQuestion(1, 1);

        context.Games.Add(game);
        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var controller = GetController(context);

        question.QuestionText =
            "What is 3 + 3?";

        // Act
        var result =
            await controller.Edit(question);

        // Assert
        var updatedQuestion =
            await context.Questions.FindAsync(1);

        Assert.NotNull(updatedQuestion);

        Assert.Equal(
            "What is 3 + 3?",
            updatedQuestion.QuestionText
        );

        Assert.Equal(
            1,
            updatedQuestion.GameId
        );

        Assert.IsType<RedirectToActionResult>(
            result
        );
    }

    // Tests that Edit returns NotFound
    // when the question does not exist.
    [Fact]
    public async Task Edit_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var controller = GetController(context);

        // Act
        var result =
            await controller.Edit(999);

        // Assert
        Assert.IsType<NotFoundResult>(
            result
        );
    }

    // Tests that Delete shows a question when it exists.
    [Fact]
    public async Task Delete_ReturnsQuestion_WhenQuestionExists()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var game = GetTestGame(1);
        var question =
            GetTestQuestion(1, 1);

        context.Games.Add(game);
        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var controller = GetController(context);

        // Act
        var result =
            await controller.Delete(1);

        // Assert
        var viewResult =
            Assert.IsType<ViewResult>(result);

        var model =
            Assert.IsType<Question>(
                viewResult.Model
            );

        Assert.Equal(
            1,
            model.QuestionId
        );

        Assert.Equal(
            1,
            model.GameId
        );
    }

    // Tests that Delete returns NotFound when
    // the question does not exist.
    [Fact]
    public async Task Delete_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var controller = GetController(context);

        // Act
        var result =
            await controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(
            result
        );
    }

    // Tests that DeleteConfirmed removes a question.
    [Fact]
    public async Task DeleteConfirmed_RemovesQuestion()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var game = GetTestGame(1);
        var question =
            GetTestQuestion(1, 1);

        context.Games.Add(game);
        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var controller = GetController(context);

        // Act
        var result =
            await controller.DeleteConfirmed(1);

        // Assert
        var deletedQuestion =
            await context.Questions.FindAsync(1);

        Assert.Null(deletedQuestion);

        Assert.IsType<RedirectToActionResult>(
            result
        );
    }

    // Tests that DeleteConfirmed returns NotFound
    // when the question does not exist.
    [Fact]
    public async Task DeleteConfirmed_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var controller = GetController(context);

        // Act
        var result =
            await controller.DeleteConfirmed(999);

        // Assert
        Assert.IsType<NotFoundResult>(
            result
        );
    }

    // Minimal user store required to create
    // a UserManager for controller tests.
    private sealed class TestUserStore :
        IUserStore<ApplicationUser>
    {
        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                user.Id
            );
        }

        public Task<string?> GetUserNameAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                user.UserName
            );
        }

        public Task SetUserNameAsync(
            ApplicationUser user,
            string? userName,
            CancellationToken cancellationToken)
        {
            user.UserName = userName;

            return Task.CompletedTask;
        }

        public Task<string?> GetNormalizedUserNameAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                user.NormalizedUserName
            );
        }

        public Task SetNormalizedUserNameAsync(
            ApplicationUser user,
            string? normalizedName,
            CancellationToken cancellationToken)
        {
            user.NormalizedUserName =
                normalizedName;

            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                IdentityResult.Success
            );
        }

        public Task<IdentityResult> UpdateAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                IdentityResult.Success
            );
        }

        public Task<IdentityResult> DeleteAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                IdentityResult.Success
            );
        }

        public Task<ApplicationUser?> FindByIdAsync(
            string userId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<ApplicationUser?>(
                null
            );
        }

        public Task<ApplicationUser?> FindByNameAsync(
            string normalizedUserName,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<ApplicationUser?>(
                null
            );
        }
    }
}
