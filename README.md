# ITPE3200-HMJT

## Project Overview

This project is developed as part of the ITPE3200 course.

Code Pursuit is a gamified coding and trivia game built with ASP.NET Core MVC.

The application includes:

- Interactive local multiplayer gameplay
- 2–6 players
- Player progression and wedge collection
- Board movement and dice mechanics
- Question handling
- Difficulty selection
- Standard and Custom Game modes
- Preset questions
- User-owned custom games
- Custom question management
- SQLite database functionality
- User registration, login and logout
- Secure password hashing through ASP.NET Core Identity
- Server-side ownership checks for custom games
- Automated testing with xUnit
- Responsive Razor Views
- Learning resources for the six game categories

Registered users can create their own custom games, manage questions for those games and select one of their saved games when starting a Custom Game.

The normal Standard Game remains available without requiring login.

---

## Technology Stack

- .NET SDK: 10.0.400
- Framework: ASP.NET Core MVC
- Language: C#
- Frontend: Razor Views, HTML, CSS and Bootstrap
- Node.js: v24.13.1
- Database access: Entity Framework Core
- Database: SQLite
- Authentication: ASP.NET Core Identity
- Testing framework: xUnit

---

## Requirements

The following software should be installed before running the project:

- .NET SDK 10.0.400
- Node.js v24.13.1
- Git
- Entity Framework Core command-line tools
- A modern web browser

The installed versions can be checked with:

```bash
dotnet --version
node --version
```

If the Entity Framework command-line tool is not installed:

```bash
dotnet tool install --global dotnet-ef
```

---

## Required NuGet Packages

Restore all packages included in the project with:

```bash
dotnet restore
```

Authentication uses ASP.NET Core Identity.

The project includes the required Identity packages:

```bash
dotnet add .\HMJT\CodePursuit.csproj package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add .\HMJT\CodePursuit.csproj package Microsoft.AspNetCore.Identity.UI
```

Normally these packages do not need to be installed manually after cloning the repository because they are already included in the project file.

The project also uses Entity Framework Core with SQLite.

---

## Clone the Repository

Clone the repository:

```bash
git clone <REPOSITORY-URL>
```

Navigate to the project folder:

```bash
cd ITPE3200-HMJT
```

Restore packages:

```bash
dotnet restore
```

---

## Database Structure

The application uses two Entity Framework Core database contexts.

### GameDbContext

`GameDbContext` contains game-related data, including:

- Games
- Questions
- Preset questions
- Notes
- Game / preset-question relationships
- Ownership information for custom games

Custom games contain an `OwnerUserId` that identifies which registered user owns the game.

Questions belonging to custom games are managed through their parent game.

### AuthDbContext

`AuthDbContext` is used by ASP.NET Core Identity and contains authentication-related information such as:

- Registered users
- Password hashes
- Authentication tokens
- User claims
- Login information

Passwords are never stored as plain text.

ASP.NET Core Identity handles password hashing, salting and password verification.

The authentication data is kept separate from the main game data through its own database context.

---

## Database Setup

Because the project contains more than one `DbContext`, the context should be specified when running Entity Framework commands.

### Game Database

List game database migrations:

```bash
dotnet ef migrations list --context GameDbContext --project .\HMJT\CodePursuit.csproj
```

Update the game database:

```bash
dotnet ef database update --context GameDbContext --project .\HMJT\CodePursuit.csproj
```

### Authentication Database

List authentication migrations:

```bash
dotnet ef migrations list --context AuthDbContext --project .\HMJT\CodePursuit.csproj
```

Update the authentication database:

```bash
dotnet ef database update --context AuthDbContext --project .\HMJT\CodePursuit.csproj
```

Developers should normally not create a new migration after cloning the repository if the required migration already exists.

Instead, apply the existing migrations using `dotnet ef database update`.

When a new migration is intentionally required during development, it can be created by specifying the correct context.

For example:

```bash
dotnet ef migrations add AddAuthentication --context AuthDbContext --project .\HMJT\CodePursuit.csproj
```

The same principle applies to `GameDbContext`.

---

## Common Database Errors

After cloning the repository or pulling new changes, the local databases may be behind the migrations stored in the project.

For example:

```text
SQLite Error 1: 'no such table: Questions'
```

Update the game database with:

```bash
dotnet ef database update --context GameDbContext --project .\HMJT\CodePursuit.csproj
```

If authentication tables are missing, update the authentication database:

```bash
dotnet ef database update --context AuthDbContext --project .\HMJT\CodePursuit.csproj
```

---

## Run Application

From the repository root:

```bash
dotnet run --project .\HMJT\CodePursuit.csproj
```

When the application starts, the terminal displays a local address similar to:

```text
http://localhost:xxxx
```

Open the displayed address in a browser.

The gameplay page is available through the **Play** link in the navigation menu.

---

## Recommended Setup After Cloning

A typical first-time setup is:

```bash
dotnet restore

dotnet ef database update --context GameDbContext --project .\HMJT\CodePursuit.csproj

dotnet ef database update --context AuthDbContext --project .\HMJT\CodePursuit.csproj

dotnet build .\HMJT\CodePursuit.csproj

dotnet test .\HMJT.Tests\HMJT.Tests.csproj

dotnet run --project .\HMJT\CodePursuit.csproj
```

If `dotnet ef` is not installed:

```bash
dotnet tool install --global dotnet-ef
```

Then run the database commands again.

---

## Authentication

The project uses ASP.NET Core Identity for authentication.

The authentication system supports:

- User registration
- User login
- User logout
- Secure authentication cookies
- Secure password hashing
- Failed-login lockout
- User-specific custom games

Passwords are not stored directly in the database.

ASP.NET Core Identity stores a salted password hash and verifies entered passwords against that hash.

The application requires passwords containing:

- At least 10 characters
- Uppercase letters
- Lowercase letters
- Digits
- Non-alphanumeric characters

The application also uses account lockout protection after repeated unsuccessful login attempts.

Authentication is required for managing and playing user-created custom games.

The normal Standard Game does not require authentication.

---

## Gameplay

The gameplay system supports:

- 2–6 local players
- Player names entered in sequential order
- Standard Game and Custom Game modes
- Difficulty selection
- Random starting player
- Starter confirmation screen
- Manual dice rolling
- Interactive board movement
- Exact movement based on dice rolls
- Wildcard movement when rolling a 6
- Category questions based on the board space landed on
- Roll Again spaces
- Center category selection
- Multiple-choice questions with A, B, C and D answers
- Correct and incorrect answer handling
- Six category wedges
- Prevention of duplicate wedges
- Turn switching between players
- Visual player pieces on the board
- Multiple players occupying the same board space
- Final question after collecting all six wedges and returning to the center
- Final category selection by the other players
- Winner handling
- Resetting the current game

The six categories are:

- Java
- JavaScript
- HTML/CSS
- Python
- Game History
- C#

---

## Standard Game

Standard Game uses the normal Code Pursuit question source.

Standard Game:

- Does not require login
- Uses the normal game question set
- Uses the selected difficulty
- Supports all normal Code Pursuit gameplay rules

Standard Game remains separate from user-created Custom Games.

---

## Custom Games

Registered users can create and manage their own custom games.

The **My Games** page shows the custom games belonging to the currently logged-in user.

Users can:

- Create custom games
- Edit their own custom games
- Delete their own custom games
- Open **Manage Questions**
- Add questions to their custom games
- Edit questions
- Delete questions
- Select one of their saved custom games when starting gameplay

Each saved custom game contains an `OwnerUserId`.

Server-side ownership checks ensure that users can only access games that belong to their own account.

This means a user cannot access another user's custom game simply by changing an ID in the URL.

Question-management actions also verify ownership of the parent game.

The question management flow is:

```text
My Games
→ Manage Questions
→ Add / Edit / Delete Questions
→ Back to My Games
```

The previous intermediate Details page is no longer required for the normal question-management flow.

---

## Custom Game Mode

Custom Game mode is available from the Play setup page.

To use Custom Game mode:

1. The user must be logged in.
2. The user must have at least one saved custom game.
3. The user selects one of their own custom games.
4. The game uses questions belonging to that custom game.

The selected game mode, custom game ID and custom questions are stored in the temporary game state while the game is active.

Custom Game mode does not use another user's custom games.

---

## Custom Game Validation

A Custom Game is validated before gameplay starts.

The game cannot start if:

- The user is not logged in
- No custom game has been selected
- The selected game cannot be found
- The selected game does not belong to the logged-in user
- The selected custom game contains no questions
- One or more required categories contain no questions

All six categories must contain at least one question:

- Java
- JavaScript
- HTML/CSS
- Python
- Game History
- C#

If one or more categories are missing, the user receives an error message explaining which categories need questions.

This prevents the player from reaching a category during gameplay where no custom question is available.

---

## Preset Questions

The Standard Game uses the normal Code Pursuit question data.

Preset questions and user-created custom questions are kept separate.

Custom Game mode uses questions belonging to the selected custom game rather than another user's questions.

---

## Categories and Learning Resources

The project includes a Categories page for the six game categories.

It contains:

- Java learning resources
- JavaScript learning resources
- HTML/CSS learning resources
- Python learning resources
- C# learning resources
- Game History resources
- Links to W3Schools
- Links to official or authoritative documentation
- A short game-history timeline
- A link to a more extensive game-history timeline

---

## Run Tests

The automated test project is located in:

```text
HMJT.Tests
```

Run all tests from the repository root:

```bash
dotnet test .\HMJT.Tests\HMJT.Tests.csproj
```

All automated tests should pass before changes are merged into `main` or before the project is submitted.

---

## Testing

Automated tests are written with xUnit.

The current automated test suite contains:

```text
Total:   104
Passed:  104
Failed:  0
Skipped: 0
```

The tests cover important parts of the application, including:

- Dice logic
- Board movement
- Legal destinations
- Category handling
- Question selection
- Answer validation
- Wedge awarding
- Prevention of duplicate wedges
- Player turn order
- Game state
- Center behavior
- Final-round logic
- Winner handling
- Controller and game-flow behavior
- Player setup validation
- Reset functionality
- Question CRUD functionality
- Authenticated question-controller behavior
- Custom-game ownership checks used by question management
- Positive and negative backend scenarios

The controller tests were updated after authentication and ownership checking were added.

`QuestionControllerTests` simulate an authenticated user and create test games owned by that user so that the controller can perform the same ownership checks used by the application.

`GameplayControllerTests` were updated for the new controller dependencies and game-mode parameters.

All 104 tests passed after the latest changes.

---

## Code Formatting

The project can be formatted using the built-in .NET formatter.

Format the main project:

```bash
dotnet format .\HMJT\CodePursuit.csproj
```

Format the test project:

```bash
dotnet format .\HMJT.Tests\HMJT.Tests.csproj
```

After formatting, the automated tests should be run again:

```bash
dotnet test .\HMJT.Tests\HMJT.Tests.csproj
```

Formatting should not change application behavior.

---

## AI Assistance and External Help

AI tools have been used during development as an assistance tool.

AI has been used to:

- Suggest implementation approaches
- Explain code
- Assist with debugging
- Help write parts of the code
- Suggest automated tests
- Help review application structure
- Assist with documentation

The group has reviewed and adapted the generated suggestions while implementing the project.

Comments have been added throughout the code to document the implementation and help the group understand and maintain it.

Automated tests have also been run and verified throughout development.

The group is responsible for understanding, reviewing, adapting and testing all code included in the project.

External documentation, examples and code snippets used as inspiration should also be documented where relevant before final submission.

---

## Access for Examiners

The submitted repository must be accessible to the examiner(s).

The repository contains:

- Application source code
- CSS files
- MVC controllers
- Models
- Services
- Razor Views
- Database-related code
- Entity Framework migrations
- Authentication configuration
- Automated test project
- Project configuration files

After cloning the repository, restore dependencies:

```bash
dotnet restore
```

Update the game database:

```bash
dotnet ef database update --context GameDbContext --project .\HMJT\CodePursuit.csproj
```

Update the authentication database:

```bash
dotnet ef database update --context AuthDbContext --project .\HMJT\CodePursuit.csproj
```

Build the project:

```bash
dotnet build .\HMJT\CodePursuit.csproj
```

Run the automated tests:

```bash
dotnet test .\HMJT.Tests\HMJT.Tests.csproj
```

Start the application:

```bash
dotnet run --project .\HMJT\CodePursuit.csproj
```

The project should not require a specific IDE as long as the required .NET SDK, Git, Entity Framework tools and project dependencies are installed.

---

## Development Workflow

Development is done using separate Git branches.

Changes are pushed to GitHub and merged into the `main` branch through pull requests and code review.

After pulling a newer version of `main`, developers should restore packages and update both databases:

```bash
git pull origin main

dotnet restore

dotnet ef database update --context GameDbContext --project .\HMJT\CodePursuit.csproj

dotnet ef database update --context AuthDbContext --project .\HMJT\CodePursuit.csproj
```

Before creating or merging a pull request, the group should:

1. Make sure the project builds successfully.
2. Restore any newly added packages.
3. Update the local databases to the latest migrations.
4. Run `dotnet format` if required.
5. Run all automated tests.
6. Review the changed files with `git status` and `git diff`.
7. Confirm that no required files are missing.
8. Confirm that the README contains the correct setup instructions.
9. Confirm that local database files, passwords, secrets or other sensitive information have not been committed.

A recommended final check is:

```bash
dotnet restore

dotnet ef database update --context GameDbContext --project .\HMJT\CodePursuit.csproj

dotnet ef database update --context AuthDbContext --project .\HMJT\CodePursuit.csproj

dotnet format .\HMJT\CodePursuit.csproj

dotnet format .\HMJT.Tests\HMJT.Tests.csproj

dotnet build .\HMJT\CodePursuit.csproj

dotnet test .\HMJT.Tests\HMJT.Tests.csproj

dotnet run --project .\HMJT\CodePursuit.csproj
```