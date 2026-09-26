// Gives this test file access to GameMechanicsService
// from the main HMJT.Services namespace.

using HMJT.Services;

using HMJT.Models;

// Organizes tests for service classes.
// The full class name is:
// HMJT.Tests.Services.GameMechanicsServiceTests

namespace HMJT.Tests.Services;

public class GameMechanicsServiceTests
{
    // Theory is used when the same test should run
    // multiple times with different input values.

    [Theory]
    
    // Each InlineData contains:
    // dice roll -> expected category
    [InlineData(1, "Java")]
    [InlineData(2, "JavaScript")]
    [InlineData(3, "HTML/CSS")]
    [InlineData(4, "Python")]
    [InlineData(5, "Game History")]
    [InlineData(6, "C#")]

    public void GetCategoryFromDiceRoll_ShouldReturnCorrectCategory(
        int diceRoll,
        string expectedCategory)
    {
        // Create an instance of GameMechanicsService
        // so the method can be tested.
        var gameMechanicsService = new GameMechanicsService();

        // Call the method using the dice roll provided
        // by the current InlineData.

        string result =
            gameMechanicsService.GetCategoryFromDiceRoll(diceRoll);

        // Check that the returned category is the category
        // we expected for this dice roll.
        Assert.Equal(expectedCategory, result);
    }
    
    //test for numbers out of range:
    [Theory]
    
    [InlineData(0)]
    [InlineData(7)] 

    public void GetCategoryFromDiceRoll_ShouldThrowExceptionForInvalidRoll(
    int diceRoll)
    {   
        var gameMechanicsService = new GameMechanicsService();

        // Check that an invalid dice roll causes
        // an ArgumentOutOfRangeException.

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            gameMechanicsService.GetCategoryFromDiceRoll(diceRoll));

    }

    // Test to see if the answer is correct, then true. 
    [Fact]
    public void IsAnswerCorrect_ShouldReturnTrueForCorrectAnswer()
    {
        // Create a question where the correct answer is C.
        var question = new Question
        {
            QuestionId = 1,
            CorrectAnswer = AnswerOption.C
        };

        var gameMechanicsService = new GameMechanicsService();

        // Select the same answer as the question's correct answer.
        bool result = gameMechanicsService.IsAnswerCorrect(
            question,
            AnswerOption.C
        );

        // The answer should be marked as correct.
        Assert.True(result);
    }

    // And a test for wrong answer, should return false
    [Fact]
    public void IsAnswerCorrect_ShouldReturnFalseForWrongAnswer()
    {
        // Create a question where the correct answer is C.
        var question = new Question
        {
            QuestionId = 1,
            CorrectAnswer = AnswerOption.C
        };

        var gameMechanicsService = new GameMechanicsService();

        // Select a different answer than the correct answer.
        bool result = gameMechanicsService.IsAnswerCorrect(
            question,
            AnswerOption.A
        );

        // The answer should be marked as incorrect.
        Assert.False(result);
    }

    // Test for
    // QuestionId     
    // PlayerName     
    // SelectedAnswer 
    // IsCorrect       
    [Fact]
    public void CreateGameAnswer_ShouldStoreCorrectAnswerData()
    {
        // Create a question where the correct answer is B.
        var question = new Question
        {
            QuestionId = 20,
            CorrectAnswer = AnswerOption.B
        };

        var gameMechanicsService = new GameMechanicsService();

        // Create a GameAnswer where the player selects
        // the correct answer.
        GameAnswer result = gameMechanicsService.CreateGameAnswer(
            question,
            "Player 1",
            AnswerOption.B
        );

        // Check that the GameAnswer contains the correct question ID.
        Assert.Equal(20, result.QuestionId);

        // Check that the player's name was stored correctly.
        Assert.Equal("Player 1", result.PlayerName);

        // Check that the selected answer was stored correctly.
        Assert.Equal(AnswerOption.B, result.SelectedAnswer);

        // The selected answer matches the correct answer.
        Assert.True(result.IsCorrect);
    }

    // Test for ShouldMarkWrongAnswerAsIncorrect
    [Fact]
    public void CreateGameAnswer_ShouldMarkWrongAnswerAsIncorrect()
    {
        // Create a question where the correct answer is C.
        var question = new Question
        {
            QuestionId = 21,
            CorrectAnswer = AnswerOption.C
        };

        var gameMechanicsService = new GameMechanicsService();

        // The player selects A even though C is correct.
        GameAnswer result = gameMechanicsService.CreateGameAnswer(
            question,
            "Player 2",
            AnswerOption.A
        );

        // The answer should be marked as incorrect.
        Assert.False(result.IsCorrect);
    }

    // Test for wedge, only get for correctanswer:

    [Fact]
    public void AwardWedge_ShouldAddWedgeForCorrectAnswer()
    {
        // Create a player with no wedges.
        var player = new PlayerGameState
        {
            PlayerName = "Player 1"
        };

        // Create a question in the Python category.
        var question = new Question
        {
            QuestionId = 30,
            Category = "Python"
        };

        // Create a correct game answer.
        var gameAnswer = new GameAnswer
        {
            QuestionId = 30,
            IsCorrect = true
        };

        var gameMechanicsService = new GameMechanicsService();

        // Try to award the Python wedge.
        bool result = gameMechanicsService.AwardWedge(
            player,
            question,
            gameAnswer
        );

        // A new wedge should have been awarded.
        Assert.True(result);

        // The player's wedges should now contain Python.
        Assert.Contains("Python", player.Wedges);
    }

    // Test for wedge, should not get for wrong answer:

    [Fact]
    public void AwardWedge_ShouldNotAddWedgeForWrongAnswer()
    {
        // Create a player with no wedges.
        var player = new PlayerGameState
        {
            PlayerName = "Player 1"
        };

        var question = new Question
        {
            QuestionId = 31,
            Category = "Java"
        };

        // The player's answer was incorrect.
        var gameAnswer = new GameAnswer
        {
            QuestionId = 31,
            IsCorrect = false
        };

        var gameMechanicsService = new GameMechanicsService();

        bool result = gameMechanicsService.AwardWedge(
            player,
            question,
            gameAnswer
        );

        // No wedge should have been awarded.
        Assert.False(result);

        // Java should not exist in the player's wedge collection.
        Assert.DoesNotContain("Java", player.Wedges);
    }

    // Test for wedge, should not be able to have duplicate of wedges:

    [Fact]
    public void AwardWedge_ShouldNotAddDuplicateWedge()
    {
        // Create a player who already owns the C# wedge.
        var player = new PlayerGameState
        {
            PlayerName = "Player 1",
            Wedges = new HashSet<string> { "C#" }
        };

        var question = new Question
        {
            QuestionId = 32,
            Category = "C#"
        };

        var gameAnswer = new GameAnswer
        {
            QuestionId = 32,
            IsCorrect = true
        };

        var gameMechanicsService = new GameMechanicsService();

        // Try to award the same wedge again.
        bool result = gameMechanicsService.AwardWedge(
            player,
            question,
            gameAnswer
        );

        // No new wedge should have been added.
        Assert.False(result);

        // The player should still only have one wedge.
        Assert.Single(player.Wedges);
    }

    [Fact]
    public void HasWedge_ShouldReturnTrueWhenPlayerOwnsWedge()
    {
        // Create a player who already owns the Python wedge.
        var player = new PlayerGameState
        {
            PlayerName = "Player 1",
            Wedges = new HashSet<string> { "Python" }
        };

        var gameMechanicsService = new GameMechanicsService();

        // Check whether the player owns the Python wedge.
        bool result = gameMechanicsService.HasWedge(player, "Python");

        // The player owns this wedge, so the result should be true.
        Assert.True(result);
    }

    [Fact]
    public void HasWedge_ShouldReturnFalseWhenPlayerDoesNotOwnWedge()
    {
        // Create a player who owns the Java wedge.
        var player = new PlayerGameState
        {
            PlayerName = "Player 1",
            Wedges = new HashSet<string> { "Java" }
        };

        var gameMechanicsService = new GameMechanicsService();

        // Check for a wedge the player does not own.
        bool result = gameMechanicsService.HasWedge(
            player,
            "JavaScript"
        );

        // The player does not own this wedge.
        Assert.False(result);
    }

    [Fact]
    public void HasAllWedges_ShouldReturnTrueWhenPlayerHasAllWedges()
    {
        // Create a player who owns all six category wedges.
        var player = new PlayerGameState
        {
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

        var gameMechanicsService = new GameMechanicsService();

        // Check whether the player has completed the wedge collection.
        bool result = gameMechanicsService.HasAllWedges(player);

        // All six wedges are present.
        Assert.True(result);
    }

    [Fact]
    public void HasAllWedges_ShouldReturnFalseWhenWedgeIsMissing()
    {
        // Create a player who is missing the C# wedge.
        var player = new PlayerGameState
        {
            PlayerName = "Player 1",

            Wedges = new HashSet<string>
            {
                "Java",
                "JavaScript",
                "HTML/CSS",
                "Python",
                "Game History"
            }
        };

        var gameMechanicsService = new GameMechanicsService();

        // Check whether the player has collected every wedge.
        bool result = gameMechanicsService.HasAllWedges(player);

        // One required wedge is missing.
        Assert.False(result);
    }

    [Fact]
    public void GetMissingWedges_ShouldReturnOnlyMissingWedges()
    {
        // Create a player who currently owns three wedges.
        var player = new PlayerGameState
        {
            PlayerName = "Player 1",

            Wedges = new HashSet<string>
            {
                "Java",
                "Python",
                "C#"
            }
        };

        var gameMechanicsService = new GameMechanicsService();

        // Get the wedges the player still needs to collect.
        List<string> result =
            gameMechanicsService.GetMissingWedges(player);

        // The player should be missing exactly three wedges.
        Assert.Equal(3, result.Count);

        // Check that all missing wedges are included.
        Assert.Contains("JavaScript", result);
        Assert.Contains("HTML/CSS", result);
        Assert.Contains("Game History", result);

        // Check that already owned wedges are not included.
        Assert.DoesNotContain("Java", result);
        Assert.DoesNotContain("Python", result);
        Assert.DoesNotContain("C#", result);
    }

    [Fact]
    public void SelectRandomStartingPlayer_ShouldSelectValidPlayer()
    {
        // Create a game with four players.
        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                new PlayerGameState { PlayerNumber = 1, PlayerName = "Player 1" },
                new PlayerGameState { PlayerNumber = 2, PlayerName = "Player 2" },
                new PlayerGameState { PlayerNumber = 3, PlayerName = "Player 3" },
                new PlayerGameState { PlayerNumber = 4, PlayerName = "Player 4" }
            }
        };

        var gameMechanicsService = new GameMechanicsService();

        // Select the random starting player.
        gameMechanicsService.SelectRandomStartingPlayer(gameState);

        // The selected index must point to one of the players in the list.
        Assert.InRange(
            gameState.CurrentPlayerIndex,
            0,
            // because four players, the valid indices are 0, 1, 2, and 3
            gameState.Players.Count - 1
        );
    }

    [Fact]
    public void GetCurrentPlayer_ShouldReturnSelectedPlayer()
    {
        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                new PlayerGameState { PlayerNumber = 1, PlayerName = "Anna" },
                new PlayerGameState { PlayerNumber = 2, PlayerName = "Erik" },
                new PlayerGameState { PlayerNumber = 3, PlayerName = "Nora" }
            },

            // Index 1 represents Player 2.
            CurrentPlayerIndex = 1
        };

        var gameMechanicsService = new GameMechanicsService();

        // Get the player whose turn it currently is.
        PlayerGameState result =
            gameMechanicsService.GetCurrentPlayer(gameState);

        // Player 2 should be the active player.
        Assert.Equal(2, result.PlayerNumber);
        Assert.Equal("Erik", result.PlayerName);
    }

    [Fact]
    public void MoveToNextPlayer_ShouldFollowPlayerOrder()
    {
        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                new PlayerGameState { PlayerNumber = 1, PlayerName = "Anna" },
                new PlayerGameState { PlayerNumber = 2, PlayerName = "Erik" },
                new PlayerGameState { PlayerNumber = 3, PlayerName = "Nora" },
                new PlayerGameState { PlayerNumber = 4, PlayerName = "Håvard" }
            },

            // Player 2 currently has the turn.
            CurrentPlayerIndex = 1
        };

        var gameMechanicsService = new GameMechanicsService();

        // Move from Player 2 to Player 3.
        gameMechanicsService.MoveToNextPlayer(gameState);

        PlayerGameState result =
            gameMechanicsService.GetCurrentPlayer(gameState);

        Assert.Equal(3, result.PlayerNumber);
        Assert.Equal("Nora", result.PlayerName);
    }

    [Fact]
    public void MoveToNextPlayer_ShouldReturnToFirstPlayerAfterLast()
    {
        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                new PlayerGameState { PlayerNumber = 1, PlayerName = "Anna" },
                new PlayerGameState { PlayerNumber = 2, PlayerName = "Erik" },
                new PlayerGameState { PlayerNumber = 3, PlayerName = "Nora" },
                new PlayerGameState { PlayerNumber = 4, PlayerName = "Håvard" }
            },

            // Player 4 is the final player in the turn order.
            CurrentPlayerIndex = 3
        };

        var gameMechanicsService = new GameMechanicsService();

        // The next turn should return to Player 1.
        gameMechanicsService.MoveToNextPlayer(gameState);

        PlayerGameState result =
            gameMechanicsService.GetCurrentPlayer(gameState);

        Assert.Equal(1, result.PlayerNumber);
        Assert.Equal("Anna", result.PlayerName);
    }

    [Fact]
    public void MoveToNextPlayer_ShouldOnlyUseActivePlayers()
    {
        // Only player slots 1, 2 and 3 are being used.
        var gameState = new GameSessionState
        {
            Players = new List<PlayerGameState>
            {
                new PlayerGameState { PlayerNumber = 1, PlayerName = "Anna" },
                new PlayerGameState { PlayerNumber = 2, PlayerName = "Erik" },
                new PlayerGameState { PlayerNumber = 3, PlayerName = "Nora" }
            },

            CurrentPlayerIndex = 2
        };

        var gameMechanicsService = new GameMechanicsService();

        // After Player 3, the turn should return to Player 1.
        gameMechanicsService.MoveToNextPlayer(gameState);

        PlayerGameState result =
            gameMechanicsService.GetCurrentPlayer(gameState);

        Assert.Equal(1, result.PlayerNumber);
    }

    [Fact]
    public void SelectRandomStartingPlayer_ShouldThrowWhenNoPlayersExist()
    {
        // Create a game without any players.
        var gameState = new GameSessionState();

        var gameMechanicsService = new GameMechanicsService();

        // A starting player cannot be selected
        // when the game contains no players.
        Assert.Throws<InvalidOperationException>(() =>
            gameMechanicsService.SelectRandomStartingPlayer(gameState));
    }

    [Fact]
    public void MoveToNextPlayer_ShouldThrowWhenNoPlayersExist()
    {
        var gameState = new GameSessionState();

        var gameMechanicsService = new GameMechanicsService();

        // The turn cannot move forward if no players exist.
        Assert.Throws<InvalidOperationException>(() =>
            gameMechanicsService.MoveToNextPlayer(gameState));
    }

}