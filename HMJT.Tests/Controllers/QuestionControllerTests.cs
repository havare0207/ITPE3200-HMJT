using HMJT.Controllers;
using HMJT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMJT.Tests;

public class QuestionControllerTests
{
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

    // Creates a test game.
    private Game GetTestGame(int gameId)
    {
        return new Game
        {
            GameId = gameId,
            Name = "Test Game"
        };
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

        var controller = new QuestionController(context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var questions = Assert.IsAssignableFrom<List<Question>>(viewResult.Model);

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
        context.Questions.Add(GetTestQuestion(1, 1));

        await context.SaveChangesAsync();

        var controller = new QuestionController(context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var questions = Assert.IsAssignableFrom<List<Question>>(viewResult.Model);

        Assert.NotNull(questions[0].Game);
        Assert.Equal("Test Game", questions[0].Game!.Name);
    }

    // Tests that Details shows a question when it exists.
    [Fact]
    public async Task Details_ReturnsQuestion_WhenQuestionExists()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var game = GetTestGame(1);
        var question = GetTestQuestion(1, 1);

        context.Games.Add(game);
        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var controller = new QuestionController(context);

        // Act
        var result = await controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Question>(viewResult.Model);

        Assert.Equal(1, model.QuestionId);
        Assert.Equal(1, model.GameId);
    }

    // Tests that Details returns NotFound when the question does not exist.
    [Fact]
    public async Task Details_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var controller = new QuestionController(context);

        // Act
        var result = await controller.Details(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // Tests that Create adds a new question to the correct game.
    [Fact]
    public async Task Create_AddsQuestionToGame()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var game = GetTestGame(1);
        context.Games.Add(game);

        await context.SaveChangesAsync();

        var controller = new QuestionController(context);

        var question = GetTestQuestion(1, 1);

        // Act
        var result = await controller.Create(question);

        // Assert
        var savedQuestion = await context.Questions.FindAsync(1);

        Assert.NotNull(savedQuestion);
        Assert.Equal(1, savedQuestion.GameId);
        Assert.IsType<RedirectToActionResult>(result);
    }

    // Tests that Create returns NotFound when the game does not exist.
    [Fact]
    public async Task Create_ReturnsNotFound_WhenGameDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var controller = new QuestionController(context);

        var question = GetTestQuestion(1, 999);

        // Act
        var result = await controller.Create(question);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // Tests that Edit updates an existing question.
    [Fact]
    public async Task Edit_UpdatesQuestion()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var game = GetTestGame(1);
        var question = GetTestQuestion(1, 1);

        context.Games.Add(game);
        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var controller = new QuestionController(context);

        question.QuestionText = "What is 3 + 3?";

        // Act
        var result = await controller.Edit(question);

        // Assert
        var updatedQuestion = await context.Questions.FindAsync(1);

        Assert.NotNull(updatedQuestion);
        Assert.Equal("What is 3 + 3?", updatedQuestion.QuestionText);
        Assert.Equal(1, updatedQuestion.GameId);
        Assert.IsType<RedirectToActionResult>(result);
    }

    // Tests that Edit returns NotFound when the question does not exist.
    [Fact]
    public async Task Edit_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var controller = new QuestionController(context);

        // Act
        var result = await controller.Edit(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // Tests that Delete shows a question when it exists.
    [Fact]
    public async Task Delete_ReturnsQuestion_WhenQuestionExists()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var game = GetTestGame(1);
        var question = GetTestQuestion(1, 1);

        context.Games.Add(game);
        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var controller = new QuestionController(context);

        // Act
        var result = await controller.Delete(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Question>(viewResult.Model);

        Assert.Equal(1, model.QuestionId);
        Assert.Equal(1, model.GameId);
    }

    // Tests that Delete returns NotFound when the question does not exist.
    [Fact]
    public async Task Delete_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var controller = new QuestionController(context);

        // Act
        var result = await controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // Tests that DeleteConfirmed removes a question.
    [Fact]
    public async Task DeleteConfirmed_RemovesQuestion()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var game = GetTestGame(1);
        var question = GetTestQuestion(1, 1);

        context.Games.Add(game);
        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var controller = new QuestionController(context);

        // Act
        var result = await controller.DeleteConfirmed(1);

        // Assert
        var deletedQuestion = await context.Questions.FindAsync(1);

        Assert.Null(deletedQuestion);
        Assert.IsType<RedirectToActionResult>(result);
    }

    // Tests that DeleteConfirmed returns NotFound when the question does not exist.
    [Fact]
    public async Task DeleteConfirmed_ReturnsNotFound_WhenQuestionDoesNotExist()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var controller = new QuestionController(context);

        // Act
        var result = await controller.DeleteConfirmed(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}