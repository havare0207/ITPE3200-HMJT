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


    // Tests that Index shows all questions.
    [Fact]
    public async Task Index_ReturnsAllQuestions()
    {
        // Arrange
        using var context = GetDatabaseContext();

        context.Questions.AddRange(
            new Question
            {
                QuestionId = 1
                // Add required Question properties here.
            },
            new Question
            {
                QuestionId = 2
                // Add required Question properties here.
            }
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


    // Tests that Details shows a question when it exists.
    [Fact]
    public async Task Details_ReturnsQuestion_WhenQuestionExists()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var question = new Question
        {
            QuestionId = 1
            // Add required Question properties here.
        };

        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var controller = new QuestionController(context);

        // Act
        var result = await controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Question>(viewResult.Model);

        Assert.Equal(1, model.QuestionId);
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


    // Tests that Create adds a new question.
    [Fact]
    public async Task Create_AddsQuestion()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var controller = new QuestionController(context);

        var question = new Question
        {
            QuestionId = 1
            // Add required Question properties here.
        };

        // Act
        var result = await controller.Create(question);

        // Assert
        var savedQuestion = await context.Questions.FindAsync(1);

        Assert.NotNull(savedQuestion);
        Assert.IsType<RedirectToActionResult>(result);
    }


    // Tests that Edit updates an existing question.
    [Fact]
    public async Task Edit_UpdatesQuestion()
    {
        // Arrange
        using var context = GetDatabaseContext();

        var question = new Question
        {
            QuestionId = 1
            // Add required Question properties here.
        };

        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var controller = new QuestionController(context);

        // Act
        var result = await controller.Edit(question);

        // Assert
        var updatedQuestion = await context.Questions.FindAsync(1);

        Assert.NotNull(updatedQuestion);
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

        var question = new Question
        {
            QuestionId = 1
            // Add required Question properties here.
        };

        context.Questions.Add(question);

        await context.SaveChangesAsync();

        var controller = new QuestionController(context);

        // Act
        var result = await controller.Delete(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Question>(viewResult.Model);

        Assert.Equal(1, model.QuestionId);
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

        var question = new Question
        {
            QuestionId = 1
            // Add required Question properties here.
        };

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