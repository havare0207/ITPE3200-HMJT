using HMJT.Controllers;
using HMJT.Models;
using HMJT.Services;
using Microsoft.AspNetCore.Mvc;

namespace HMJT.Tests.Controllers;

public class GameplayControllerTests
{
    // Creates a GameController with the services
    // needed for the controller tests.
    private static GameplayController  CreateController(
        GameStateService gameStateService)
    {
        var gameMechanicsService =
            new GameMechanicsService();

        var diceService =
            new DiceService();

        var gameTurnService =
            new GameTurnService(
                new DiceService(),
                gameMechanicsService,
                new QuestionService()
            );

        var boardService =
            new BoardService();

        return new GameplayController(
            gameTurnService,
            gameStateService,
            gameMechanicsService,
            boardService,
            diceService
        );
    }

    // Tests that ResetGame removes the current
    // game session, active turn and feedback message.
    [Fact]
    public void ResetGame_ClearsCurrentGameState()
    {
        // Arrange
        var gameStateService = new GameStateService
        {
            GameSession = new(),
            CurrentTurn = new(),
            Message = "Test message"
        };

        var controller =
            CreateController(gameStateService);

        // Act
        IActionResult result =
            controller.ResetGame();

        // Assert
        Assert.Null(gameStateService.GameSession);
        Assert.Null(gameStateService.CurrentTurn);

        Assert.Equal(
            string.Empty,
            gameStateService.Message
        );

        Assert.IsType<RedirectToActionResult>(
            result
        );
    }

    // Tests that StartGame creates only the players
    // that have names and stores the selected difficulty.
    [Fact]
    public void StartGame_CreatesPlayersAndStoresDifficulty()
    {
        // Arrange
        var gameStateService =
            new GameStateService();

        var controller =
            CreateController(gameStateService);

        // Act
        IActionResult result =
            controller.StartGame(
                "Alice",
                "Bob",
                "",
                "",
                "",
                "",
                Difficulty.Hard
            );

        // Assert
        Assert.NotNull(
            gameStateService.GameSession
        );

        Assert.Equal(
            2,
            gameStateService.GameSession!
                .Players.Count
        );

        Assert.Equal(
            "Alice",
            gameStateService.GameSession
                .Players[0]
                .PlayerName
        );

        Assert.Equal(
            "Bob",
            gameStateService.GameSession
                .Players[1]
                .PlayerName
        );

        Assert.Equal(
            Difficulty.Hard,
            gameStateService.Difficulty
        );

        Assert.Null(
            gameStateService.CurrentTurn
        );

        Assert.IsType<RedirectToActionResult>(
            result
        );
    }

    // Tests that pressing Roll Dice creates
    // a new active turn for the current player.
    [Fact]
    public void RollDice_CreatesCurrentMove()
    {
        // Arrange
        var gameStateService = new GameStateService();

        var controller =
            CreateController(gameStateService);

        var gameState =
            new GameSessionState();

        gameState.Players.Add(
            new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = "Alice"
            }
        );

        gameState.CurrentPlayerIndex = 0;

        gameStateService.GameSession =
            gameState;

        // Act
        controller.RollDice();

        // Assert
        Assert.NotNull(
            gameStateService.CurrentMove
        );

        Assert.True(
            gameStateService.CurrentMove!
                .IsWaitingForMove
        );

        Assert.NotEmpty(
            gameStateService.CurrentMove
                .LegalDestinationIds
        );

        Assert.Null(
            gameStateService.CurrentTurn
        );
    }

    // Tests that submitting a correct answer
    // completes the turn, awards a wedge
    // and moves the game to the next player.
    [Fact]
    public void SubmitAnswer_CompletesTurnAndClearsCurrentTurn()
    {
        // Arrange
        var player1 =
            new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = "Alice"
            };

        var player2 =
            new PlayerGameState
            {
                PlayerNumber = 2,
                PlayerName = "Bob"
            };

        var question =
            new Question
            {
                QuestionId = 1,
                QuestionText = "Test question",
                Category = "Java",
                Difficulty = Difficulty.Easy,
                AnswerA = "A",
                AnswerB = "B",
                AnswerC = "C",
                AnswerD = "D",
                CorrectAnswer = AnswerOption.A
            };

        var gameStateService =
            new GameStateService
            {
                GameSession =
                    new GameSessionState
                    {
                        Players =
                        [
                            player1,
                            player2
                        ],
                        CurrentPlayerIndex = 0
                    },

                CurrentTurn =
                    new GameTurnState
                    {
                        Player = player1,
                        DiceRoll = 1,
                        Category = "Java",
                        Question = question,
                        IsFinalTurn = false
                    },

                Difficulty =
                    Difficulty.Easy
            };

        var controller =
            CreateController(gameStateService);

        // Act
        IActionResult result =
            controller.SubmitAnswer(
                AnswerOption.A
            );

        // Assert
        Assert.Null(
            gameStateService.CurrentTurn
        );

        Assert.Contains(
            "Correct",
            gameStateService.Message
        );

        Assert.Equal(
            1,
            gameStateService
                .GameSession!
                .CurrentPlayerIndex
        );

        Assert.Contains(
            "Java",
            player1.Wedges
        );

        Assert.IsType<RedirectToActionResult>(
            result
        );
    }

    // Tests that selecting a final category
    // creates a final turn with a question
    // from the selected category and difficulty.
    [Fact]
    public void SelectFinalCategory_CreatesFinalQuestion()
    {
        // Arrange
        var gameStateService =
            new GameStateService();

        var controller =
            CreateController(gameStateService);

        var player =
            new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = "Alice",
                BoardSpaceId = 43
            };

        // Give the player all six wedges.
        player.Wedges.Add("Java");
        player.Wedges.Add("JavaScript");
        player.Wedges.Add("HTML/CSS");
        player.Wedges.Add("Python");
        player.Wedges.Add("Game History");
        player.Wedges.Add("C#");

        var gameState =
            new GameSessionState();

        gameState.Players.Add(player);
        gameState.CurrentPlayerIndex = 0;

        gameStateService.GameSession =
            gameState;

        gameStateService.Difficulty =
            Difficulty.Easy;

        // Simulate that the player has just moved
        // into the center of the board.
        gameStateService.IsCenterChoicePending =
            true;

        // Act
        controller.SelectFinalCategory("Java");

        // Assert
        Assert.NotNull(
            gameStateService.CurrentTurn
        );

        Assert.True(
            gameStateService.CurrentTurn!.IsFinalTurn
        );

        Assert.Equal(
            "Java",
            gameStateService.CurrentTurn.Category
        );

        Assert.False(
            gameStateService.IsCenterChoicePending
        );
    }

    // Tests that answering the final question
    // correctly ends the game and stores
    // the current player as the winner.
    [Fact]
    public void SubmitAnswer_CorrectFinalAnswerEndsGame()
    {
        // Arrange
        var player =
            new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = "Alice"
            };

        var question =
            new Question
            {
                QuestionId = 1,
                QuestionText = "Final question",
                Category = "Java",
                Difficulty = Difficulty.Easy,
                AnswerA = "Correct",
                AnswerB = "Wrong",
                AnswerC = "Wrong",
                AnswerD = "Wrong",
                CorrectAnswer = AnswerOption.A
            };

        var gameState =
            new GameSessionState
            {
                Players =
                [
                    player
                ],
                CurrentPlayerIndex = 0
            };

        var gameStateService =
            new GameStateService
            {
                GameSession = gameState,

                CurrentTurn =
                    new GameTurnState
                    {
                        Player = player,
                        Category = "Java",
                        Question = question,
                        IsFinalTurn = true
                    }
            };

        var controller =
            CreateController(gameStateService);

        // Act
        IActionResult result =
            controller.SubmitAnswer(
                AnswerOption.A
            );

        // Assert
        Assert.True(
            gameState.IsGameOver
        );

        Assert.Equal(
            player,
            gameState.Winner
        );

        Assert.Null(
            gameStateService.CurrentTurn
        );

        Assert.Contains(
            "won the game",
            gameStateService.Message
        );

        Assert.IsType<RedirectToActionResult>(
            result
        );
    }

    // Tests that the game cannot start if Player 1 is empty
    // while another player name has been entered.
    [Fact]
    public void StartGame_RejectsGameWhenPlayer1IsMissing()
    {
        // Arrange
        var gameStateService =
            new GameStateService();

        var controller =
            CreateController(gameStateService);

        // Act
        IActionResult result =
            controller.StartGame(
                "",
                "Bob",
                "",
                "",
                "",
                "",
                Difficulty.Easy
            );

        // Assert
        Assert.Null(
            gameStateService.GameSession
        );

        Assert.Equal(
            "Player 1 must be entered.",
            gameStateService.Message
        );

        Assert.IsType<RedirectToActionResult>(
            result
        );
    }


    // Tests that players must be entered in order.
    // Player 3 cannot be used if Player 2 is empty.
    // There must be at least two players to start a game.
    [Fact]
    public void StartGame_RejectsSkippedPlayerNumber()
    {
        // Arrange
        var gameStateService =
            new GameStateService();

        var controller =
            CreateController(gameStateService);

        // Act
        controller.StartGame(
            "Alice",
            "Bob",
            "",
            "Diana",
            "",
            "",
            Difficulty.Easy
        );

        // Assert
        Assert.False(
            gameStateService.HasActiveGame
        );

        Assert.Equal(
            "Player 3 must be entered before Player 4.",
            gameStateService.Message
        );
    }

    // Tests that a new player starts
    // in the center of the game board.
    [Fact]
    public void PlayerGameState_ShouldStartInCenter()
    {
        // Arrange and Act
        var player = new PlayerGameState
        {
            PlayerNumber = 1,
            PlayerName = "Alice"
        };

        // Assert
        Assert.Equal(
            43,
            player.BoardSpaceId
        );
    }

    // Tests that the current player can move
    // to a legal board destination.
    [Fact]
    public void Move_LegalDestination_ShouldUpdatePlayerPosition()
    {
        // Arrange
        var gameStateService =
            new GameStateService();

        var controller =
            CreateController(gameStateService);

        var player =
            new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = "Alice",
                BoardSpaceId = 43
            };

        var gameState =
            new GameSessionState();

        gameState.Players.Add(player);
        gameState.CurrentPlayerIndex = 0;

        gameStateService.GameSession =
            gameState;

        gameStateService.CurrentMove =
            new BoardMoveState
            {
                DiceRoll = 1,
                LegalDestinationIds =
                    new List<int> { 27, 30, 33, 36, 39, 42 },

                IsWaitingForMove = true
            };

        // Act
        controller.Move(27);

        // Assert
        Assert.Equal(
            27,
            player.BoardSpaceId
        );

        Assert.Null(
            gameStateService.CurrentMove
        );
    }

    // Tests that the player cannot move
    // to a space that was not calculated
    // as a legal destination.
    [Fact]
    public void Move_IllegalDestination_ShouldNotMovePlayer()
    {
        // Arrange
        var gameStateService =
            new GameStateService();

        var controller =
            CreateController(gameStateService);

        var player =
            new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = "Alice",
                BoardSpaceId = 43
            };

        var gameState =
            new GameSessionState();

        gameState.Players.Add(player);
        gameState.CurrentPlayerIndex = 0;

        gameStateService.GameSession =
            gameState;

        gameStateService.CurrentMove =
            new BoardMoveState
            {
                DiceRoll = 1,
                LegalDestinationIds =
                    new List<int> { 27, 30, 33, 36, 39, 42 },

                IsWaitingForMove = true
            };

        // Act
        controller.Move(1);

        // Assert
        Assert.Equal(
            43,
            player.BoardSpaceId
        );

        Assert.NotNull(
            gameStateService.CurrentMove
        );
    }

    // Tests that landing on a colored board space
    // starts a question from that space's category.
    [Fact]
    public void Move_CategorySpace_ShouldCreateQuestionTurn()
    {
        // Arrange
        var gameStateService =
            new GameStateService();

        var controller =
            CreateController(gameStateService);

        var player =
            new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = "Alice",
                BoardSpaceId = 43
            };

        var gameState =
            new GameSessionState();

        gameState.Players.Add(player);
        gameState.CurrentPlayerIndex = 0;

        gameStateService.GameSession = gameState;
        gameStateService.Difficulty = Difficulty.Easy;

        gameStateService.CurrentMove =
            new BoardMoveState
            {
                DiceRoll = 1,
                LegalDestinationIds =
                    new List<int> { 27 },

                IsWaitingForMove = true
            };

        // Act
        controller.Move(27);

        // Assert
        Assert.Equal(
            27,
            player.BoardSpaceId
        );

        Assert.NotNull(
            gameStateService.CurrentTurn
        );

        Assert.Equal(
            "Java",
            gameStateService.CurrentTurn!.Category
        );
    }

    // Tests that landing on a Roll Again space
    // keeps the same player's turn and does not
    // create a question.
    [Fact]
    public void Move_RollAgainSpace_ShouldAllowSamePlayerToRollAgain()
    {
        // Arrange
        var gameStateService =
            new GameStateService();

        var controller =
            CreateController(gameStateService);

        var player =
            new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = "Alice",
                BoardSpaceId = 3
            };

        var gameState =
            new GameSessionState();

        gameState.Players.Add(player);
        gameState.CurrentPlayerIndex = 0;

        gameStateService.GameSession = gameState;

        gameStateService.CurrentMove =
            new BoardMoveState
            {
                DiceRoll = 1,
                LegalDestinationIds =
                    new List<int> { 4 },

                IsWaitingForMove = true
            };

        // Act
        controller.Move(4);

        // Assert
        Assert.Equal(
            4,
            player.BoardSpaceId
        );

        Assert.Null(
            gameStateService.CurrentMove
        );

        Assert.Null(
            gameStateService.CurrentTurn
        );

        Assert.Equal(
            0,
            gameState.CurrentPlayerIndex
        );
    }

    [Fact]
    public void StartGame_RequiresAtLeastTwoPlayers()
    {
        // Arrange
        var gameStateService =
            new GameStateService();

        var controller =
            CreateController(gameStateService);

        // Act
        controller.StartGame(
            "Alice",
            "",
            "",
            "",
            "",
            "",
            Difficulty.Easy
        );

        // Assert
        Assert.False(
            gameStateService.HasActiveGame
        );

        Assert.Equal(
            "At least two players are required.",
            gameStateService.Message
        );
    }

}

