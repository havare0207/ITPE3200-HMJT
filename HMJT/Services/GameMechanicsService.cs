//Needs HMJT.Models for logic from Question and AnswerOption

using HMJT.Models;

// Organizes this class inside the Services namespace.
// The full class name is HMJT.Services.GameMechanicsService.
//See DiceService.cs for more info about namespace.

namespace HMJT.Services;

public class GameMechanicsService
{
    //makes a variable GetCategoryFromDiceRoll of type string
    //which takes in a number (int diceRoll) and returns category of type string.
    public string GetCategoryFromDiceRoll(int diceRoll)
    {
        string category;

        //if statement to deside which category to return based on diceRoll:

        if (diceRoll == 1)
        {
            category = "Java";
        }
        else if (diceRoll == 2)
        {
            category = "JavaScript";
        }
        else if (diceRoll == 3)
        {
            category = "HTML/CSS";
        }
        else if (diceRoll == 4)
        {
            category = "Python";
        }
        else if (diceRoll == 5)
        {
            category = "Game History";
        }
        else if (diceRoll == 6)
        {
            category = "C#";
        }

        //if the diceRoll number is not between 1 and 6, 
        // throw a ArgumentOutOfRangeExeption.

        else
        {
            throw new ArgumentOutOfRangeException(
                nameof(diceRoll),
                "Dice roll must be between 1 and 6."
            );
        }

        return category;
    }

    // Checks whether the answer selected by the player
    // matches the correct answer for the question.
    public bool IsAnswerCorrect(
        Question question,
        AnswerOption selectedAnswer)
    {
        return question.CorrectAnswer == selectedAnswer;
    }

    // Creates a GameAnswer containing information
    // about the player's answer to a question.
    public GameAnswer CreateGameAnswer(
        Question question,
        string playerName,
        AnswerOption selectedAnswer)
    {
        // Check whether the selected answer is correct.
        bool isCorrect = IsAnswerCorrect(question, selectedAnswer);

        // Create and return a GameAnswer containing
        // the result of the player's answer.
        return new GameAnswer
        {
            QuestionId = question.QuestionId,
            PlayerName = playerName,
            SelectedAnswer = selectedAnswer,
            IsCorrect = isCorrect
        };
    }

    // Awards a category wedge to the player if the answer was correct.
    // Returns true if a new wedge was added.
    // Returns false if the answer was wrong or the player already has the wedge.
    public bool AwardWedge(
        PlayerGameState player,
        Question question,
        GameAnswer gameAnswer)
    {
        // A player only receives a wedge for a correct answer.
        if (!gameAnswer.IsCorrect)
        {
            return false;
        }

        // HashSet.Add returns true if the category was added,
        // and false if the player already had that category.
        return player.Wedges.Add(question.Category);
    }

    // Checks whether the player already owns
    // the wedge for the given category.
    public bool HasWedge(PlayerGameState player, string category)
    {
        return player.Wedges.Contains(category);
    }
    
    // Contains all category wedges required to complete the game.
    private readonly HashSet<string> _requiredWedges = new()
    {
        "Java",
        "JavaScript",
        "HTML/CSS",
        "Python",
        "Game History",
        "C#"
    };

    // Checks whether the player has collected
    // every required category wedge.
    public bool HasAllWedges(PlayerGameState player)
    {
        return _requiredWedges.IsSubsetOf(player.Wedges);
    }

    // Returns the category wedges that the player
    // has not collected yet.
    public List<string> GetMissingWedges(PlayerGameState player)
    {
        return _requiredWedges
            .Where(category => !player.Wedges.Contains(category))
            .ToList();
    }

    // Selects a random starting player.
    // This method should only be called once when the game begins.
    public void SelectRandomStartingPlayer(GameSessionState gameState)
    {
        // A game cannot start without players.
        if (gameState.Players.Count == 0)
        {
            throw new InvalidOperationException(
                "The game must contain at least one player."
            );
        }

        // Select a random index from the player list.
        gameState.CurrentPlayerIndex =
            Random.Shared.Next(gameState.Players.Count);
    }

    // Returns the player whose turn it currently is.
    public PlayerGameState GetCurrentPlayer(GameSessionState gameState)
    {
        if (gameState.Players.Count == 0)
        {
            throw new InvalidOperationException(
                "The game must contain at least one player."
            );
        }

        return gameState.Players[gameState.CurrentPlayerIndex];
    }

    // Moves the turn to the next player.
    // After the last player, the turn returns to the first player.
    public void MoveToNextPlayer(GameSessionState gameState)
    {
        if (gameState.Players.Count == 0)
        {
            throw new InvalidOperationException(
                "The game must contain at least one player."
            );
        }

        gameState.CurrentPlayerIndex++;

        // Return to the first player after the last player.
        if (gameState.CurrentPlayerIndex >= gameState.Players.Count)
        {
            gameState.CurrentPlayerIndex = 0;
        }
    }

    

}
