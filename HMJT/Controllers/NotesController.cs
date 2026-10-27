using HMJT.DAL;
using HMJT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMJT.Controllers;

[Authorize]
public class NotesController : Controller
{
    private readonly INotesRepository _notesRepository;

    public NotesController(INotesRepository notesRepository)
    {
        _notesRepository = notesRepository;
    }

    // READ - Display all notes
    public async Task<IActionResult> Index()
    {
        var notes = await _notesRepository.GetAll();
        return View(notes);
    }

    // CREATE - Display the create form
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // CREATE - Save a new note
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Note note)
    {
        if (ModelState.IsValid)
        {
            await _notesRepository.Create(note);
            return RedirectToAction(nameof(Index));
        }

        return View(note);
    }

    // UPDATE - Display the edit form
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var note = await _notesRepository.GetNoteById(id);

        if (note == null)
        {
            return NotFound();
        }

        return View(note);
    }

    // UPDATE - Save changes
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Note note)
    {
        if (id != note.NoteId)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            var existingNote = await _notesRepository.GetNoteById(id);

            if (existingNote == null)
            {
                return NotFound();
            }

            existingNote.Title = note.Title;
            existingNote.Content = note.Content;

            await _notesRepository.Update(existingNote);

            return RedirectToAction(nameof(Index));
        }

        return View(note);
    }

    // DELETE - Display delete confirmation
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _notesRepository.GetNoteById(id);

        if (note == null)
        {
            return NotFound();
        }

        return View(note);
    }

    // DELETE - Confirm deletion
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _notesRepository.Delete(id);

        return RedirectToAction(nameof(Index));
    }
}