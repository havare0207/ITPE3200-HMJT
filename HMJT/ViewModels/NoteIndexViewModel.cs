using System.Collections.Generic;

namespace HMJT.ViewModels;

// Represents the data displayed on the Notes Index page.
public class NoteIndexViewModel
{
    // The collection of notes displayed in the table.
    public List<NoteViewModel> Notes { get; set; } = new();
}