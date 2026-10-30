using HMJT.Controllers;
using HMJT.DAL;
using HMJT.Models;
using HMJT.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace HMJT.Tests;

public class NotesControllerTests
{
    // Creates a fresh in-memory database for each test.
    private GameDbContext GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GameDbContext(options);
    }

    // Creates a controller with TempData set up,
    // because the controller writes error messages to TempData.
    private NotesController GetController(INotesRepository repository)
    {
        var controller = new NotesController(repository);
        controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(), new FakeTempDataProvider());
        return controller;
    }

    private NotesController GetController(GameDbContext context)
    {
        return GetController(new NotesRepository(context));
    }

    // Stores nothing. Only needed so TempData can be used in tests.
    private class FakeTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context)
            => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }

    // A repository that always fails, used to test error handling.
    private class FailingNotesRepository : INotesRepository
    {
        public Task<IEnumerable<Note>> GetAll() => throw new InvalidOperationException("DB error");
        public Task<Note?> GetNoteById(int id) => throw new InvalidOperationException("DB error");
        public Task Create(Note note) => throw new InvalidOperationException("DB error");
        public Task Update(Note note) => throw new InvalidOperationException("DB error");
        public Task<bool> Delete(int id) => throw new InvalidOperationException("DB error");
    }

    // ---------- Positive tests ----------

    [Fact]
    public async Task Index_ReturnsAllNotes()
    {
        var context = GetDatabaseContext();
        context.Notes.Add(new Note { Title = "A", Content = "a" });
        context.Notes.Add(new Note { Title = "B", Content = "b" });
        await context.SaveChangesAsync();

        var result = await GetController(context).Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<NoteIndexViewModel>(view.Model);
        Assert.Equal(2, model.Notes.Count());
    }

    [Fact]
    public async Task Index_ReturnsNewestNoteFirst()
    {
        var context = GetDatabaseContext();
        context.Notes.Add(new Note { Title = "Old", Content = "a", CreatedAt = new DateTime(2026, 1, 1) });
        context.Notes.Add(new Note { Title = "New", Content = "b", CreatedAt = new DateTime(2026, 6, 1) });
        await context.SaveChangesAsync();

        var result = await GetController(context).Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<NoteIndexViewModel>(view.Model);
        Assert.Equal("New", model.Notes.First().Title);
    }

    [Fact]
    public void Create_Get_ReturnsView()
    {
        var result = GetController(GetDatabaseContext()).Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_SavesNote_AndRedirectsToIndex()
    {
        var context = GetDatabaseContext();

        var result = await GetController(context)
            .Create(new NoteViewModel { Title = "Title", Content = "Content" });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Single(context.Notes);
    }

    [Fact]
    public async Task Edit_Get_ReturnsView_WhenNoteExists()
    {
        var context = GetDatabaseContext();
        context.Notes.Add(new Note { NoteId = 1, Title = "T", Content = "C" });
        await context.SaveChangesAsync();

        var result = await GetController(context).Edit(1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(1, Assert.IsType<NoteViewModel>(view.Model).NoteId);
    }

    [Fact]
    public async Task Edit_Post_UpdatesTitleAndContent_AndKeepsCreatedAt()
    {
        var context = GetDatabaseContext();
        var created = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        context.Notes.Add(new Note { NoteId = 1, Title = "Old", Content = "Old", CreatedAt = created });
        await context.SaveChangesAsync();

        var result = await GetController(context)
            .Edit(1, new NoteViewModel { NoteId = 1, Title = "New", Content = "New content" });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        var saved = await context.Notes.FindAsync(1);
        Assert.Equal("New", saved!.Title);
        Assert.Equal("New content", saved.Content);
        Assert.Equal(created, saved.CreatedAt);
    }

    [Fact]
    public async Task Delete_Get_ReturnsView_WhenNoteExists()
    {
        var context = GetDatabaseContext();
        context.Notes.Add(new Note { NoteId = 1, Title = "T", Content = "C" });
        await context.SaveChangesAsync();

        var result = await GetController(context).Delete(1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(1, Assert.IsType<NoteViewModel>(view.Model).NoteId);
    }

    [Fact]
    public async Task DeleteConfirmed_RemovesNote_AndRedirectsToIndex()
    {
        var context = GetDatabaseContext();
        context.Notes.Add(new Note { NoteId = 1, Title = "T", Content = "C" });
        await context.SaveChangesAsync();

        var result = await GetController(context).DeleteConfirmed(1);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Empty(context.Notes);
    }

    // ---------- Negative tests ----------

    [Fact]
    public async Task Create_Post_ReturnsView_WhenModelStateIsInvalid()
    {
        var context = GetDatabaseContext();
        var controller = GetController(context);
        controller.ModelState.AddModelError("Title", "Required");

        var result = await controller.Create(new NoteViewModel { Content = "Content" });

        Assert.IsType<ViewResult>(result);
        Assert.Empty(context.Notes);
    }

    [Fact]
    public async Task Edit_Get_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        var result = await GetController(GetDatabaseContext()).Edit(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ReturnsBadRequest_WhenIdDoesNotMatchNote()
    {
        var result = await GetController(GetDatabaseContext())
            .Edit(2, new NoteViewModel { NoteId = 1, Title = "T", Content = "C" });

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        var result = await GetController(GetDatabaseContext())
            .Edit(999, new NoteViewModel { NoteId = 999, Title = "T", Content = "C" });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ReturnsView_WhenModelStateIsInvalid()
    {
        var controller = GetController(GetDatabaseContext());
        controller.ModelState.AddModelError("Title", "Required");

        var result = await controller.Edit(1, new NoteViewModel { NoteId = 1, Content = "C" });

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Delete_Get_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        var result = await GetController(GetDatabaseContext()).Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        var result = await GetController(GetDatabaseContext()).DeleteConfirmed(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Index_ReturnsEmptyListAndErrorMessage_WhenDatabaseFails()
    {
        var controller = GetController(new FailingNotesRepository());

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<NoteIndexViewModel>(view.Model);
        Assert.Empty(model.Notes);
        Assert.NotNull(controller.TempData["ErrorMessage"]);
    }

    [Fact]
    public async Task Create_Post_ReturnsViewWithError_WhenSaveFails()
    {
        var controller = GetController(new FailingNotesRepository());

        var result = await controller.Create(new NoteViewModel { Title = "T", Content = "C" });

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task DeleteConfirmed_RedirectsWithErrorMessage_WhenDeleteFails()
    {
        var controller = GetController(new FailingNotesRepository());

        var result = await controller.DeleteConfirmed(1);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.NotNull(controller.TempData["ErrorMessage"]);
    }
}
