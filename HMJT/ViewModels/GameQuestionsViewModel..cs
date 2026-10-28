using System.Collections.Generic;

namespace HMJT.ViewModels;

// Represents a game and its questions on the Manage Questions page.
public class GameQuestionsViewModel
{
    // Identifies the game used by the Add Question link.
    public int GameId { get; set; }

    // Displays the game name in the page heading.
    public string GameName { get; set; } = string.Empty;

    // Contains the questions associated with this game.
    // Reuses the existing QuestionViewModel.
    public List<QuestionViewModel> Questions { get; set; } = new();
}