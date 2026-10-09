using HMJT.Models;

namespace HMJT.Tests.Models;

public class PlayerGameStateTests
{
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
}