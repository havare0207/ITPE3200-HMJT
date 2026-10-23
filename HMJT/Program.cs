using HMJT.DAL;
using HMJT.Models;
using HMJT.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Identity uses Razor Pages for
// Login, Register and account pages.
builder.Services.AddRazorPages();

// M - VS Code autofilled this differently, teacher's code line in MyShop: builder.Configuration["ConnectionStrings:ItemDbContextConnection"]);
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString(
            "GameDbConnection")));

// Separate database used for registered users
// and authentication information.
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString(
            "AuthDbConnection")));

// Adds ASP.NET Core Identity.
//
// Identity securely handles:
// - user registration
// - password hashing
// - login cookies
// - failed login attempts
// - logout
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        // Email confirmation is disabled for this student project.
        // A production application should normally verify
        // that the user owns the registered email address.
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequiredLength = 10;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredUniqueChars = 4;

        options.Lockout.MaxFailedAccessAttempts = 5;

        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.Configure<PasswordHasherOptions>(
    options =>
    {
        options.IterationCount = 210_000;
    });

// Registers the game services for dependency injection.
// ASP.NET Core can then create these services automatically
// when they are required by controllers or other services.
builder.Services.AddScoped<DiceService>();
builder.Services.AddScoped<GameMechanicsService>();
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<GameTurnService>();
builder.Services.AddScoped<BoardService>();

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IPresetQuestionRepository, PresetQuestionRepository>();
builder.Services.AddScoped<INotesRepository, NotesRepository>();

builder.Services.AddSingleton<GameStateService>();


var app = builder.Build();

DBInit.Seed(app); // Not under if (!app.Environment.IsDevelopment()) so examiner can see the questions however they launch the application

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

// Reads the authentication cookie
// and identifies the logged-in user.
app.UseAuthentication();

// Applies authorization rules.
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Maps Identity pages such as
// /Identity/Account/Login
// and /Identity/Account/Register.
app.MapRazorPages();

app.Run();
