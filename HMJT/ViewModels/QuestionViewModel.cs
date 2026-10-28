
using System.ComponentModel.DataAnnotations;
using HMJT.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMJT.ViewModels;

public class QuestionViewModel
{
    public int QuestionId { get; set; }

    [Required(ErrorMessage = "Question text is required.")]
    [StringLength(500, ErrorMessage = "Question text cannot exceed 500 characters.")]
    public string QuestionText { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required.")]
    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
    public string Category { get; set; } = string.Empty;

    public Difficulty Difficulty { get; set; }

    [Required(ErrorMessage = "Answer A is required.")]
    [StringLength(300, ErrorMessage = "Answer A cannot exceed 300 characters.")]
    public string AnswerA { get; set; } = string.Empty;

    [Required(ErrorMessage = "Answer B is required.")]
    [StringLength(300, ErrorMessage = "Answer B cannot exceed 300 characters.")]
    public string AnswerB { get; set; } = string.Empty;

    [Required(ErrorMessage = "Answer C is required.")]
    [StringLength(300, ErrorMessage = "Answer C cannot exceed 300 characters.")]
    public string AnswerC { get; set; } = string.Empty;

    [Required(ErrorMessage = "Answer D is required.")]
    [StringLength(300, ErrorMessage = "Answer D cannot exceed 300 characters.")]
    public string AnswerD { get; set; } = string.Empty;

    public AnswerOption CorrectAnswer { get; set; }

    public int GameId { get; set; }

    // Display information; this is not a database relationship.
    public string? GameName { get; set; }

    // Dropdown options for the Create and Edit forms.
    public List<SelectListItem> Categories { get; set; } = new();

    public List<SelectListItem> Difficulties { get; set; } = new();
}
