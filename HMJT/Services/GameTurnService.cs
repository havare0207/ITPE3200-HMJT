// Gives this service access to the game models.
using HMJT.Models;

// Places GameTurnService in the HMJT.Services namespace.
namespace HMJT.Services;

// Coordinates the different services needed for one game turn.
public class GameTurnService
{
    // Service used for rolling the dice.
    private readonly DiceService _diceService;

    // Service used for game rules and category selection.
    private readonly GameMechanicsService _gameMechanicsService;

    // Service used for selecting questions.
    private readonly QuestionService _questionService;

    // Constructor for GameTurnService.
    // The required services are provided when a new GameTurnService
    // object is created.

    public GameTurnService(
        DiceService diceService,
        GameMechanicsService gameMechanicsService,
        QuestionService questionService)
    {
        // Store the provided DiceService so it can be used
        // by other methods in this class.
        _diceService = diceService;

        // Store the provided GameMechanicsService.
        _gameMechanicsService = gameMechanicsService;

        // Store the provided QuestionService.
        _questionService = questionService;
    }

    // Starts one player's turn.
    // Players with all six wedges enter a final question turn.
    // Other players start a normal turn with a dice roll.
    public GameTurnState StartTurn(
        GameSessionState gameState,
        Difficulty difficulty)
    {
        // Get the player whose turn it currently is.
        PlayerGameState player =
            _gameMechanicsService.GetCurrentPlayer(gameState);

        // If the player already has all six wedges,
        // this becomes a final question turn.
        // The dice is not rolled because the other players
        // will choose the final category.
        if (_gameMechanicsService.HasAllWedges(player))
        {
            return new GameTurnState
            {
                Player = player,
                IsFinalTurn = true
            };
        }

        // Roll the dice for a normal turn.
        int diceRoll =
            _diceService.RollDice();

        // Convert the dice roll into a category.
        string category =
            _gameMechanicsService.GetCategoryFromDiceRoll(diceRoll);

        // Select a question matching the category
        // and the game's selected difficulty.
        Question question =
            _questionService.GetRandomQuestion(
                category,
                difficulty
            );

        // Return the information for a normal turn.
        return new GameTurnState
        {
            Player = player,
            DiceRoll = diceRoll,
            Category = category,
            Question = question,
            IsFinalTurn = false
        };
    }

    // Processes the player's answer, awards a wedge if appropriate,
    // moves the game to the next player, and returns the turn result.
    public GameTurnResult CompleteTurn(
        GameSessionState gameState,
        GameTurnState turnState,
        AnswerOption selectedAnswer)
    {
        // Create a GameAnswer containing the player's answer
        // and whether it was correct.
        GameAnswer gameAnswer =
            _gameMechanicsService.CreateGameAnswer(
                turnState.Question,
                turnState.Player.PlayerName,
                selectedAnswer
            );

        // Try to award the wedge for the question's category.
        // This only succeeds if the answer was correct
        // and the player does not already own the wedge.
        bool wedgeAwarded =
            _gameMechanicsService.AwardWedge(
                turnState.Player,
                turnState.Question,
                gameAnswer
            );

        // Move the turn to the next player.
        _gameMechanicsService.MoveToNextPlayer(gameState);

        // Return information about what happened during the turn.
        return new GameTurnResult
        {
            GameAnswer = gameAnswer,
            WedgeAwarded = wedgeAwarded
        };
    }

    // Sets the category for a final turn and selects
    // a question from that category without rolling the dice.
    public void SetFinalQuestion(
        GameTurnState turnState,
        string selectedCategory,
        Difficulty difficulty)
    {
        // This method should only be used during a final turn.
        if (!turnState.IsFinalTurn)
        {
            throw new InvalidOperationException(
                "A final question can only be selected during a final turn."
            );
        }

        // Store the category chosen by the other players.
        turnState.Category = selectedCategory;

        // Select a question that matches the chosen category
        // and the game's selected difficulty.
        turnState.Question =
            _questionService.GetRandomQuestion(
                selectedCategory,
                difficulty
            );
    }

    // Completes a final question turn.
    // A correct answer ends the game and sets the winner.
    // A wrong answer ends the turn and moves to the next player.
    public GameTurnResult CompleteFinalTurn(
        GameSessionState gameState,
        GameTurnState turnState,
        AnswerOption selectedAnswer)
    {
        // This method should only be used for final turns.
        if (!turnState.IsFinalTurn)
        {
            throw new InvalidOperationException(
                "A final turn can only be completed during a final question turn."
            );
        }

        // Create a GameAnswer and check whether
        // the selected answer is correct.
        GameAnswer gameAnswer =
            _gameMechanicsService.CreateGameAnswer(
                turnState.Question,
                turnState.Player.PlayerName,
                selectedAnswer
            );

        // If the final answer is correct,
        // the current player wins the game.
        if (gameAnswer.IsCorrect)
        {
            gameState.IsGameOver = true;
            gameState.Winner = turnState.Player;

            return new GameTurnResult
            {
                GameAnswer = gameAnswer,
                WedgeAwarded = false
            };
        }

        // If the final answer is wrong,
        // the game continues with the next player.
        _gameMechanicsService.MoveToNextPlayer(gameState);

        return new GameTurnResult
        {
            GameAnswer = gameAnswer,
            WedgeAwarded = false
        };
    }

    // Starts a question turn after the player
    // has moved to a colored board space.
    // The category comes from the board space
    // instead of from the dice roll.
    public GameTurnState StartBoardQuestion(
        PlayerGameState player,
        string category,
        Difficulty difficulty)
    {
        // Select a random question that matches
        // the category of the board space and
        // the difficulty chosen for the game.
        Question question =
            _questionService.GetRandomQuestion(
                category,
                difficulty
            );

        // Create a new turn containing the
        // question from the landed board space.
        return new GameTurnState
        {
            Player = player,
            DiceRoll = 0,
            Category = category,
            Question = question,
            IsFinalTurn = false
        };
    }

}