using HMJT.DAL;
using HMJT.Models;
using HMJT.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// =====================================
// MVC AND RAZOR PAGES
// =====================================

// Adds support for MVC controllers and Razor views.
builder.Services.AddControllersWithViews();

// ASP.NET Core Identity uses Razor Pages for
// pages such as Login, Register and account management.
builder.Services.AddRazorPages();


// =====================================
// DATABASES
// =====================================

// Main game database.
//
// Contains game-related data such as:
// - games
// - questions
// - preset questions
// - notes
builder.Services.AddDbContext<GameDbContext>(
    options =>
        options.UseSqlite(
            builder.Configuration.GetConnectionString(
                "GameDbConnection"
            )
        )
);


// Separate authentication database.
//
// Authentication information is kept separate
// from normal gameplay data.
builder.Services.AddDbContext<AuthDbContext>(
    options =>
        options.UseSqlite(
            builder.Configuration.GetConnectionString(
                "AuthDbConnection"
            )
        )
);


// =====================================
// ASP.NET CORE IDENTITY
// =====================================

// Adds ASP.NET Core Identity.
//
// Identity securely handles:
// - user registration
// - password hashing
// - authentication cookies
// - failed login attempts
// - account lockout
// - logout
builder.Services
    .AddDefaultIdentity<ApplicationUser>(
        options =>
        {
            // Email confirmation is disabled for this
            // student project because no email service
            // has been configured.
            //
            // A production application would normally
            // verify that users own their email address.
            options.SignIn.RequireConfirmedAccount =
                false;


            // Require stronger passwords than the
            // default configuration.
            options.Password.RequiredLength =
                10;

            options.Password.RequireUppercase =
                true;

            options.Password.RequireLowercase =
                true;

            options.Password.RequireDigit =
                true;

            options.Password.RequireNonAlphanumeric =
                true;

            options.Password.RequiredUniqueChars =
                4;


            // Temporarily lock the account after
            // repeated failed login attempts.
            //
            // This helps reduce brute-force attacks.
            options.Lockout.MaxFailedAccessAttempts =
                5;

            options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(5);
        }
    )
    .AddEntityFrameworkStores<AuthDbContext>();


// Increase the work factor used by ASP.NET Core
// when hashing passwords.
//
// Passwords are never stored as plain text.
builder.Services.Configure<PasswordHasherOptions>(
    options =>
    {
        options.IterationCount =
            210_000;
    }
);


// =====================================
// GAME SERVICES
// =====================================

// Registers the gameplay services for
// ASP.NET Core dependency injection.
//
// Scoped services are created once for each
// HTTP request.
builder.Services.AddScoped<DiceService>();

builder.Services.AddScoped<
    GameMechanicsService>();

builder.Services.AddScoped<
    QuestionService>();

builder.Services.AddScoped<
    GameTurnService>();

builder.Services.AddScoped<
    BoardService>();


// =====================================
// REPOSITORIES
// =====================================

// Register repository implementations.
//
// Controllers depend on repository interfaces
// instead of directly accessing the database.
builder.Services.AddScoped<
    IGameRepository,
    GameRepository>();

builder.Services.AddScoped<
    IQuestionRepository,
    QuestionRepository>();

builder.Services.AddScoped<
    IPresetQuestionRepository,
    PresetQuestionRepository>();

builder.Services.AddScoped<
    INotesRepository,
    NotesRepository>();


// Stores the temporary active gameplay state.
//
// The local multiplayer game currently shares
// one in-memory game state while the application
// is running.
builder.Services.AddSingleton<
    GameStateService>();


var app = builder.Build();


// =====================================
// DATABASE INITIALIZATION
// =====================================

// Seed required database data.
//
// This is intentionally not placed only inside
// Development so the examiner can access the
// required preset data regardless of how the
// application is launched.
DBInit.Seed(app);


// =====================================
// ERROR HANDLING
// =====================================

// Detailed exception information should not
// be exposed to normal users in production.
//
// Unexpected exceptions are redirected to
// Home/Error, where a generic error message
// is displayed.
//
// Technical exception details are instead
// written to the server-side application log.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Home/Error"
    );

    // HTTP Strict Transport Security tells
    // compatible browsers to prefer HTTPS.
    app.UseHsts();
}


// Handles HTTP status codes such as 404.
//
// The request is internally re-executed using
// Home/Status while preserving the status code.
//
// The Status page shows a generic message
// instead of exposing technical implementation
// details to the user.
app.UseStatusCodePagesWithReExecute(
    "/Home/Status",
    "?code={0}"
);


// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();


// =====================================
// SECURITY HEADERS
// =====================================

// Adds additional browser security headers
// to every HTTP response.
app.Use(
    async (context, next) =>
    {
        // Prevent browsers from trying to guess
        // a different MIME/content type.
        // In other words: 
        // Tells the browser not to guess what type of file it receives.
        // For example, a file marked as an image should not suddenly
        // be treated as JavaScript or another type of content.
        context.Response.Headers[
            "X-Content-Type-Options"
        ] = "nosniff";


        // Prevent Code Pursuit from being embedded
        // inside an iframe on another website.
        // This helps protect users from clickjacking,
        // where someone tries to trick them into clicking
        // something they did not intend to click.
        context.Response.Headers[
            "X-Frame-Options"
        ] = "DENY";


        // Limits how much URL information is sent
        // through the Referer header when navigating
        // to another origin.
        // In other words: 
        // Reduces how much information about the current URL
        // is sent when the user follows a link to another website.
        context.Response.Headers[
            "Referrer-Policy"
        ] = "strict-origin-when-cross-origin";


        await next();
    }
);


// =====================================
// REQUEST PIPELINE
// =====================================

// Enables endpoint routing.
app.UseRouting();


// Reads the ASP.NET Core Identity
// authentication cookie and identifies
// the currently logged-in user.
app.UseAuthentication();


// Applies authorization rules such as
// [Authorize] attributes and ownership-based
// access checks implemented by controllers.
app.UseAuthorization();


// Serves static application assets.
app.MapStaticAssets();


// =====================================
// MVC ROUTING
// =====================================

app.MapControllerRoute(
        name: "default",
        pattern:
            "{controller=Home}/{action=Index}/{id?}"
    )
    .WithStaticAssets();


// =====================================
// IDENTITY RAZOR PAGES
// =====================================

// Maps Identity pages such as:
//
// /Identity/Account/Login
// /Identity/Account/Register
//
// Local Login/Register Razor Pages override
// the default Identity UI where required.
app.MapRazorPages();


// Start the application.
app.Run();