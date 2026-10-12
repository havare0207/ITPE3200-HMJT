using HMJT.Models;
using HMJT.Services;

namespace HMJT.Tests.Services;

public class BoardServiceTests
{
    // Tests that the board contains the expected
    // total number of spaces.
    [Fact]
    public void GetAllSpaces_ShouldReturn43Spaces()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        List<BoardSpace> spaces =
            boardService.GetAllSpaces();

        // Assert
        Assert.Equal(43, spaces.Count);
    }


    // Tests that the board contains exactly
    // six Roll Again spaces.
    [Fact]
    public void GetAllSpaces_ShouldContainSixRollAgainSpaces()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        List<BoardSpace> rollAgainSpaces =
            boardService
                .GetAllSpaces()
                .Where(space =>
                    space.SpaceType ==
                    BoardSpaceType.RollAgain)
                .ToList();

        // Assert
        Assert.Equal(6, rollAgainSpaces.Count);
    }


    // Tests that the board contains exactly
    // one center space.
    [Fact]
    public void GetAllSpaces_ShouldContainOneCenterSpace()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        List<BoardSpace> centerSpaces =
            boardService
                .GetAllSpaces()
                .Where(space =>
                    space.SpaceType ==
                    BoardSpaceType.Center)
                .ToList();

        // Assert
        Assert.Single(centerSpaces);

        Assert.Equal(
            43,
            centerSpaces[0].BoardSpaceId
        );
    }


    // Tests that GetSpace returns the board
    // space with the requested ID.
    [Fact]
    public void GetSpace_ShouldReturnCorrectSpace()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        BoardSpace space =
            boardService.GetSpace(1);

        // Assert
        Assert.Equal(
            1,
            space.BoardSpaceId
        );

        Assert.Equal(
            BoardSpaceType.Category,
            space.SpaceType
        );

        Assert.Equal(
            "C#",
            space.Category
        );
    }


    // Tests that requesting a board space
    // that does not exist throws an exception.
    [Fact]
    public void GetSpace_InvalidId_ShouldThrowException()
    {
        // Arrange
        var boardService = new BoardService();

        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => boardService.GetSpace(100)
        );
    }


    // Tests that the outer ring connects
    // back to itself and forms a complete circle.
    [Fact]
    public void OuterRing_FirstSpace_ShouldConnectToPreviousAndNextSpace()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        BoardSpace space =
            boardService.GetSpace(1);

        // Assert
        Assert.Contains(
            24,
            space.ConnectedSpaceIds
        );

        Assert.Contains(
            2,
            space.ConnectedSpaceIds
        );
    }


    // Tests that an outer-ring space connected
    // to a spoke can also move into that spoke.
    [Fact]
    public void OuterRingSpace_ShouldConnectToInnerPath()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        BoardSpace space =
            boardService.GetSpace(2);

        // Assert
        Assert.Contains(
            25,
            space.ConnectedSpaceIds
        );
    }


    // Tests that the spaces on an inner path
    // are connected in both directions.
    [Fact]
    public void InnerPath_ShouldConnectBothDirections()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        BoardSpace firstInnerSpace =
            boardService.GetSpace(25);

        BoardSpace secondInnerSpace =
            boardService.GetSpace(26);

        // Assert
        Assert.Contains(
            26,
            firstInnerSpace.ConnectedSpaceIds
        );

        Assert.Contains(
            25,
            secondInnerSpace.ConnectedSpaceIds
        );
    }


    // Tests that the final space on a spoke
    // connects to the center of the board.
    [Fact]
    public void InnerPath_LastSpace_ShouldConnectToCenter()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        BoardSpace space =
            boardService.GetSpace(27);

        // Assert
        Assert.Contains(
            43,
            space.ConnectedSpaceIds
        );
    }


    // Tests that the center connects back to
    // all six inner paths on the board.
    [Fact]
    public void Center_ShouldConnectToAllSixInnerPaths()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        BoardSpace center =
            boardService.GetSpace(43);

        // Assert
        Assert.Equal(
            6,
            center.ConnectedSpaceIds.Count
        );

        Assert.Contains(27, center.ConnectedSpaceIds);
        Assert.Contains(30, center.ConnectedSpaceIds);
        Assert.Contains(33, center.ConnectedSpaceIds);
        Assert.Contains(36, center.ConnectedSpaceIds);
        Assert.Contains(39, center.ConnectedSpaceIds);
        Assert.Contains(42, center.ConnectedSpaceIds);
    }

    // Tests that moving zero steps keeps
    // the player on the current board space.
    [Fact]
    public void GetReachableSpaces_ZeroSteps_ShouldReturnCurrentSpace()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        List<BoardSpace> spaces =
            boardService.GetReachableSpaces(
                43,
                0
            );

        // Assert
        Assert.Single(spaces);
        Assert.Equal(
            43,
            spaces[0].BoardSpaceId
        );
    }

    // Tests that one step from the center
    // reaches the six spaces connected to it.
    [Fact]
    public void GetReachableSpaces_OneStepFromCenter_ShouldReturnSixSpaces()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        List<BoardSpace> spaces =
            boardService.GetReachableSpaces(
                43,
                1
            );

        // Assert
        Assert.Equal(
            6,
            spaces.Count
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 27
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 30
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 33
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 36
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 39
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 42
        );
    }

    // Tests that moving three steps from the center
    // reaches the first space on each inner path.
    [Fact]
    public void GetReachableSpaces_ThreeStepsFromCenter_ShouldReachFirstInnerSpaces()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        List<BoardSpace> spaces =
            boardService.GetReachableSpaces(
                43,
                3
            );

        // Assert
        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 25
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 28
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 31
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 34
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 37
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 40
        );
    }

    // Tests that a negative number of steps
    // is rejected.
    [Fact]
    public void GetReachableSpaces_NegativeSteps_ShouldThrowException()
    {
        // Arrange
        var boardService = new BoardService();

        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () =>
                boardService.GetReachableSpaces(
                    43,
                    -1
                )
        );
    }

    // Tests that a normal dice roll uses
    // the regular movement rules.
    [Fact]
    public void GetLegalDestinations_RollThree_ShouldUseNormalMovement()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        List<BoardSpace> spaces =
            boardService.GetLegalDestinations(
                43,
                3
            );

        // Assert
        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 25
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 28
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 31
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 34
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 37
        );

        Assert.Contains(
            spaces,
            space => space.BoardSpaceId == 40
        );
    }

    // Tests that rolling a 6 allows the player
    // to choose any other space on the board.
    [Fact]
    public void GetLegalDestinations_RollSix_ShouldReturnAllOtherSpaces()
    {
        // Arrange
        var boardService = new BoardService();

        // Act
        List<BoardSpace> spaces =
            boardService.GetLegalDestinations(
                43,
                6
            );

        // Assert
        Assert.Equal(
            42,
            spaces.Count
        );

        Assert.DoesNotContain(
            spaces,
            space => space.BoardSpaceId == 43
        );
    }

    // Tests that dice rolls below 1 are rejected.
    [Fact]
    public void GetLegalDestinations_RollBelowOne_ShouldThrowException()
    {
        // Arrange
        var boardService = new BoardService();

        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () =>
                boardService.GetLegalDestinations(
                    43,
                    0
                )
        );
    }

    // Tests that dice rolls above 6 are rejected.
    [Fact]
    public void GetLegalDestinations_RollAboveSix_ShouldThrowException()
    {
        // Arrange
        var boardService = new BoardService();

        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () =>
                boardService.GetLegalDestinations(
                    43,
                    7
                )
        );
    }

}