// Gives this test file access to the game models.
using HMJT.Models;

// Gives this test file access to the services being tested.
using HMJT.Services;

// Organizes tests for service classes.
namespace HMJT.Tests.Services;

public class GameTurnServiceTests
{
    [Fact]
    public void StartTurn_ShouldCreateValidTurn()
    {
        // Create a game with two players.
        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                new PlayerGameState
                {
                    PlayerNumber = 1,
                    PlayerName = "Player 1"
                },

                new PlayerGameState
                {
                    PlayerNumber = 2,
                    PlayerName = "Player 2"
                }
            },

            // Player 2 currently has the turn.
            CurrentPlayerIndex = 1
        };

        // Create the services needed by GameTurnService.
        var diceService = new DiceService();
        var gameMechanicsService = new GameMechanicsService();
        var questionService = new QuestionService();

        // Give the required services to GameTurnService.
        var gameTurnService = new GameTurnService(
            diceService,
            gameMechanicsService,
            questionService
        );

        // Start a turn using Medium difficulty.
        GameTurnState result =
            gameTurnService.StartTurn(
                gameState,
                Difficulty.Medium
            );

        // The current player should be Player 2.
        Assert.Equal("Player 2", result.Player.PlayerName);
        Assert.Equal(2, result.Player.PlayerNumber);

        // The dice result must always be between 1 and 6.
        Assert.InRange(result.DiceRoll, 1, 6);

        // The category stored in the turn must match
        // the category that belongs to the dice result.
        string expectedCategory =
            gameMechanicsService.GetCategoryFromDiceRoll(
                result.DiceRoll
            );

        Assert.Equal(expectedCategory, result.Category);

        // The selected question must belong to
        // the same category as the dice result.
        Assert.Equal(
            result.Category,
            result.Question.Category
        );

        // The selected question must use
        // the difficulty chosen for the game.
        Assert.Equal(
            Difficulty.Medium,
            result.Question.Difficulty
        );
    }

    [Theory]

    [InlineData(Difficulty.Easy)]
    [InlineData(Difficulty.Medium)]
    [InlineData(Difficulty.Hard)]
    public void StartTurn_ShouldUseSelectedDifficulty(
        Difficulty difficulty)
    {
        // Create a game with one player.
        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                new PlayerGameState
                {
                    PlayerNumber = 1,
                    PlayerName = "Player 1"
                }
            }
        };

        var diceService = new DiceService();
        var gameMechanicsService = new GameMechanicsService();
        var questionService = new QuestionService();

        var gameTurnService = new GameTurnService(
            diceService,
            gameMechanicsService,
            questionService
        );

        // Start the turn using the difficulty
        // provided by the current InlineData.
        GameTurnState result =
            gameTurnService.StartTurn(
                gameState,
                difficulty
            );

        // The selected question must use
        // the requested difficulty.
        Assert.Equal(
            difficulty,
            result.Question.Difficulty
        );
    }

    [Fact]
    public void CompleteTurn_ShouldAwardWedgeForCorrectAnswer()
    {
        // Create a player with no wedges.
        var player = new PlayerGameState
        {
            PlayerNumber = 1,
            PlayerName = "Player 1"
        };

        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState> { player },
            CurrentPlayerIndex = 0
        };

        // Create a question where B is the correct answer.
        var question = new Question
        {
            QuestionId = 50,
            Category = "Java",
            CorrectAnswer = AnswerOption.B
        };

        var turnState = new GameTurnState
        {
            Player = player,
            Question = question,
            Category = "Java"
        };

        var gameTurnService = new GameTurnService(
            new DiceService(),
            new GameMechanicsService(),
            new QuestionService()
        );

        // Complete the turn with the correct answer.
        GameTurnResult result =
            gameTurnService.CompleteTurn(
                gameState,
                turnState,
                AnswerOption.B
            );

        // The answer should be correct.
        Assert.True(result.GameAnswer.IsCorrect);

        // The player should receive a new Java wedge.
        Assert.True(result.WedgeAwarded);
        Assert.Contains("Java", player.Wedges);
    }

    [Fact]
    public void CompleteTurn_ShouldNotAwardWedgeForWrongAnswer()
    {
        var player = new PlayerGameState
        {
            PlayerNumber = 1,
            PlayerName = "Player 1"
        };

        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState> { player }
        };

        var question = new Question
        {
            QuestionId = 51,
            Category = "Python",
            CorrectAnswer = AnswerOption.C
        };

        var turnState = new GameTurnState
        {
            Player = player,
            Question = question,
            Category = "Python"
        };

        var gameTurnService = new GameTurnService(
            new DiceService(),
            new GameMechanicsService(),
            new QuestionService()
        );

        // The player selects A even though C is correct.
        GameTurnResult result =
            gameTurnService.CompleteTurn(
                gameState,
                turnState,
                AnswerOption.A
            );

        Assert.False(result.GameAnswer.IsCorrect);
        Assert.False(result.WedgeAwarded);

        // Python should not have been added.
        Assert.DoesNotContain("Python", player.Wedges);
    }

    [Fact]
    public void CompleteTurn_ShouldNotAwardDuplicateWedge()
    {
        var player = new PlayerGameState
        {
            PlayerNumber = 1,
            PlayerName = "Player 1",
            Wedges = new HashSet<string> { "C#" }
        };

        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState> { player }
        };

        var question = new Question
        {
            QuestionId = 52,
            Category = "C#",
            CorrectAnswer = AnswerOption.D
        };

        var turnState = new GameTurnState
        {
            Player = player,
            Question = question,
            Category = "C#"
        };

        var gameTurnService = new GameTurnService(
            new DiceService(),
            new GameMechanicsService(),
            new QuestionService()
        );

        GameTurnResult result =
            gameTurnService.CompleteTurn(
                gameState,
                turnState,
                AnswerOption.D
            );

        // The answer itself is correct.
        Assert.True(result.GameAnswer.IsCorrect);

        // No new wedge should be awarded.
        Assert.False(result.WedgeAwarded);

        // The player should still have only one C# wedge.
        Assert.Single(player.Wedges);
    }

    [Fact]
    public void CompleteTurn_ShouldMoveToNextPlayer()
    {
        var player1 = new PlayerGameState
        {
            PlayerNumber = 1,
            PlayerName = "Player 1"
        };

        var player2 = new PlayerGameState
        {
            PlayerNumber = 2,
            PlayerName = "Player 2"
        };

        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                player1,
                player2
            },

            CurrentPlayerIndex = 0
        };

        var question = new Question
        {
            QuestionId = 53,
            Category = "JavaScript",
            CorrectAnswer = AnswerOption.A
        };

        var turnState = new GameTurnState
        {
            Player = player1,
            Question = question,
            Category = "JavaScript"
        };

        var gameTurnService = new GameTurnService(
            new DiceService(),
            new GameMechanicsService(),
            new QuestionService()
        );

        // Complete Player 1's turn.
        gameTurnService.CompleteTurn(
            gameState,
            turnState,
            AnswerOption.A
        );

        // Player 2 should now have the turn.
        Assert.Equal(1, gameState.CurrentPlayerIndex);
        Assert.Equal(
            "Player 2",
            gameState.Players[gameState.CurrentPlayerIndex].PlayerName
        );
    }

    [Fact]
    public void StartTurn_ShouldCreateNormalTurnWhenPlayerIsMissingWedges()
    {
        // Create a player who does not have all six wedges.
        var player = new PlayerGameState
        {
            PlayerNumber = 1,
            PlayerName = "Player 1",
            Wedges = new HashSet<string>
            {
                "Java",
                "Python"
            }
        };

        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                player
            },

            CurrentPlayerIndex = 0
        };

        var gameMechanicsService = new GameMechanicsService();

        var gameTurnService = new GameTurnService(
            new DiceService(),
            gameMechanicsService,
            new QuestionService()
        );

        // Start a normal turn.
        GameTurnState result =
            gameTurnService.StartTurn(
                gameState,
                Difficulty.Easy
            );

        // The player does not have all wedges,
        // so this should not be a final turn.
        Assert.False(result.IsFinalTurn);

        // A normal turn should contain a valid dice roll.
        Assert.InRange(result.DiceRoll, 1, 6);

        // The category should match the dice result.
        string expectedCategory =
            gameMechanicsService.GetCategoryFromDiceRoll(
                result.DiceRoll
            );

        Assert.Equal(expectedCategory, result.Category);

        // The selected question should belong
        // to the same category.
        Assert.Equal(
            result.Category,
            result.Question.Category
        );
    }

    [Fact]
    public void StartTurn_ShouldCreateFinalTurnWhenPlayerHasAllWedges()
    {
        // Create a player who has collected all six wedges.
        var player = new PlayerGameState
        {
            PlayerNumber = 1,
            PlayerName = "Player 1",

            Wedges = new HashSet<string>
            {
                "Java",
                "JavaScript",
                "HTML/CSS",
                "Python",
                "Game History",
                "C#"
            }
        };

        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                player
            },

            CurrentPlayerIndex = 0
        };

        var gameTurnService = new GameTurnService(
            new DiceService(),
            new GameMechanicsService(),
            new QuestionService()
        );

        // Start the player's next turn after all
        // six wedges have already been collected.
        GameTurnState result =
            gameTurnService.StartTurn(
                gameState,
                Difficulty.Medium
            );

        // The turn should now be a final question turn.
        Assert.True(result.IsFinalTurn);

        // The correct player should still own the turn.
        Assert.Equal(
            "Player 1",
            result.Player.PlayerName
        );

        // No dice is used during a final turn.
        // int defaults to 0 when no value has been assigned.
        Assert.Equal(0, result.DiceRoll);

        // The other players have not selected
        // the final category yet.
        Assert.Equal(string.Empty, result.Category);
    }

    [Fact]
    public void SetFinalQuestion_ShouldUseSelectedCategoryAndDifficulty()
    {
        // Create a player who has all six wedges.
        var player = new PlayerGameState
        {
            PlayerNumber = 1,
            PlayerName = "Player 1",
            Wedges = new HashSet<string>
            {
                "Java",
                "JavaScript",
                "HTML/CSS",
                "Python",
                "Game History",
                "C#"
            }
        };

        // Create a final turn for the player.
        var turnState = new GameTurnState
        {
            Player = player,
            IsFinalTurn = true
        };

        var gameTurnService = new GameTurnService(
            new DiceService(),
            new GameMechanicsService(),
            new QuestionService()
        );

        // The other players choose Python as the final category.
        gameTurnService.SetFinalQuestion(
            turnState,
            "Python",
            Difficulty.Hard
        );

        // The selected category should be stored in the turn.
        Assert.Equal("Python", turnState.Category);

        // The selected question must belong to the chosen category.
        Assert.Equal(
            "Python",
            turnState.Question.Category
        );

        // The selected question must use the game's difficulty.
        Assert.Equal(
            Difficulty.Hard,
            turnState.Question.Difficulty
        );
    }

    [Fact]
    public void SetFinalQuestion_ShouldThrowWhenTurnIsNotFinal()
    {
        // Create a normal turn.
        var turnState = new GameTurnState
        {
            Player = new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = "Player 1"
            },

            IsFinalTurn = false
        };

        var gameTurnService = new GameTurnService(
            new DiceService(),
            new GameMechanicsService(),
            new QuestionService()
        );

        // A final question should not be allowed
        // during a normal turn.
        Assert.Throws<InvalidOperationException>(() =>
            gameTurnService.SetFinalQuestion(
                turnState,
                "Java",
                Difficulty.Easy
            ));
    }

    [Fact]
    public void SetFinalQuestion_ShouldNotChangeDiceRoll()
    {
        var turnState = new GameTurnState
        {
            Player = new PlayerGameState
            {
                PlayerNumber = 1,
                PlayerName = "Player 1"
            },

            IsFinalTurn = true,

            // Final turns should not use a dice roll.
            DiceRoll = 0
        };

        var gameTurnService = new GameTurnService(
            new DiceService(),
            new GameMechanicsService(),
            new QuestionService()
        );

        gameTurnService.SetFinalQuestion(
            turnState,
            "C#",
            Difficulty.Medium
        );

        // Selecting the final category must not roll or modify the dice.
        Assert.Equal(0, turnState.DiceRoll);
    }

    [Fact]
    public void CompleteFinalTurn_ShouldEndGameForCorrectAnswer()
    {
        // Create a player who has reached the final turn.
        var player = new PlayerGameState
        {
            PlayerNumber = 1,
            PlayerName = "Player 1"
        };

        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                player
            },

            CurrentPlayerIndex = 0,
            IsGameOver = false
        };

        // Create a final question where B is correct.
        var question = new Question
        {
            QuestionId = 60,
            Category = "Java",
            CorrectAnswer = AnswerOption.B
        };

        var turnState = new GameTurnState
        {
            Player = player,
            Question = question,
            Category = "Java",
            IsFinalTurn = true
        };

        var gameTurnService = new GameTurnService(
            new DiceService(),
            new GameMechanicsService(),
            new QuestionService()
        );

        // Complete the final turn with the correct answer.
        GameTurnResult result =
            gameTurnService.CompleteFinalTurn(
                gameState,
                turnState,
                AnswerOption.B
            );

        // The answer should be correct.
        Assert.True(result.GameAnswer.IsCorrect);

        // The game should now be finished.
        Assert.True(gameState.IsGameOver);

        // The current player should be stored as the winner.
        Assert.Equal(player, gameState.Winner);

        // No wedge is awarded during the final question.
        Assert.False(result.WedgeAwarded);
    }

    [Fact]
    public void CompleteFinalTurn_ShouldContinueGameForWrongAnswer()
    {
        var player1 = new PlayerGameState
        {
            PlayerNumber = 1,
            PlayerName = "Player 1"
        };

        var player2 = new PlayerGameState
        {
            PlayerNumber = 2,
            PlayerName = "Player 2"
        };

        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                player1,
                player2
            },

            CurrentPlayerIndex = 0,
            IsGameOver = false
        };

        var question = new Question
        {
            QuestionId = 61,
            Category = "Python",
            CorrectAnswer = AnswerOption.C
        };

        var turnState = new GameTurnState
        {
            Player = player1,
            Question = question,
            Category = "Python",
            IsFinalTurn = true
        };

        var gameTurnService = new GameTurnService(
            new DiceService(),
            new GameMechanicsService(),
            new QuestionService()
        );

        // Player 1 selects the wrong answer.
        GameTurnResult result =
            gameTurnService.CompleteFinalTurn(
                gameState,
                turnState,
                AnswerOption.A
            );

        // The answer should be incorrect.
        Assert.False(result.GameAnswer.IsCorrect);

        // The game should continue.
        Assert.False(gameState.IsGameOver);

        // There should still be no winner.
        Assert.Null(gameState.Winner);

        // The turn should move to Player 2.
        Assert.Equal(1, gameState.CurrentPlayerIndex);
        Assert.Equal(
            "Player 2",
            gameState.Players[gameState.CurrentPlayerIndex].PlayerName
        );
    }

    [Fact]
    public void CompleteFinalTurn_ShouldThrowWhenTurnIsNotFinal()
    {
        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                new PlayerGameState
                {
                    PlayerNumber = 1,
                    PlayerName = "Player 1"
                }
            }
        };

        // This is a normal turn, not a final turn.
        var turnState = new GameTurnState
        {
            Player = gameState.Players[0],
            IsFinalTurn = false
        };

        var gameTurnService = new GameTurnService(
            new DiceService(),
            new GameMechanicsService(),
            new QuestionService()
        );

        // CompleteFinalTurn should only be allowed
        // when IsFinalTurn is true.
        Assert.Throws<InvalidOperationException>(() =>
            gameTurnService.CompleteFinalTurn(
                gameState,
                turnState,
                AnswerOption.A
            ));
    }



}