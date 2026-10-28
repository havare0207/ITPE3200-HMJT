
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMJT.ViewModels;

public class QuestionIndexViewModel
{
    public List<QuestionViewModel> Questions { get; set; } = new();

    // Current search and filter values.
    public string? SearchString { get; set; }

    public string? Category { get; set; }

    public string? Difficulty { get; set; }

    public string? GameName { get; set; }

    // Options displayed in the filter dropdowns.
    public List<SelectListItem> Categories { get; set; } = new();

    public List<SelectListItem> Difficulties { get; set; } = new();

    public List<SelectListItem> GameNames { get; set; } = new();
}
