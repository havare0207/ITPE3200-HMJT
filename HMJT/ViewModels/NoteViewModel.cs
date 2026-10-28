using System;
using System.ComponentModel.DataAnnotations;

namespace HMJT.ViewModels;

// Represents the data used by the individual note views.
public class NoteViewModel
{
    // Identifies the note.
    public int NoteId { get; set; }

    // The title is required and cannot exceed 100 characters.
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    // The content is required.
    [Required]
    public string Content { get; set; } = string.Empty;

    // The date the note was originally created.
    public DateTime CreatedAt { get; set; }
}