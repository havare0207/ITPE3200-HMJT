using HMJT.Models;

namespace HMJT.Tests.Models;

public class BoardMoveStateTests
{
    // Tests that a board movement can store
    // the dice roll and legal destinations.
    [Fact]
    public void BoardMoveState_ShouldStoreMovementInformation()
    {
        // Arrange and Act
        var moveState = new BoardMoveState
        {
            DiceRoll = 4,

            LegalDestinationIds = new List<int>
            {
                5,
                10,
                15
            },

            IsWaitingForMove = true
        };

        // Assert
        Assert.Equal(
            4,
            moveState.DiceRoll
        );

        Assert.Equal(
            3,
            moveState.LegalDestinationIds.Count
        );

        Assert.Contains(
            10,
            moveState.LegalDestinationIds
        );

        Assert.True(
            moveState.IsWaitingForMove
        );
    }

    // Tests that a new movement state starts
    // without any legal destinations.
    [Fact]
    public void BoardMoveState_ShouldStartWithEmptyDestinationList()
    {
        // Arrange and Act
        var moveState = new BoardMoveState();

        // Assert
        Assert.Empty(
            moveState.LegalDestinationIds
        );

        Assert.False(
            moveState.IsWaitingForMove
        );
    }    


}