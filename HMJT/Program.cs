using HMJT.Models;
using Microsoft.EntityFrameworkCore;
using HMJT.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// M - VS Code autofilled this differently, teacher's code line in MyShop: builder.Configuration["ConnectionStrings:ItemDbContextConnection"]);
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("GameDbConnection")));
// Registers the game services for dependency injection.
// ASP.NET Core can then create these services automatically
// when they are required by controllers or other services.
builder.Services.AddScoped<DiceService>();
builder.Services.AddScoped<GameMechanicsService>();
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<GameTurnService>();
builder.Services.AddScoped<BoardService>();

builder.Services.AddSingleton<GameStateService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Status", "?code={0}");
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
