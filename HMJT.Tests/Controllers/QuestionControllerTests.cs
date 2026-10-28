
using System.Security.Claims;
using HMJT.Controllers;
using HMJT.DAL;
using HMJT.Models;
using HMJT.ViewModels;
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

    // Creates a unique in-memory database for each test.
    private GameDbContext GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GameDbContext(options);
    }

    // Creates a valid Question entity for database tests.
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

    // Creates a game owned by the authenticated test user.
    private Game GetTestGame(int gameId)
    {
        return new Game
        {
            GameId = gameId,
            Name = "Test Game",
            OwnerUserId = TestUserId
        };
    }

    // Creates a valid ViewModel for Create and Edit POST tests.
    private QuestionViewModel GetTestViewModel(
        int questionId = 1,
        int gameId = 1)
    {
        return new QuestionViewModel
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
            GameId = gameId,
            GameName = "Test Game"
        };
    }

    // Creates a UserManager for the authenticated test user.
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

    // Creates a controller with real repositories using the in-memory database.
    private QuestionController GetController(GameDbContext context)
    {
        var controller = new QuestionController(
            new QuestionRepository(context),
            new GameRepository(context),
            GetUserManager()
        );

        // Simulates an authenticated ASP.NET Core Identity user.
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, TestUserId),
                new Claim(ClaimTypes.Name, "test@example.com")
            },
            "TestAuthentication"
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        return controller;
    }

    // Tests that Index returns the question index ViewModel.
    [Fact]
    public async Task Index_ReturnsQuestionIndexViewModel()
    {
        // Arrange
        using var context = GetDatabaseContext();

        context.Games.Add(GetTestGame(1));
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
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<QuestionIndexViewModel>(
            viewResult.Model
        );

        Assert.Equal(2, model.Questions.Count);
        Assert.All(
            model.Questions,
            question => Assert.Equal("Test Game", question.GameName)
        );
    }

    // Tests that Index populates the filter dropdown lists.
    [Fact]
    public async Task Index_PopulatesFilterOptions()
    {
        // Arrange
        using var context = GetDatabaseContext();
        context.Games.Add(GetTestGame(1));
        context.Questions.Add(GetTestQuestion(1, 1));
        await context.SaveChangesAsync();

        var controller = GetController(context);

        // Act
        var result = await controller.Index(
    null,
    null,
    null,
    null);
        

        // Assert
        var model = Assert.IsType<QuestionIndexViewModel>(
            Assert.IsType<ViewResult>(result).Model
        );

        Assert.NotEmpty(model.Categories);
        Assert.NotEmpty(model.Difficulties);
        Assert.NotNull(model.GameNames);
    }

    // Tests that Details returns a QuestionViewModel.
    [Fact]
    public async Task Details_ReturnsViewModel_WhenQuestionExists()
    {
        // Arrange
        using var context = GetDatabaseContext();
        context.Games.Add(GetTestGame(1));
        context.Questions.Add(GetTestQuestion(1, 1));
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
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<QuestionViewModel>(
            viewResult.Model
        );

        Assert.Equal(1, model.QuestionId);
        Assert.Equal(1, model.GameId);
        Assert.Equal("What is 2 + 2?", model.QuestionText);
        Assert.Equal("Test Game", model.GameName);
    }

    // Tests that Details returns NotFound for a missing question.
    [Fact]
    public async Task Details_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();
        var controller = GetController(context);

        // Act
        var result = await controller.Details(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // Tests that Create GET returns a populated QuestionViewModel.
    [Fact]
    public async Task CreateGet_ReturnsViewModel()
    {
        // Arrange
        using var context = GetDatabaseContext();
        context.Games.Add(GetTestGame(1));
        await context.SaveChangesAsync();

        var controller = GetController(context);

        // Act
        var result = await controller.Create(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<QuestionViewModel>(
            viewResult.Model
        );

        Assert.Equal(1, model.GameId);
        Assert.Equal("Test Game", model.GameName);
        Assert.NotEmpty(model.Categories);
        Assert.NotEmpty(model.Difficulties);
    }

    // Tests that Create POST saves a valid ViewModel as a Question entity.
    [Fact]
    public async Task CreatePost_SavesQuestion_WhenModelIsValid()
    {
        // Arrange
        using var context = GetDatabaseContext();
        context.Games.Add(GetTestGame(1));
        await context.SaveChangesAsync();

        var controller = GetController(context);
        var model = GetTestViewModel();

        // Act
        var result = await controller.Create(model);

        // Assert
        var savedQuestion = await context.Questions
            .SingleOrDefaultAsync();

        Assert.NotNull(savedQuestion);
        Assert.Equal("What is 2 + 2?", savedQuestion.QuestionText);
        Assert.Equal(1, savedQuestion.GameId);
        Assert.IsType<RedirectToActionResult>(result);
    }

    // Tests that Create POST returns NotFound if the game is missing.
    [Fact]
    public async Task CreatePost_ReturnsNotFound_WhenGameDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();
        var controller = GetController(context);
        var model = GetTestViewModel(gameId: 999);

        // Act
        var result = await controller.Create(model);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        Assert.Empty(context.Questions);
    }

    // Tests that invalid Create input returns the form with dropdowns repopulated.
    [Fact]
    public async Task CreatePost_ReturnsView_WhenModelStateIsInvalid()
    {
        // Arrange
        using var context = GetDatabaseContext();
        context.Games.Add(GetTestGame(1));
        await context.SaveChangesAsync();

        var controller = GetController(context);
        var model = GetTestViewModel();

        controller.ModelState.AddModelError(
            nameof(QuestionViewModel.QuestionText),
            "Question text is required."
        );

        // Act
        var result = await controller.Create(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsType<QuestionViewModel>(
            viewResult.Model
        );

        Assert.NotEmpty(returnedModel.Categories);
        Assert.NotEmpty(returnedModel.Difficulties);
        Assert.Empty(context.Questions);
    }

    // Tests that Edit GET returns the existing question as a ViewModel.
    [Fact]
    public async Task EditGet_ReturnsViewModel_WhenQuestionExists()
    {
        // Arrange
        using var context = GetDatabaseContext();
        context.Games.Add(GetTestGame(1));
        context.Questions.Add(GetTestQuestion(1, 1));
        await context.SaveChangesAsync();

        var controller = GetController(context);

        // Act
        var result = await controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<QuestionViewModel>(
            viewResult.Model
        );

        Assert.Equal(1, model.QuestionId);
        Assert.Equal("What is 2 + 2?", model.QuestionText);
        Assert.NotEmpty(model.Categories);
        Assert.NotEmpty(model.Difficulties);
    }

    // Tests that Edit GET returns NotFound for a missing question.
    [Fact]
    public async Task EditGet_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();
        var controller = GetController(context);

        // Act
        var result = await controller.Edit(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // Tests that Edit POST updates the stored question.
    [Fact]
    public async Task EditPost_UpdatesQuestion_WhenModelIsValid()
    {
        // Arrange
        using var context = GetDatabaseContext();
        context.Games.Add(GetTestGame(1));
        context.Questions.Add(GetTestQuestion(1, 1));
        await context.SaveChangesAsync();

        var controller = GetController(context);
        var model = GetTestViewModel();
        model.QuestionText = "What is 3 + 3?";

        // Act
        var result = await controller.Edit(model);

        // Assert
        var updatedQuestion = await context.Questions.FindAsync(1);

        Assert.NotNull(updatedQuestion);
        Assert.Equal("What is 3 + 3?", updatedQuestion.QuestionText);
        Assert.Equal(1, updatedQuestion.GameId);
        Assert.IsType<RedirectToActionResult>(result);
    }

    // Tests that Edit POST cannot edit a missing question.
    [Fact]
    public async Task EditPost_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();
        var controller = GetController(context);
        var model = GetTestViewModel(questionId: 999);

        // Act
        var result = await controller.Edit(model);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // Tests that invalid Edit input returns the form with dropdowns repopulated.
    [Fact]
    public async Task EditPost_ReturnsView_WhenModelStateIsInvalid()
    {
        // Arrange
        using var context = GetDatabaseContext();
        context.Games.Add(GetTestGame(1));
        context.Questions.Add(GetTestQuestion(1, 1));
        await context.SaveChangesAsync();

        var controller = GetController(context);
        var model = GetTestViewModel();
        model.QuestionText = "";

        controller.ModelState.AddModelError(
            nameof(QuestionViewModel.QuestionText),
            "Question text is required."
        );

        // Act
        var result = await controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsType<QuestionViewModel>(
            viewResult.Model
        );

        Assert.NotEmpty(returnedModel.Categories);
        Assert.NotEmpty(returnedModel.Difficulties);

        // The original question should remain unchanged.
        var savedQuestion = await context.Questions.FindAsync(1);
        Assert.Equal("What is 2 + 2?", savedQuestion!.QuestionText);
    }

    // Tests that Delete GET returns the question as a ViewModel.
    [Fact]
    public async Task DeleteGet_ReturnsViewModel_WhenQuestionExists()
    {
        // Arrange
        using var context = GetDatabaseContext();
        context.Games.Add(GetTestGame(1));
        context.Questions.Add(GetTestQuestion(1, 1));
        await context.SaveChangesAsync();

        var controller = GetController(context);

        // Act
        var result = await controller.Delete(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<QuestionViewModel>(
            viewResult.Model
        );

        Assert.Equal(1, model.QuestionId);
        Assert.Equal(1, model.GameId);
        Assert.Equal("What is 2 + 2?", model.QuestionText);
    }

    // Tests that Delete GET returns NotFound for a missing question.
    [Fact]
    public async Task DeleteGet_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();
        var controller = GetController(context);

        // Act
        var result = await controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // Tests that Delete POST removes the selected question.
    [Fact]
    public async Task DeleteConfirmed_RemovesQuestion()
    {
        // Arrange
        using var context = GetDatabaseContext();
        context.Games.Add(GetTestGame(1));
        context.Questions.Add(GetTestQuestion(1, 1));
        await context.SaveChangesAsync();

        var controller = GetController(context);

        // Act
        var result = await controller.DeleteConfirmed(1);

        // Assert
        Assert.Null(await context.Questions.FindAsync(1));
        Assert.IsType<RedirectToActionResult>(result);
    }

    // Tests that Delete POST returns NotFound for a missing question.
    [Fact]
    public async Task DeleteConfirmed_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();
        var controller = GetController(context);

        // Act
        var result = await controller.DeleteConfirmed(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // Minimal user store needed by UserManager in controller tests.
    private sealed class TestUserStore : IUserStore<ApplicationUser>
    {
        public void Dispose() { }

        public Task<string> GetUserIdAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
            => Task.FromResult(user.Id);

        public Task<string?> GetUserNameAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
            => Task.FromResult(user.UserName);

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
            => Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(
            ApplicationUser user,
            string? normalizedName,
            CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
            => Task.FromResult(IdentityResult.Success);

        public Task<IdentityResult> UpdateAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
            => Task.FromResult(IdentityResult.Success);

        public Task<IdentityResult> DeleteAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
            => Task.FromResult(IdentityResult.Success);

        public Task<ApplicationUser?> FindByIdAsync(
            string userId,
            CancellationToken cancellationToken)
            => Task.FromResult<ApplicationUser?>(null);

        public Task<ApplicationUser?> FindByNameAsync(
            string normalizedUserName,
            CancellationToken cancellationToken)
            => Task.FromResult<ApplicationUser?>(null);
    }
}
