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

        var gameTurnService =
            new GameTurnService(
                new DiceService(),
                gameMechanicsService,
                new QuestionService()
            );

        return new GameplayController(
            gameTurnService,
            gameStateService,
            gameMechanicsService
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
    public void RollDice_CreatesCurrentTurn()
    {
        // Arrange
        var gameStateService =
            new GameStateService
            {
                GameSession =
                    new GameSessionState
                    {
                        Players =
                        [
                            new PlayerGameState
                            {
                                PlayerNumber = 1,
                                PlayerName = "Alice"
                            }
                        ],
                        CurrentPlayerIndex = 0
                    },

                Difficulty =
                    Difficulty.Easy
            };

        var controller =
            CreateController(gameStateService);

        // Act
        IActionResult result =
            controller.RollDice();

        // Assert
        Assert.NotNull(
            gameStateService.CurrentTurn
        );

        Assert.Equal(
            "Alice",
            gameStateService
                .CurrentTurn!
                .Player
                .PlayerName
        );

        Assert.InRange(
            gameStateService
                .CurrentTurn
                .DiceRoll,
            1,
            6
        );

        Assert.IsType<RedirectToActionResult>(
            result
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
        var player =
            new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = "Alice",
                Wedges =
                [
                    "Java",
                    "JavaScript",
                    "HTML/CSS",
                    "Python",
                    "Game History",
                    "C#"
                ]
            };

        var gameStateService =
            new GameStateService
            {
                GameSession =
                    new GameSessionState
                    {
                        Players =
                        [
                            player
                        ],
                        CurrentPlayerIndex = 0
                    },

                Difficulty =
                    Difficulty.Medium
            };

        var controller =
            CreateController(gameStateService);

        // Act
        IActionResult result =
            controller.SelectFinalCategory(
                "Python"
            );

        // Assert
        Assert.NotNull(
            gameStateService.CurrentTurn
        );

        Assert.True(
            gameStateService
                .CurrentTurn!
                .IsFinalTurn
        );

        Assert.Equal(
            "Python",
            gameStateService
                .CurrentTurn
                .Category
        );

        Assert.Equal(
            Difficulty.Medium,
            gameStateService
                .CurrentTurn
                .Question
                .Difficulty
        );

        Assert.IsType<RedirectToActionResult>(
            result
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
}