using System.Collections.Generic;

namespace HMJT.ViewModels;

// Represents the data required by the My Games index page.
public class GameIndexViewModel
{
    // Contains the games owned by the currently logged-in user.
    public List<GameViewModel> Games { get; set; } = new();
}