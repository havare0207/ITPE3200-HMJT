using System.ComponentModel.DataAnnotations;

namespace HMJT.Models;

public class Note
{
    public int NoteId { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}