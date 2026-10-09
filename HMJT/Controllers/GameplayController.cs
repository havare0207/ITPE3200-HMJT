// Gives this controller access to ASP.NET Core MVC.
using Microsoft.AspNetCore.Mvc;

// Gives this controller access to the game services.
using HMJT.Services;

// Gives this controller access to the game models.
using HMJT.Models;

// Places GameplayController in the HMJT.Controllers namespace.
namespace HMJT.Controllers;

// Handles requests related to the playable Code Pursuit game.
public class GameplayController : Controller
{
    // Service used for starting and completing question turns.
    private readonly GameTurnService _gameTurnService;

    // Stores the current temporary in-memory game state.
    private readonly GameStateService _gameStateService;

    // Service used for game rules such as player order
    // and checking collected wedges.
    private readonly GameMechanicsService _gameMechanicsService;

    // Service used for the board structure
    // and calculating legal movement.
    private readonly BoardService _boardService;

    // Service used for rolling the game dice.
    private readonly DiceService _diceService;


    // Constructor for GameplayController.
    // The required services are provided through dependency injection.
    public GameplayController(
        GameTurnService gameTurnService,
        GameStateService gameStateService,
        GameMechanicsService gameMechanicsService,
        BoardService boardService,
        DiceService diceService)
    {
        _gameTurnService = gameTurnService;
        _gameStateService = gameStateService;
        _gameMechanicsService = gameMechanicsService;
        _boardService = boardService;
        _diceService = diceService;
    }


    // Displays the main game page.
    // If no game has been started yet,
    // the setup form is shown instead.
    public IActionResult Play()
    {
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

        // Get the current active question turn.
        // This is null when no question is being answered.
        GameTurnState? turnState =
            _gameStateService.CurrentTurn;

        // Get the current board movement.
        // This is null before rolling and after moving.
        BoardMoveState? moveState =
            _gameStateService.CurrentMove;

        // Check whether the player has rolled the dice
        // and is currently choosing a destination.
        bool isWaitingForMove =
            moveState?.IsWaitingForMove == true;

        // Check whether a question is currently active.
        bool hasActiveTurn =
            turnState != null;

        // A center choice is only pending after the player
        // has actually moved into the center.
        //
        // Simply starting the game on space 43 must not
        // automatically trigger a center question.
        bool centerChoicePending =
            _gameStateService.IsCenterChoicePending;

        // Normal center category choice:
        // the player reached the center but does not yet
        // have all six category wedges.
        bool needsCenterCategory =
            !gameState.IsGameOver &&
            !hasActiveTurn &&
            !isWaitingForMove &&
            centerChoicePending &&
            !_gameMechanicsService.HasAllWedges(currentPlayer);

        // Final center category choice:
        // the player reached the center,
        // has collected all six wedges,
        // and is allowed to attempt the final question.
        bool needsFinalCategory =
            !gameState.IsGameOver &&
            !hasActiveTurn &&
            !isWaitingForMove &&
            centerChoicePending &&
            _gameMechanicsService.HasAllWedges(currentPlayer) &&
            !currentPlayer.MustLeaveCenterBeforeFinalRetry;

        // Create a ViewModel containing all data
        // required by Play.cshtml.
        var viewModel = new GamePlayViewModel
        {
            // A game session is active.
            HasActiveGame = true,

            // Current player information.
            PlayerName = currentPlayer.PlayerName,
            PlayerNumber = currentPlayer.PlayerNumber,

            // Show the dice roll from movement first.
            // If no movement exists, use the question turn value.
            // Otherwise use zero.
            DiceRoll =
                moveState?.DiceRoll
                ?? turnState?.DiceRoll
                ?? 0,

            // Current board position.
            CurrentBoardSpaceId =
                currentPlayer.BoardSpaceId,

            // Board spaces that can currently be selected.
            LegalDestinationIds =
                moveState?.LegalDestinationIds
                ?? new List<int>(),

            // Shows whether the player must choose
            // a movement destination.
            IsWaitingForMove =
                isWaitingForMove,

            // Current question category.
            Category =
                turnState?.Category
                ?? string.Empty,

            // Current question.
            Question =
                turnState?.Question
                ?? new Question(),

            // Wedges already collected by the player.
            Wedges =
                currentPlayer.Wedges,

            // Wedges the player is still missing.
            MissingWedges =
                _gameMechanicsService.GetMissingWedges(
                    currentPlayer
                ),

            // Shows whether a question is active.
            HasActiveTurn =
                hasActiveTurn,

            // The player may roll only when:
            // - the game is not over,
            // - no question is active,
            // - no movement choice is active,
            // - and no center category choice is pending.
            CanRollDice =
                !gameState.IsGameOver &&
                !hasActiveTurn &&
                !isWaitingForMove &&
                !centerChoicePending,

            // Shows whether a normal center category
            // must be selected by the current player.
            NeedsCenterCategory =
                needsCenterCategory,

            // Shows whether the other players must choose
            // the category for the final question.
            NeedsFinalCategory =
                needsFinalCategory,

            // If a question exists, use its final-turn value.
            // Otherwise use the pending final state.
            IsFinalTurn =
                turnState?.IsFinalTurn
                ?? needsFinalCategory,

            // Game-over information.
            IsGameOver =
                gameState.IsGameOver,

            WinnerName =
                gameState.Winner?.PlayerName
                ?? string.Empty,

            // Temporary feedback shown on the page.
            Message =
                _gameStateService.Message,

            // Store all players so the view can later
            // render player pieces and wedge progress.
            Players =
                gameState.Players
        };

        // Sends the ViewModel to Views/Gameplay/Play.cshtml.
        return View(viewModel);
    }


    // Starts a new game using player names
    // and the selected question difficulty.
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

        // Players must be entered in numerical order.
        // Player 3 cannot be used unless Player 2 exists, etc.
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
        var gameState =
            new GameSessionState();

        // Player 1 is required.
        gameState.Players.Add(
            new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = player1
            }
        );

        // Add the remaining players only
        // when a name has been entered.
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
        _gameStateService.GameSession =
            gameState;

        // Store the selected question difficulty.
        _gameStateService.Difficulty =
            difficulty;

        // No question is active at game start.
        _gameStateService.CurrentTurn =
            null;

        // No movement is active at game start.
        _gameStateService.CurrentMove =
            null;

        // Although every player starts physically
        // in the center, they have not moved into it yet.
        // Therefore no center category choice is pending.
        _gameStateService.IsCenterChoicePending =
            false;

        _gameStateService.Message =
            "Game started. The first player can roll the dice.";

        return RedirectToAction(nameof(Play));
    }


    // Rolls the dice and calculates every legal
    // destination for the current player.
    [HttpPost]
    public IActionResult RollDice()
    {
        // A game must currently be active.
        if (!_gameStateService.HasActiveGame)
        {
            return RedirectToAction(nameof(Play));
        }

        // Do not allow another roll while:
        // - a question is active,
        // - movement is already waiting,
        // - or a center category choice is pending.
        if (_gameStateService.CurrentTurn != null ||
            _gameStateService.CurrentMove != null ||
            _gameStateService.IsCenterChoicePending)
        {
            return RedirectToAction(nameof(Play));
        }

        GameSessionState gameState =
            _gameStateService.GameSession!;

        // Get the player whose turn it is.
        PlayerGameState player =
            _gameMechanicsService.GetCurrentPlayer(gameState);

        // Roll the dice.
        int diceRoll =
            _diceService.RollDice();

        // Calculate legal destinations.
        List<BoardSpace> legalDestinations =
            _boardService.GetLegalDestinations(
                player.BoardSpaceId,
                diceRoll
            );

        // Special final-retry rule:
        //
        // After answering the final question incorrectly,
        // the player normally has to leave the center
        // before attempting it again.
        //
        // However, rolling a 6 while still in the center
        // allows the player to select the center again.
        if (diceRoll == 6 &&
            player.BoardSpaceId == 43 &&
            player.MustLeaveCenterBeforeFinalRetry)
        {
            // BoardService normally excludes the player's
            // current position from wildcard destinations.
            // Add center manually for this special rule.
            BoardSpace center =
                _boardService.GetSpace(43);

            if (!legalDestinations.Any(
                    space => space.BoardSpaceId == 43))
            {
                legalDestinations.Add(center);
            }
        }

        // Store the movement state while the player
        // chooses where to move.
        _gameStateService.CurrentMove =
            new BoardMoveState
            {
                DiceRoll =
                    diceRoll,

                LegalDestinationIds =
                    legalDestinations
                        .Select(
                            space =>
                                space.BoardSpaceId
                        )
                        .ToList(),

                IsWaitingForMove =
                    true
            };

        // No question is active until movement is completed.
        _gameStateService.CurrentTurn =
            null;

        _gameStateService.Message =
            $"Rolled {diceRoll}. Choose a space to move to.";

        return RedirectToAction(nameof(Play));
    }


    // Moves the current player to one of the legal
    // board destinations created by the dice roll.
    [HttpPost]
    public IActionResult Move(int boardSpaceId)
    {
        // A game must be active and the player
        // must currently be waiting to move.
        if (!_gameStateService.HasActiveGame ||
            _gameStateService.CurrentMove == null ||
            !_gameStateService.CurrentMove.IsWaitingForMove)
        {
            return RedirectToAction(nameof(Play));
        }

        BoardMoveState moveState =
            _gameStateService.CurrentMove;

        // The requested destination must be one
        // of the calculated legal destinations.
        if (!moveState
            .LegalDestinationIds
            .Contains(boardSpaceId))
        {
            _gameStateService.Message =
                "That space is not a legal destination.";

            return RedirectToAction(nameof(Play));
        }

        GameSessionState gameState =
            _gameStateService.GameSession!;

        // Get the current player.
        PlayerGameState player =
            _gameMechanicsService.GetCurrentPlayer(gameState);

        // Move the player's piece.
        player.BoardSpaceId =
            boardSpaceId;

        // Get the board-space information
        // for the selected destination.
        BoardSpace landedSpace =
            _boardService.GetSpace(boardSpaceId);

        // The movement itself is now complete.
        _gameStateService.CurrentMove =
            null;

        // No center choice is pending unless
        // this movement specifically lands in center.
        _gameStateService.IsCenterChoicePending =
            false;


        // -------------------------------------------------
        // FINAL-RETRY MOVEMENT
        // -------------------------------------------------

        // If the player previously failed the final question
        // and now leaves the center, they have satisfied
        // the requirement to move away before trying again.
        if (player.MustLeaveCenterBeforeFinalRetry &&
            boardSpaceId != 43)
        {
            player.MustLeaveCenterBeforeFinalRetry =
                false;
        }

        // Special exception:
        // rolling a 6 while still in center allows
        // the player to select center again immediately.
        if (player.MustLeaveCenterBeforeFinalRetry &&
            boardSpaceId == 43 &&
            moveState.DiceRoll == 6)
        {
            player.MustLeaveCenterBeforeFinalRetry =
                false;
        }


        // -------------------------------------------------
        // CENTER
        // -------------------------------------------------

        // Center has no fixed category.
        // The next screen must therefore ask
        // somebody to choose a category.
        if (landedSpace.SpaceType ==
            BoardSpaceType.Center)
        {
            _gameStateService.CurrentTurn =
                null;

            // Record that center was reached through movement.
            _gameStateService.IsCenterChoicePending =
                true;

            // A player with all six wedges is attempting
            // the final question.
            if (_gameMechanicsService
                .HasAllWedges(player))
            {
                _gameStateService.Message =
                    $"{player.PlayerName} reached the center. " +
                    "The other players must choose the final category.";
            }
            else
            {
                // During a normal round, the current player
                // chooses their own center category.
                _gameStateService.Message =
                    $"{player.PlayerName} reached the center " +
                    "and may choose a category.";
            }

            return RedirectToAction(nameof(Play));
        }


        // -------------------------------------------------
        // ROLL AGAIN
        // -------------------------------------------------

        // Roll Again does not create a question.
        // The current player keeps the same turn
        // and may immediately roll again.
        if (landedSpace.SpaceType ==
            BoardSpaceType.RollAgain)
        {
            _gameStateService.CurrentTurn =
                null;

            _gameStateService.Message =
                $"{player.PlayerName} landed on Roll Again " +
                "and can roll again.";

            return RedirectToAction(nameof(Play));
        }


        // -------------------------------------------------
        // NORMAL CATEGORY SPACE
        // -------------------------------------------------

        // A normal colored board space creates a question.
        // The small triangle/color of the exact space
        // determines the question category.
        if (landedSpace.SpaceType ==
            BoardSpaceType.Category)
        {
            _gameStateService.CurrentTurn =
                _gameTurnService.StartBoardQuestion(
                    player,
                    landedSpace.Category!,
                    _gameStateService.Difficulty
                );

            _gameStateService.Message =
                $"{player.PlayerName} landed on " +
                $"{landedSpace.Category}.";

            return RedirectToAction(nameof(Play));
        }


        // Fallback for an unexpected board-space type.
        _gameStateService.CurrentTurn =
            null;

        _gameStateService.Message =
            "The selected board space could not be handled.";

        return RedirectToAction(nameof(Play));
    }


    // Starts a normal question after the current player
    // reaches the center and chooses a category.
    [HttpPost]
    public IActionResult SelectCenterCategory(
        string category)
    {
        // A game must currently be active.
        if (!_gameStateService.HasActiveGame)
        {
            return RedirectToAction(nameof(Play));
        }

        GameSessionState gameState =
            _gameStateService.GameSession!;

        PlayerGameState player =
            _gameMechanicsService.GetCurrentPlayer(gameState);

        // This action is only valid after the player
        // has actually moved into center.
        if (!_gameStateService.IsCenterChoicePending ||
            player.BoardSpaceId != 43)
        {
            return RedirectToAction(nameof(Play));
        }

        // Players with all six wedges must use
        // the final-category flow instead.
        if (_gameMechanicsService.HasAllWedges(player))
        {
            return RedirectToAction(nameof(Play));
        }

        // Reject category names that do not belong
        // to the six supported game categories.
        if (!IsValidCategory(category))
        {
            _gameStateService.Message =
                "Invalid category.";

            return RedirectToAction(nameof(Play));
        }

        // Create a normal question using
        // the category selected by the player.
        _gameStateService.CurrentTurn =
            _gameTurnService.StartBoardQuestion(
                player,
                category,
                _gameStateService.Difficulty
            );

        // The center choice has now been completed.
        _gameStateService.IsCenterChoicePending =
            false;

        _gameStateService.Message =
            $"{player.PlayerName} chose {category} in the center.";

        return RedirectToAction(nameof(Play));
    }


    // Starts the final question using the category
    // selected by the other players.
    [HttpPost]
    public IActionResult SelectFinalCategory(
        string category)
    {
        // A game must currently be active.
        if (!_gameStateService.HasActiveGame)
        {
            return RedirectToAction(nameof(Play));
        }

        GameSessionState gameState =
            _gameStateService.GameSession!;

        PlayerGameState player =
            _gameMechanicsService.GetCurrentPlayer(gameState);

        // Final category selection is only valid when:
        // - the player moved into center,
        // - the player has all six wedges,
        // - and a retry is currently allowed.
        if (!_gameStateService.IsCenterChoicePending ||
            player.BoardSpaceId != 43 ||
            !_gameMechanicsService.HasAllWedges(player) ||
            player.MustLeaveCenterBeforeFinalRetry)
        {
            return RedirectToAction(nameof(Play));
        }

        // Reject unknown categories.
        if (!IsValidCategory(category))
        {
            _gameStateService.Message =
                "Invalid category.";

            return RedirectToAction(nameof(Play));
        }

        // StartTurn recognizes that a player with
        // all six wedges needs a final turn.
        GameTurnState finalTurn =
            _gameTurnService.StartTurn(
                gameState,
                _gameStateService.Difficulty
            );

        // Protect against entering this action
        // when the generated turn is not actually final.
        if (!finalTurn.IsFinalTurn)
        {
            return RedirectToAction(nameof(Play));
        }

        // Use the category selected by the other players
        // to generate the final question.
        _gameTurnService.SetFinalQuestion(
            finalTurn,
            category,
            _gameStateService.Difficulty
        );

        _gameStateService.CurrentTurn =
            finalTurn;

        // Category selection has now been completed.
        _gameStateService.IsCenterChoicePending =
            false;

        _gameStateService.Message =
            string.Empty;

        return RedirectToAction(nameof(Play));
    }


    // Processes the answer selected
    // for the current question.
    [HttpPost]
    public IActionResult SubmitAnswer(
        AnswerOption selectedAnswer)
    {
        // A game and active question must exist.
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


        // -------------------------------------------------
        // FINAL QUESTION
        // -------------------------------------------------

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
                    $"{turnState.Player.PlayerName} " +
                    "answered correctly and won the game!";
            }
            else
            {
                // A wrong final answer does not immediately
                // allow another final attempt.
                //
                // The player must normally leave center
                // and return later.
                turnState.Player
                    .MustLeaveCenterBeforeFinalRetry =
                    true;

                _gameStateService.Message =
                    "Wrong final answer. " +
                    "The player must leave the center " +
                    "before trying again.";
            }
        }


        // -------------------------------------------------
        // NORMAL QUESTION
        // -------------------------------------------------

        else
        {
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
                    $"Correct! " +
                    $"{turnState.Player.PlayerName} earned " +
                    $"the {turnState.Category} wedge.";
            }
            else
            {
                _gameStateService.Message =
                    $"Correct! " +
                    $"{turnState.Player.PlayerName} already has " +
                    $"the {turnState.Category} wedge.";
            }
        }

        // The completed question is no longer active.
        _gameStateService.CurrentTurn =
            null;

        // A completed question also ends
        // any previous center-choice state.
        _gameStateService.IsCenterChoicePending =
            false;

        return RedirectToAction(nameof(Play));
    }


    // Resets the current game
    // and returns to the setup screen.
    [HttpPost]
    public IActionResult ResetGame()
    {
        // Remove the active game session.
        _gameStateService.GameSession =
            null;

        // Remove the active question.
        _gameStateService.CurrentTurn =
            null;

        // Remove unfinished board movement.
        _gameStateService.CurrentMove =
            null;

        // Remove unfinished center-category selection.
        _gameStateService.IsCenterChoicePending =
            false;

        // Reset the selected difficulty.
        _gameStateService.Difficulty =
            Difficulty.Easy;

        // Remove temporary feedback.
        _gameStateService.Message =
            string.Empty;

        // Return to Play.
        // Because no active game exists,
        // the setup screen will be displayed.
        return RedirectToAction(nameof(Play));
    }


    // Checks whether a category name belongs
    // to one of the six supported question categories.
    private static bool IsValidCategory(
        string category)
    {
        return category == "Java" ||
               category == "JavaScript" ||
               category == "HTML/CSS" ||
               category == "Python" ||
               category == "Game History" ||
               category == "C#";
    }
}