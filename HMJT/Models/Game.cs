namespace HMTJ.Models;

public class Game
{
    public int GameID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "NotStarted";
    public int CurrentPosition { get; set; } = 0;
}