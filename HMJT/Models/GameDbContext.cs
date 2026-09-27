using Microsoft.EntityFrameworkCore;

namespace HMJT.Models;

public class GameDbContext : DbContext
{
	public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
	{
        Database.EnsureCreated();
	}

	public DbSet<Game> Games { get; set; }
}