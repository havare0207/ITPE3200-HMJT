using Microsoft.EntityFrameworkCore;
using HMJT.Models;

namespace HMJT.DAL;

public class GameDbContext : DbContext
{
	public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
	{
	}
	public DbSet<Game> Games { get; set; }
    public DbSet<Question> Questions { get; set; }
	public DbSet<PresetQuestion> PresetQuestion { get; set; }
}