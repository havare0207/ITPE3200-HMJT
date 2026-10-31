// Places BoardSpace in the HMJT.Models namespace.
namespace HMJT.Models;

// Represents one space on the Code Pursuit game board.
public class BoardSpace
{
    // Unique number used to identify the board space.
    public int BoardSpaceId { get; set; }

    // Defines whether this is a normal category space,
    // a Roll Again space, or the center space.
    public BoardSpaceType SpaceType { get; set; }

    // Stores the question category connected to this space.
    // Roll Again and Center spaces do not need a fixed category.
    public string? Category { get; set; }

    // Stores the IDs of the spaces directly connected
    // to this space on the board.
    public List<int> ConnectedSpaceIds { get; set; } = new();
}