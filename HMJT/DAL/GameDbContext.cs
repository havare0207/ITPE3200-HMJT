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
    public DbSet<Note> Notes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the many-to-many relationship between Game and PresetQuestion
        modelBuilder.Entity<Game>()
            .HasMany(g => g.PresetQuestions)
            .WithMany(pq => pq.Games)
            .UsingEntity(j => j.ToTable("GamePresetQuestions"));
    }
}