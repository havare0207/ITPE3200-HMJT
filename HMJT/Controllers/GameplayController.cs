// Gives this controller access to ASP.NET Core MVC.
using Microsoft.AspNetCore.Mvc;

// Gives this controller access to the game services.
using HMJT.Services;

// Gives this controller access to the game models.
using HMJT.Models;

// Places GameController in the HMJT.Controllers namespace.
namespace HMJT.Controllers;

// Handles requests related to the game page.
public class GameplayController : Controller{
    // Service used for starting and completing game turns.
    private readonly GameTurnService _gameTurnService;

    // Stores the current temporary in-memory game state.
    private readonly GameStateService _gameStateService;

    // Service used for game rules such as selecting
    // the random starting player.
    private readonly GameMechanicsService _gameMechanicsService;

    // Constructor for GameController.
    // The required services are provided through dependency injection.
    public GameplayController(
        GameTurnService gameTurnService,
        GameStateService gameStateService,
        GameMechanicsService gameMechanicsService)
    {
        _gameTurnService = gameTurnService;
        _gameStateService = gameStateService;
        _gameMechanicsService = gameMechanicsService;
    }

    // Displays the main game page.
    // If no game has been started yet, the ViewModel
    // will contain empty/default game data.
    public IActionResult Play()
    {
        // If no game is currently active,
        // return an empty ViewModel to the Play view.
        // If no game is currently active,
        // return an empty ViewModel to the Play view.
        if (!_gameStateService.HasActiveGame)
        {
            var emptyViewModel = new GamePlayViewModel
            {
                HasActiveGame = false,
                Message = _gameStateService.Message
            };

            return View(emptyViewModel);
        }

        // Get the active game session.
        GameSessionState gameState =
            _gameStateService.GameSession!;

        // Get the player whose turn it currently is.
        PlayerGameState currentPlayer =
            _gameMechanicsService.GetCurrentPlayer(gameState);

        // Get the current active turn.
        // This can be null before the player rolls the dice.
        GameTurnState? turnState =
            _gameStateService.CurrentTurn;

        // Check whether a question is currently active.
        bool hasActiveTurn =
            turnState != null;

        // Check whether the current player has all six wedges
        // and therefore needs a final category instead of a dice roll.
        bool needsFinalCategory =
            !gameState.IsGameOver &&
            !hasActiveTurn &&
            _gameMechanicsService.HasAllWedges(currentPlayer);

        // Create a ViewModel containing all data
        // needed by the Play view.
        var viewModel = new GamePlayViewModel
        {
            // A game session is currently active.
            HasActiveGame = true,

            // Store the current player's name.
            PlayerName = currentPlayer.PlayerName,

            // Store the current player's number.
            PlayerNumber = currentPlayer.PlayerNumber,

            // Store the dice roll from the active turn.
            // If no turn exists yet, use 0.
            DiceRoll =
                turnState?.DiceRoll ?? 0,

            // Store the category from the active turn.
            // If no turn exists yet, use an empty string.
            Category =
                turnState?.Category ?? string.Empty,

            // Store the current question.
            // If no turn exists yet, create an empty Question object.
            Question =
                turnState?.Question ?? new Question(),

            // Store all wedges collected by the current player.
            Wedges = currentPlayer.Wedges,

            // Calculate and store the wedges
            // the current player is still missing.
            MissingWedges =
                _gameMechanicsService.GetMissingWedges(
                    currentPlayer
                ),

            // Shows whether a question is currently active.
            HasActiveTurn = hasActiveTurn,

            // The player can roll the dice only if:
            // the game is not over,
            // no question is currently active,
            // and this is not a final-category turn.
            CanRollDice =
                !gameState.IsGameOver &&
                !hasActiveTurn &&
                !needsFinalCategory,

            // Shows whether the other players must choose
            // a category for the current player's final question.
            NeedsFinalCategory =
                needsFinalCategory,

            // If a turn exists, use its IsFinalTurn value.
            // Otherwise, use needsFinalCategory.
            IsFinalTurn =
                turnState?.IsFinalTurn
                ?? needsFinalCategory,

            // Store whether the game has ended.
            IsGameOver =
                gameState.IsGameOver,

            // Store the winner's name if a winner exists.
            // Otherwise, use an empty string.
            WinnerName =
                gameState.Winner?.PlayerName
                ?? string.Empty,

            // Store the temporary feedback message
            // shown on the Play page.
            Message =
                _gameStateService.Message
        };

        // Sends the ViewModel to Views/Game/Play.cshtml.
        return View(viewModel);
    }

    // Starts a new game using player names and
    // the selected difficulty from the Play page.
    [HttpPost]
    public IActionResult StartGame(
        string player1,
        string player2,
        string player3,
        string player4,
        string player5,
        string player6,
        Difficulty difficulty)
    {
        // Player 1 must always be entered.
        if (string.IsNullOrWhiteSpace(player1))
        {
            _gameStateService.Message =
                "Player 1 must be entered.";

            return RedirectToAction(nameof(Play));
        }

        // Players must be entered in order.
        // Player 3 cannot be used unless Player 2 is also entered, etc.
        if (!string.IsNullOrWhiteSpace(player3) &&
            string.IsNullOrWhiteSpace(player2))
        {
            _gameStateService.Message =
                "Player 2 must be entered before Player 3.";

            return RedirectToAction(nameof(Play));
        }

        if (!string.IsNullOrWhiteSpace(player4) &&
            string.IsNullOrWhiteSpace(player3))
        {
            _gameStateService.Message =
                "Player 3 must be entered before Player 4.";

            return RedirectToAction(nameof(Play));
        }

        if (!string.IsNullOrWhiteSpace(player5) &&
            string.IsNullOrWhiteSpace(player4))
        {
            _gameStateService.Message =
                "Player 4 must be entered before Player 5.";

            return RedirectToAction(nameof(Play));
        }

        if (!string.IsNullOrWhiteSpace(player6) &&
            string.IsNullOrWhiteSpace(player5))
        {
            _gameStateService.Message =
                "Player 5 must be entered before Player 6.";

            return RedirectToAction(nameof(Play));
        }

        // Create a new game session.
        var gameState = new GameSessionState();

        // Player 1 is required.
        gameState.Players.Add(
            new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = player1
            }
        );

        // Add the remaining players only if their names were entered.
        if (!string.IsNullOrWhiteSpace(player2))
        {
            gameState.Players.Add(
                new PlayerGameState
                {
                    PlayerNumber = 2,
                    PlayerName = player2
                }
            );
        }

        if (!string.IsNullOrWhiteSpace(player3))
        {
            gameState.Players.Add(
                new PlayerGameState
                {
                    PlayerNumber = 3,
                    PlayerName = player3
                }
            );
        }

        if (!string.IsNullOrWhiteSpace(player4))
        {
            gameState.Players.Add(
                new PlayerGameState
                {
                    PlayerNumber = 4,
                    PlayerName = player4
                }
            );
        }

        if (!string.IsNullOrWhiteSpace(player5))
        {
            gameState.Players.Add(
                new PlayerGameState
                {
                    PlayerNumber = 5,
                    PlayerName = player5
                }
            );
        }

        if (!string.IsNullOrWhiteSpace(player6))
        {
            gameState.Players.Add(
                new PlayerGameState
                {
                    PlayerNumber = 6,
                    PlayerName = player6
                }
            );
        }

        // Select one random starting player.
        _gameMechanicsService
            .SelectRandomStartingPlayer(gameState);

        // Store the new game session.
        _gameStateService.GameSession = gameState;

        // Store the selected difficulty.
        _gameStateService.Difficulty = difficulty;

        // No turn has started yet.
        // The randomly selected player must press Roll Dice.
        _gameStateService.CurrentTurn = null;

        _gameStateService.Message =
            "Game started. The first player can roll the dice.";

        return RedirectToAction(nameof(Play));
    }

    // Starts a normal turn when the current player
    // presses the Roll Dice button.
    [HttpPost]
    public IActionResult RollDice()
    {
        // Make sure a game is currently active.
        if (!_gameStateService.HasActiveGame)
        {
            return RedirectToAction(nameof(Play));
        }

        GameSessionState gameState =
            _gameStateService.GameSession!;

        // Get the player whose turn it is.
        PlayerGameState player =
            _gameMechanicsService.GetCurrentPlayer(gameState);

        // Players with all six wedges do not roll the dice.
        // They must receive a final question instead.
        if (_gameMechanicsService.HasAllWedges(player))
        {
            return RedirectToAction(nameof(Play));
        }

        // StartTurn rolls the dice, finds the category
        // and selects the matching question.
        _gameStateService.CurrentTurn =
            _gameTurnService.StartTurn(
                gameState,
                _gameStateService.Difficulty
            );

        _gameStateService.Message = string.Empty;

        return RedirectToAction(nameof(Play));
    }

    // Processes the answer selected by the current player.
    // Processes the answer selected by the current player.
    [HttpPost]
    public IActionResult SubmitAnswer(
        AnswerOption selectedAnswer)
    {
        // A game and an active question must exist.
        if (!_gameStateService.HasActiveGame ||
            _gameStateService.CurrentTurn == null)
        {
            return RedirectToAction(nameof(Play));
        }

        GameSessionState gameState =
            _gameStateService.GameSession!;

        GameTurnState turnState =
            _gameStateService.CurrentTurn;

        GameTurnResult result;

        // Final turns use the special win-condition logic.
        if (turnState.IsFinalTurn)
        {
            result =
                _gameTurnService.CompleteFinalTurn(
                    gameState,
                    turnState,
                    selectedAnswer
                );

            if (result.GameAnswer.IsCorrect)
            {
                _gameStateService.Message =
                    $"{turnState.Player.PlayerName} answered correctly and won the game!";
            }
            else
            {
                _gameStateService.Message =
                    "Wrong final answer. The game continues.";
            }
        }
        else
        {
            // Complete a normal turn.
            result =
                _gameTurnService.CompleteTurn(
                    gameState,
                    turnState,
                    selectedAnswer
                );

            if (!result.GameAnswer.IsCorrect)
            {
                _gameStateService.Message =
                    "Wrong answer.";
            }
            else if (result.WedgeAwarded)
            {
                _gameStateService.Message =
                    $"Correct! {turnState.Player.PlayerName} earned the {turnState.Category} wedge.";
            }
            else
            {
                _gameStateService.Message =
                    $"Correct! {turnState.Player.PlayerName} already has the {turnState.Category} wedge.";
            }
        }

        // The completed question is no longer active.
        _gameStateService.CurrentTurn = null;

        return RedirectToAction(nameof(Play));
    }

    // Starts the final question using the category
    // selected by the other players.
    [HttpPost]
    public IActionResult SelectFinalCategory(
        string category)
    {
        if (!_gameStateService.HasActiveGame)
        {
            return RedirectToAction(nameof(Play));
        }

        GameSessionState gameState =
            _gameStateService.GameSession!;

        // StartTurn recognizes that the current player
        // has all wedges and creates a final turn.
        GameTurnState finalTurn =
            _gameTurnService.StartTurn(
                gameState,
                _gameStateService.Difficulty
            );

        // Protect against using this action
        // when the player is not actually in the final stage.
        if (!finalTurn.IsFinalTurn)
        {
            return RedirectToAction(nameof(Play));
        }

        // Use the category chosen by the other players
        // to select the final question.
        _gameTurnService.SetFinalQuestion(
            finalTurn,
            category,
            _gameStateService.Difficulty
        );

        _gameStateService.CurrentTurn = finalTurn;
        _gameStateService.Message = string.Empty;

        return RedirectToAction(nameof(Play));
    }

    // Resets the current game and returns to the game setup screen.
    [HttpPost]
    public IActionResult ResetGame()
    {
        // Remove the current game session.
        _gameStateService.GameSession = null;

        // Remove the current active turn.
        _gameStateService.CurrentTurn = null;

        // Reset the selected difficulty to the default value.
        _gameStateService.Difficulty = Difficulty.Easy;

        // Remove any temporary feedback message.
        _gameStateService.Message = string.Empty;

        // Return to the Play page.
        // Because there is no active game anymore,
        // the player setup form will be shown again.
        return RedirectToAction(nameof(Play));
    }

}