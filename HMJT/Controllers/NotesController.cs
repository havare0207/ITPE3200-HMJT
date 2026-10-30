using HMJT.DAL;
using HMJT.Models;
using HMJT.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace HMJT.Controllers;

// Only authenticated users can view and manage notes.
[Authorize]
public class NotesController : Controller
{
    // Gives the controller access to notes stored in the database.
    private readonly INotesRepository _notesRepository;

    // Used to write messages to the application log.
    private readonly ILogger<NotesController> _logger;

    // Gets the repository and logger through dependency injection.
    public NotesController(
        INotesRepository notesRepository,
        ILogger<NotesController>? logger = null)
    {
        _notesRepository = notesRepository;
        _logger = logger ?? NullLogger<NotesController>.Instance;
    }

    // READ - Display all notes, newest first
    public async Task<IActionResult> Index()
    {
        try
        {
            var notes = await _notesRepository.GetAll();

            var viewModel = new NoteIndexViewModel
            {
                Notes = notes
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(ToViewModel)
                    .ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not load notes");
            TempData["ErrorMessage"] = "Could not load your notes. Please try again.";
            return View(new NoteIndexViewModel { Notes = new List<NoteViewModel>() });
        }
    }

    // CREATE - Display the create form
    [HttpGet]
    public IActionResult Create()
    {
        return View(new NoteViewModel());
    }

    // CREATE - Save a new note
    // Bind only accepts Title and Content, so the browser cannot
    // set NoteId or CreatedAt (over-posting protection).
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Content")] NoteViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Creating a note failed validation");
            return View(viewModel);
        }

        var note = new Note
        {
            Title = viewModel.Title,
            Content = viewModel.Content,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _notesRepository.Create(note);
            _logger.LogInformation("Note {NoteId} was created", note.NoteId);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not create note");
            ModelState.AddModelError(string.Empty,
                "Could not save the note. Please try again.");
            return View(viewModel);
        }
    }

    // UPDATE - Display the edit form
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var note = await _notesRepository.GetNoteById(id);

            if (note == null)
            {
                _logger.LogWarning("Note {NoteId} was not found (Edit)", id);
                return NotFound();
            }

            return View(ToViewModel(note));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not load note {NoteId} for editing", id);
            TempData["ErrorMessage"] = "Could not load the note. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }

    // UPDATE - Save changes
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NoteViewModel viewModel)
    {
        if (id != viewModel.NoteId)
        {
            _logger.LogWarning(
                "Edit id {Id} does not match note id {NoteId}", id, viewModel.NoteId);
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Editing note {NoteId} failed validation", id);
            return View(viewModel);
        }

        try
        {
            var existingNote = await _notesRepository.GetNoteById(id);

            if (existingNote == null)
            {
                _logger.LogWarning("Note {NoteId} was not found (Edit)", id);
                return NotFound();
            }

            // Only Title and Content are changed. CreatedAt is kept.
            existingNote.Title = viewModel.Title;
            existingNote.Content = viewModel.Content;

            await _notesRepository.Update(existingNote);
            _logger.LogInformation("Note {NoteId} was updated", id);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not update note {NoteId}", id);
            ModelState.AddModelError(string.Empty,
                "Could not save the changes. Please try again.");
            return View(viewModel);
        }
    }

    // DELETE - Display delete confirmation
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var note = await _notesRepository.GetNoteById(id);

            if (note == null)
            {
                _logger.LogWarning("Note {NoteId} was not found (Delete)", id);
                return NotFound();
            }

            return View(ToViewModel(note));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not load note {NoteId} for deletion", id);
            TempData["ErrorMessage"] = "Could not load the note. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }

    // DELETE - Confirm deletion
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int noteId)
    {
        try
        {
            var deleted = await _notesRepository.Delete(noteId);

            if (!deleted)
            {
                _logger.LogWarning("Note {NoteId} was not found (DeleteConfirmed)", noteId);
                return NotFound();
            }

            _logger.LogInformation("Note {NoteId} was deleted", noteId);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not delete note {NoteId}", noteId);
            TempData["ErrorMessage"] = "Could not delete the note. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }

    // Converts a Note entity to a NoteViewModel for the views.
    private static NoteViewModel ToViewModel(Note note)
    {
        return new NoteViewModel
        {
            NoteId = note.NoteId,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt
        };
    }
}