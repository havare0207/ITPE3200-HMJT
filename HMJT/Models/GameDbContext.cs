using Microsoft.EntityFrameworkCore;

namespace HMJT.Models;

public class GameDbContext : DbContext
{
	public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
	{
	}
	public DbSet<Game> Games { get; set; }
    public DbSet<Question> Questions { get; set; }
}