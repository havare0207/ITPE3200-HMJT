using System.Linq;
using System;
using System.Threading.Tasks;
using HMJT.DAL;
using HMJT.Models;
using HMJT.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMJT.Controllers;

[Authorize]
public class NotesController : Controller
{
    // Repository used to retrieve and save notes.
    private readonly INotesRepository _notesRepository;

    // Inject the repository through dependency injection.
    public NotesController(INotesRepository notesRepository)
    {
        _notesRepository = notesRepository;
    }

    // READ - Display all notes.
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var notes = await _notesRepository.GetAll();

        // Convert database models into ViewModels.
        var vm = new NoteIndexViewModel
        {
            Notes = notes.Select(ToViewModel).ToList()
        };

        return View(vm);
    }

    // CREATE - Display the form.
    [HttpGet]
    public IActionResult Create()
    {
        return View(new NoteViewModel());
    }

    // CREATE - Save a new note.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NoteViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        // Map the ViewModel to the database model.
        var note = new Note
        {
            Title = vm.Title,
            Content = vm.Content,
            CreatedAt = DateTime.UtcNow
        };

        await _notesRepository.Create(note);

        return RedirectToAction(nameof(Index));
    }

    // UPDATE - Display the edit form.
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var note = await _notesRepository.GetNoteById(id);

        if (note == null)
        {
            return NotFound();
        }

        // Convert the database model into a ViewModel.
        var vm = ToViewModel(note);

        return View(vm);
    }

    // UPDATE - Save changes.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NoteViewModel vm)
    {
        // Ensure the route ID matches the submitted note ID.
        if (id != vm.NoteId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var existingNote = await _notesRepository.GetNoteById(id);

        if (existingNote == null)
        {
            return NotFound();
        }

        // Update only editable fields.
        existingNote.Title = vm.Title;
        existingNote.Content = vm.Content;

        // Preserve NoteId and the original CreatedAt value.
        await _notesRepository.Update(existingNote);

        return RedirectToAction(nameof(Index));
    }

    // DELETE - Display the confirmation page.
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _notesRepository.GetNoteById(id);

        if (note == null)
        {
            return NotFound();
        }

        var vm = ToViewModel(note);

        return View(vm);
    }

    // DELETE - Confirm deletion.
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var note = await _notesRepository.GetNoteById(id);

        if (note == null)
        {
            return NotFound();
        }

        await _notesRepository.Delete(id);

        return RedirectToAction(nameof(Index));
    }

    // Convert a database Note into a NoteViewModel.
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