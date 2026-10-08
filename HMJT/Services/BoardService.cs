using HMJT.Models;

namespace HMJT.Services;

// Stores the structure of the Code Pursuit board
// and provides access to the different board spaces.
public class BoardService
{
    // Stores all spaces that exist on the board.
    private readonly List<BoardSpace> _spaces;

    // Creates the complete board when the service is created.
    public BoardService()
    {
        _spaces = CreateBoard();
    }

    // Returns all spaces on the board.
    public List<BoardSpace> GetAllSpaces()
    {
        return _spaces;
    }

    // Returns the board space with the requested ID.
    // The method searches through all board spaces and
    // returns the matching space if it exists.
    // If no space with that ID is found, an exception is thrown.
    public BoardSpace GetSpace(int boardSpaceId)
    {
        BoardSpace? space =
            _spaces.FirstOrDefault(
                space => space.BoardSpaceId == boardSpaceId
            );

        if (space == null)
        {
            throw new ArgumentOutOfRangeException(
                nameof(boardSpaceId),
                "The requested board space does not exist."
            );
        }

        return space;
    }

    // Creates the complete board structure.
    private static List<BoardSpace> CreateBoard()
    {
        var spaces = new List<BoardSpace>();

        /*
         * Outer ring
         *
         * IDs 1-24 represent the outer circular path.
         * Every fourth space is a Roll Again space.
         */

        spaces.Add(CreateCategorySpace(1, "C#"));
        spaces.Add(CreateCategorySpace(2, "Game History"));
        spaces.Add(CreateCategorySpace(3, "JavaScript"));
        spaces.Add(CreateRollAgainSpace(4));

        spaces.Add(CreateCategorySpace(5, "Python"));
        spaces.Add(CreateCategorySpace(6, "C#"));
        spaces.Add(CreateCategorySpace(7, "Java"));
        spaces.Add(CreateRollAgainSpace(8));

        spaces.Add(CreateCategorySpace(9, "HTML/CSS"));
        spaces.Add(CreateCategorySpace(10, "Game History"));
        spaces.Add(CreateCategorySpace(11, "Python"));
        spaces.Add(CreateRollAgainSpace(12));

        spaces.Add(CreateCategorySpace(13, "JavaScript"));
        spaces.Add(CreateCategorySpace(14, "Java"));
        spaces.Add(CreateCategorySpace(15, "C#"));
        spaces.Add(CreateRollAgainSpace(16));

        spaces.Add(CreateCategorySpace(17, "HTML/CSS"));
        spaces.Add(CreateCategorySpace(18, "Python"));
        spaces.Add(CreateCategorySpace(19, "Game History"));
        spaces.Add(CreateRollAgainSpace(20));

        spaces.Add(CreateCategorySpace(21, "Java"));
        spaces.Add(CreateCategorySpace(22, "HTML/CSS"));
        spaces.Add(CreateCategorySpace(23, "JavaScript"));
        spaces.Add(CreateRollAgainSpace(24));


        /*
         * Inner paths / spokes
         *
         * Each category has three spaces leading
         * from the outer ring toward the center.
         */

        // Game History path
        spaces.Add(CreateCategorySpace(25, "Game History"));
        spaces.Add(CreateCategorySpace(26, "Python"));
        spaces.Add(CreateCategorySpace(27, "Java"));

        // Java path
        spaces.Add(CreateCategorySpace(28, "Java"));
        spaces.Add(CreateCategorySpace(29, "HTML/CSS"));
        spaces.Add(CreateCategorySpace(30, "C#"));

        // Python path
        spaces.Add(CreateCategorySpace(31, "Python"));
        spaces.Add(CreateCategorySpace(32, "JavaScript"));
        spaces.Add(CreateCategorySpace(33, "Game History"));

        // C# path
        spaces.Add(CreateCategorySpace(34, "C#"));
        spaces.Add(CreateCategorySpace(35, "Java"));
        spaces.Add(CreateCategorySpace(36, "HTML/CSS"));

        // HTML/CSS path
        spaces.Add(CreateCategorySpace(37, "HTML/CSS"));
        spaces.Add(CreateCategorySpace(38, "Python"));
        spaces.Add(CreateCategorySpace(39, "JavaScript"));

        // JavaScript path
        spaces.Add(CreateCategorySpace(40, "JavaScript"));
        spaces.Add(CreateCategorySpace(41, "C#"));
        spaces.Add(CreateCategorySpace(42, "Java"));


        /*
         * Center
         *
         * The center does not have a fixed category.
         * The category is chosen after the player
         * reaches this space.
         */

        spaces.Add(
            new BoardSpace
            {
                BoardSpaceId = 43,
                SpaceType = BoardSpaceType.Center,
                Category = null
            }
        );


        /*
         * Connect the outer ring.
         *
         * Every outer space connects to the
         * previous and next space in the circle.
         */

        for (int i = 1; i <= 24; i++)
        {
            BoardSpace currentSpace =
                spaces.First(space =>
                    space.BoardSpaceId == i);

            int previousId =
                i == 1 ? 24 : i - 1;

            int nextId =
                i == 24 ? 1 : i + 1;

            currentSpace.ConnectedSpaceIds.Add(previousId);
            currentSpace.ConnectedSpaceIds.Add(nextId);
        }


        /*
         * Connect each inner path.
         *
         * The first ID is the outer-ring connection.
         * The final inner space connects to the center.
         */

        ConnectPath(
            spaces,
            outerSpaceId: 2,
            firstInnerId: 25,
            secondInnerId: 26,
            thirdInnerId: 27
        );

        ConnectPath(
            spaces,
            outerSpaceId: 6,
            firstInnerId: 28,
            secondInnerId: 29,
            thirdInnerId: 30
        );

        ConnectPath(
            spaces,
            outerSpaceId: 10,
            firstInnerId: 31,
            secondInnerId: 32,
            thirdInnerId: 33
        );

        ConnectPath(
            spaces,
            outerSpaceId: 14,
            firstInnerId: 34,
            secondInnerId: 35,
            thirdInnerId: 36
        );

        ConnectPath(
            spaces,
            outerSpaceId: 18,
            firstInnerId: 37,
            secondInnerId: 38,
            thirdInnerId: 39
        );

        ConnectPath(
            spaces,
            outerSpaceId: 22,
            firstInnerId: 40,
            secondInnerId: 41,
            thirdInnerId: 42
        );


        /*
         * Connect the six inner paths
         * to the center space.
         */

        int[] centerConnections =
        {
            27,
            30,
            33,
            36,
            39,
            42
        };

        BoardSpace center =
            spaces.First(space =>
                space.BoardSpaceId == 43);

        foreach (int connectionId in centerConnections)
        {
            center.ConnectedSpaceIds.Add(connectionId);
        }

        return spaces;
    }

    // Creates a normal colored category space.
    private static BoardSpace CreateCategorySpace(
        int id,
        string category)
    {
        return new BoardSpace
        {
            BoardSpaceId = id,
            SpaceType = BoardSpaceType.Category,
            Category = category
        };
    }

    // Creates a black Roll Again space.
    private static BoardSpace CreateRollAgainSpace(
        int id)
    {
        return new BoardSpace
        {
            BoardSpaceId = id,
            SpaceType = BoardSpaceType.RollAgain,
            Category = null
        };
    }

    // Connects one outer-ring space to
    // three inner spaces and the center.
    private static void ConnectPath(
        List<BoardSpace> spaces,
        int outerSpaceId,
        int firstInnerId,
        int secondInnerId,
        int thirdInnerId)
    {
        BoardSpace outer =
            spaces.First(space =>
                space.BoardSpaceId == outerSpaceId);

        BoardSpace first =
            spaces.First(space =>
                space.BoardSpaceId == firstInnerId);

        BoardSpace second =
            spaces.First(space =>
                space.BoardSpaceId == secondInnerId);

        BoardSpace third =
            spaces.First(space =>
                space.BoardSpaceId == thirdInnerId);

        // Outer ring <-> first inner space
        outer.ConnectedSpaceIds.Add(firstInnerId);
        first.ConnectedSpaceIds.Add(outerSpaceId);

        // First inner <-> second inner
        first.ConnectedSpaceIds.Add(secondInnerId);
        second.ConnectedSpaceIds.Add(firstInnerId);

        // Second inner <-> third inner
        second.ConnectedSpaceIds.Add(thirdInnerId);
        third.ConnectedSpaceIds.Add(secondInnerId);

        // Third inner <-> center
        third.ConnectedSpaceIds.Add(43);
    }

   // Returns all board spaces that can be reached
    // by moving exactly the requested number of steps
    // from the player's current board space.
    public List<BoardSpace> GetReachableSpaces(
        int startSpaceId,
        int numberOfSteps)
    {
        // A player cannot move a negative number of steps.
        if (numberOfSteps < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numberOfSteps),
                "The number of steps cannot be negative."
            );
        }

        // Make sure the starting space exists.
        GetSpace(startSpaceId);

        // If the player moves zero steps,
        // the only reachable space is the current space.
        if (numberOfSteps == 0)
        {
            return new List<BoardSpace>
            {
                GetSpace(startSpaceId)
            };
        }

        // Stores every possible movement path.
        // Each path keeps track of the spaces
        // visited during that route.
        var paths = new List<List<int>>
        {
            new List<int> { startSpaceId }
        };

        // Move through the board one step at a time.
        for (int step = 0; step < numberOfSteps; step++)
        {
            var nextPaths = new List<List<int>>();

            foreach (List<int> path in paths)
            {
                // The last ID in the path is the space
                // where this possible route currently ends.
                int currentSpaceId = path[^1];

                BoardSpace currentSpace =
                    GetSpace(currentSpaceId);

                foreach (int connectedSpaceId
                        in currentSpace.ConnectedSpaceIds)
                {
                    // Do not revisit a space that has already
                    // been used in the same movement path.
                    if (path.Contains(connectedSpaceId))
                    {
                        continue;
                    }

                    var newPath =
                        new List<int>(path)
                        {
                            connectedSpaceId
                        };

                    nextPaths.Add(newPath);
                }
            }

            paths = nextPaths;
        }

        // Get the final space from each possible route,
        // remove duplicate destinations and return
        // the corresponding board spaces.
        return paths
            .Select(path => path[^1])
            .Distinct()
            .Select(GetSpace)
            .ToList();
    }

    // Returns the board spaces that the player is allowed
    // to choose after rolling the dice.
    // Rolls from 1 to 5 use normal board movement.
    // A roll of 6 acts as a wildcard and allows the player
    // to choose any board space.
    public List<BoardSpace> GetLegalDestinations(
        int startSpaceId,
        int diceRoll)
    {
        // Make sure the starting space exists.
        GetSpace(startSpaceId);

        // A normal dice roll must be between 1 and 6.
        if (diceRoll < 1 || diceRoll > 6)
        {
            throw new ArgumentOutOfRangeException(
                nameof(diceRoll),
                "The dice roll must be between 1 and 6."
            );
        }

        // A roll of 6 is a wildcard.
        // The player may choose any space on the board.
        if (diceRoll == 6)
        {
            return GetAllSpaces()
                .Where(space =>
                    space.BoardSpaceId != startSpaceId)
                .ToList();
        }

        // Rolls from 1 to 5 use normal movement.
        return GetReachableSpaces(
            startSpaceId,
            diceRoll
        );
    }

}