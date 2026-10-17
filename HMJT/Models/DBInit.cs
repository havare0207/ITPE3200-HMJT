using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace HMJT.Models;

public static class DBInit // Demo 4 - Db Seeding
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        GameDbContext context = serviceScope.ServiceProvider.GetRequiredService<GameDbContext>();

        context.Database.Migrate(); // Instead of EnsureCreated, because we want to keep existing user data
        
        if (!context.PresetQuestion.Any()) // Since we have so many preset questions, we chose to load them from a JSON file instead of having lists here
        {
            var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Data", "preset-questions.json"));
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };

            var presetQuestions = JsonSerializer.Deserialize<List<PresetQuestion>>(json, options) ?? new List<PresetQuestion>();
            
            context.AddRange(presetQuestions);
            context.SaveChanges();
        }
        
    }
}
        