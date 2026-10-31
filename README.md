# ITPE3200-HMJT

## Project Overview

This project is developed as part of the ITPE3200 course.

**Code Pursuit** is a gamified coding and trivia web application built with ASP.NET Core MVC. The application combines local multiplayer gameplay with question management, user accounts, custom games, personal notes, learning resources and automated testing.

Main features include:

- Interactive local multiplayer gameplay for 2–6 players
- Dice-based board movement
- Six coding and game-related categories
- Standard and Custom Game modes
- Difficulty selection
- Wedge collection and final-question gameplay
- Preset questions stored in JSON
- User registration, login and logout
- User-owned custom games with CRUD functionality
- Question management for custom games with CRUD functionality
- Explore Questions with multi-parameter search and content filtering
- Personal Notes with CRUD functionality
- Category learning resources
- SQLite databases with Entity Framework Core
- ASP.NET Core Identity authentication
- Ownership-based access control
- Automated testing with xUnit

The normal **Standard Game** is available without requiring login. Features connected to a personal account, such as **My Games**, question management and **Notes**, require authentication.

---

## Table of Contents

- [Technology Stack](#technology-stack)
- [Requirements](#requirements)
- [Quick Start After Cloning](#quick-start-after-cloning)
- [Database Structure](#database-structure)
- [Database Setup and Common Errors](#database-setup-and-common-errors)
- [Running the Application](#running-the-application)
- [Authentication and Access Control](#authentication-and-access-control)
- [Gameplay](#gameplay)
- [Standard Game](#standard-game)
- [Custom Games](#custom-games)
- [Explore Questions and Question Management](#explore-questions-and-question-management)
- [Notes](#notes)
- [Preset Questions](#preset-questions)
- [Categories and Learning Resources](#categories-and-learning-resources)
- [Privacy and Account Security](#privacy-and-account-security)
- [Security Measures](#security-measures)
- [Testing](#testing)
- [Dependency Security Check](#dependency-security-check)
- [Code Formatting](#code-formatting)
- [Development Workflow](#development-workflow)
- [AI Assistance and External Help](#ai-assistance-and-external-help)
- [Access for Examiners](#access-for-examiners)

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

Check installed versions with:

```bash
dotnet --version
node --version
```

If the Entity Framework command-line tool is not installed:

```bash
dotnet tool install --global dotnet-ef
```

Project dependencies are already defined in the project files and should normally be restored with:

```bash
dotnet restore
```

There is normally no need to manually reinstall individual NuGet packages after cloning the repository.

---

## Quick Start After Cloning

After cloning the repository on a new computer, the project dependencies and both databases should be prepared before running the application.

Clone the repository:

```bash
git clone <REPOSITORY-URL>
cd ITPE3200-HMJT
```

From the repository root, run:

```bash
dotnet restore

dotnet ef database update --context GameDbContext --project .\HMJT\CodePursuit.csproj

dotnet ef database update --context AuthDbContext --project .\HMJT\CodePursuit.csproj

dotnet build .\HMJT\CodePursuit.csproj

dotnet test .\HMJT.Tests\HMJT.Tests.csproj

dotnet run --project .\HMJT\CodePursuit.csproj
```

If `dotnet ef` is not available:

```bash
dotnet tool install --global dotnet-ef
```

Then run the two database update commands again.

The database update steps are especially important after a fresh clone. Features such as **Explore Questions**, **My Games**, **Manage Questions**, **Notes**, user registration and login depend on the required database tables being available.


IMPORTANT: If the browser does not trust the local HTTPS development certificate, run:

```bash
dotnet dev-certs https --trust
```

Then run the database update commands again if needed.

---

## Database Structure

The application uses two Entity Framework Core database contexts.

### GameDbContext

`GameDbContext` contains application and gameplay data, including:

- Games
- Questions
- Preset-question relationships
- Notes
- Custom-game ownership information

Custom games contain an `OwnerUserId` that identifies the registered user who owns the game.

Questions belonging to a custom game are managed through their parent game so ownership can be checked before changes are allowed.

### AuthDbContext

`AuthDbContext` is used by ASP.NET Core Identity and contains authentication-related data such as:

- Registered users
- Password hashes
- Authentication tokens
- User claims
- Login information

Authentication data is kept separate from the main game data.

Passwords are never stored as plain text. ASP.NET Core Identity stores and verifies password hashes instead.

---

## Database Setup and Common Errors

Because the project contains more than one `DbContext`, the context should be specified when running Entity Framework commands.

List game database migrations:

```bash
dotnet ef migrations list --context GameDbContext --project .\HMJT\CodePursuit.csproj
```

Update the game database:

```bash
dotnet ef database update --context GameDbContext --project .\HMJT\CodePursuit.csproj
```

List authentication database migrations:

```bash
dotnet ef migrations list --context AuthDbContext --project .\HMJT\CodePursuit.csproj
```

Update the authentication database:

```bash
dotnet ef database update --context AuthDbContext --project .\HMJT\CodePursuit.csproj
```

After cloning or pulling newer changes, local database files may be behind the migrations stored in the repository.

For example:

```text
SQLite Error 1: 'no such table: Questions'
```

This normally means the game database should be updated:

```bash
dotnet ef database update --context GameDbContext --project .\HMJT\CodePursuit.csproj
```

If Identity tables such as `AspNetUsers` are missing, update the authentication database:

```bash
dotnet ef database update --context AuthDbContext --project .\HMJT\CodePursuit.csproj
```

Developers should normally **apply existing migrations** after cloning rather than creating new migrations unless the database model has intentionally changed.

---

## Running the Application

From the repository root:

```bash
dotnet run --project .\HMJT\CodePursuit.csproj
```

The terminal will display a local address similar to:

```text
http://localhost:xxxx
```

Open the displayed address in a browser.

The main gameplay page is available through the **Play** link in the navigation menu.

---

## Authentication and Access Control

The project uses ASP.NET Core Identity for authentication.

The authentication system supports:

- User registration
- User login
- User logout
- Authentication cookies
- Password hashing
- Failed-login lockout

All registered users have the same general permission level. The application does **not** use separate roles such as administrator, teacher or student.

Instead, the application uses authentication together with **ownership-based access control**.

This means:

- Unauthenticated users can use public functionality such as the Standard Game.
- Authentication is required for personal account functionality.
- Logged-in users can manage their own resources.
- Users should not be able to edit or delete resources belonging to another user.
- Server-side ownership checks are used in addition to hiding unavailable actions in the user interface.

For example, custom games contain an `OwnerUserId`. When a user attempts to access, edit or delete a custom game, the application checks that the game belongs to the currently logged-in account.

Questions belonging to custom games are protected through ownership checks on the parent game.

---

## Gameplay

Code Pursuit supports local multiplayer gameplay for 2–6 players.

The gameplay system includes:

- Player names entered in order
- Standard and Custom Game modes
- Difficulty selection
- Random starting player
- Starter confirmation
- Manual dice rolling
- Interactive board movement
- Exact movement based on dice rolls
- Wildcard movement when rolling a 6
- Category questions based on board spaces
- Roll Again spaces
- Center category selection
- Multiple-choice answers A, B, C and D
- Correct and incorrect answer handling
- Six category wedges
- Prevention of duplicate wedges
- Turn switching
- Visual player pieces
- Multiple players on the same board space
- Final-question gameplay
- Winner handling
- Resetting the current game

The six categories are:

- Java
- JavaScript
- HTML/CSS
- Python
- Game History
- C#

A player who has collected all six wedges must return to the center and complete the final-question sequence to win.

---

## Standard Game

Standard Game is the normal game mode and does not require the user to log in.

It uses the preset Code Pursuit question set and supports the same core gameplay rules as Custom Game mode.

Standard Game:

- Uses preset questions
- Uses the selected difficulty
- Supports all six categories
- Does not depend on a user-created game
- Can be played without an account

The Standard Game question data is kept separate from user-created custom-game questions.

---

## Custom Games

Registered users can create and manage their own custom games through **My Games**.

Users can:

- Create a custom game
- View their own saved games
- Edit their own games
- Delete their own games
- Open **Manage Questions**
- Add questions to a game
- Edit questions belonging to their game
- Delete questions belonging to their game
- Select one of their saved games when starting Custom Game mode

The normal management flow is:

```text
My Games
→ Manage Questions
→ Add / Edit / Delete Questions
→ Back to My Games
```

### Custom Game Validation

Before a Custom Game starts, the application checks that:

- The user is logged in
- A custom game has been selected
- The selected game exists
- The selected game belongs to the current user
- The game contains questions
- All six required categories contain at least one question

If one or more categories are missing, the application displays an error explaining which categories still need questions.

This prevents gameplay from reaching a category where no custom question is available.

---

## Explore Questions and Question Management

The project includes question-related pages for browsing and managing question data.

**Explore Questions** is used to view available question information in the application.

Question-management functionality is also used when working with user-created custom games.

For custom games, users can:

- Open the questions belonging to one of their own games
- Add new questions
- Edit existing questions
- Delete questions
- Work with question categories
- Work with question difficulty
- Set answer alternatives
- Set the correct answer

Question CRUD operations are protected by authentication and ownership checks so a logged-in user cannot manage another user's custom-game questions simply by changing an ID in the URL.

---

## Notes

The application includes a personal **Notes** feature for registered users.

Notes allow users to store information connected to their own account.

Users can:

- View their notes
- Create notes
- Edit their own notes
- Delete their own notes

Notes depend on the game database and therefore require the `GameDbContext` database migrations to be applied after a fresh clone.

Because Notes are personal account functionality, users must be authenticated before accessing the protected Notes functionality.

---

## Preset Questions

Standard Game uses preset questions stored in:

```text
HMJT/Data/preset-questions.json
```

The JSON file is used as the question source for the Standard Game instead of relying on the previous mock-question implementation.

The preset question set contains questions for all six categories and all supported difficulty levels.

Preset questions and user-created custom-game questions are kept separate.

This allows Standard Game to use a fixed question source while Custom Game mode uses questions belonging to the selected user-created game.

---

## Categories and Learning Resources

The project includes a **Categories** page with learning resources for the six game categories.

The page contains resources for:

- Java
- JavaScript
- HTML/CSS
- Python
- C#
- Game History

The page includes links to learning resources such as W3Schools and official or authoritative documentation where appropriate.

Game History also includes timeline-style learning content and links to additional historical information.

---

## Privacy and Account Security

The application includes a **Privacy and Account Security** page explaining how registered-account information is handled.

### Account Information

ASP.NET Core Identity is used to manage registered users and login sessions.

A registered user's email address is stored so it can be used to identify the account.

### Password Security

Passwords are never stored as plain text.

ASP.NET Core Identity stores a salted password hash and verifies entered passwords against that stored hash when the user logs in.

Configured password requirements include:

- Minimum length of 10 characters
- Uppercase letters
- Lowercase letters
- Digits
- Non-alphanumeric characters
- At least 4 unique characters

The password hasher is configured with an increased iteration count.

### Login Protection

Accounts are temporarily locked after five failed login attempts.

The lockout lasts for five minutes.

This helps reduce repeated password-guessing and brute-force attempts.

### Cookies

Code Pursuit uses authentication cookies provided by ASP.NET Core Identity to keep registered users logged in.

These cookies are required for account and login functionality and are not intended for advertising or tracking.

---

## Security Measures

The project includes several application-security measures.

### Authentication and Authorization

ASP.NET Core Identity handles authentication.

Protected controllers and actions use authorization so account-specific functionality is not available to anonymous users.

Ownership checks are used to protect user-created resources.

### Anti-Forgery Protection

State-changing POST requests use ASP.NET Core anti-forgery protection where applicable.

This helps verify that submitted forms originate from the application and reduces the risk of Cross-Site Request Forgery (CSRF), where another website attempts to make a logged-in user perform an unintended action.

### Error Handling

Production exceptions are handled through a generic error page.

Technical exception details are logged on the server rather than displayed to normal users.

Status codes such as 404 are handled using a dedicated status page.

This helps prevent information such as stack traces, file paths and internal implementation details from being exposed to end users.

### HTTPS and HSTS

The application uses HTTPS redirection.

HSTS is enabled outside the Development environment to instruct compatible browsers to prefer HTTPS connections.

### Security Headers

The application adds browser security headers including:

- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `Referrer-Policy: strict-origin-when-cross-origin`

These headers help reduce risks such as MIME-type guessing, clickjacking and unnecessary referrer-information exposure.

### Database Access

Entity Framework Core is used for database access.

Application code uses the database through Entity Framework repositories and contexts rather than constructing normal database queries directly in the UI.

### Dependency Checking

NuGet dependencies can be checked for known vulnerabilities using the .NET vulnerability tools described later in this README.

At the latest verification, both the main application and the test project reported no known vulnerable packages from the configured NuGet sources.

---

## Testing

Automated tests are written with xUnit.

Run all tests from the repository root:

```bash
dotnet test .\HMJT.Tests\HMJT.Tests.csproj
```

At the latest verification:

```text
Total:   135
Passed:  135
Failed:  0
Skipped: 0
```

The automated tests cover important parts of the application, including:

- Dice logic
- Board movement
- Legal destinations
- Category handling
- Question selection
- Difficulty handling
- Answer validation
- Wedge awarding
- Prevention of duplicate wedges
- Player turn order
- Game state
- Center behavior
- Final-round logic
- Winner handling
- Reset functionality
- Controller behavior
- Game-flow behavior
- Player setup validation
- Question CRUD functionality
- Authentication-related controller behavior
- Custom-game ownership checks
- Repository behavior
- Standard and Custom Game behavior
- Positive and negative backend scenarios

All automated tests should pass before changes are merged into `main` or before final submission.

---

## Dependency Security Check

The application dependencies can be checked for known vulnerabilities using the .NET CLI.

Check the main application:

```bash
dotnet list .\HMJT\CodePursuit.csproj package --vulnerable --include-transitive
```

Check the test project:

```bash
dotnet list .\HMJT.Tests\HMJT.Tests.csproj package --vulnerable --include-transitive
```

At the latest verification, both projects reported:

```text
no vulnerable packages given the current sources
```

This check should be repeated before final submission.

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

After formatting, run the tests again:

```bash
dotnet test .\HMJT.Tests\HMJT.Tests.csproj
```

Formatting should not change application behavior.

---

## Development Workflow

Development is performed using separate Git branches.

Changes are pushed to GitHub and merged into `main` through pull requests and code review.

After pulling a newer version of `main`, developers should restore packages and update both databases:

```bash
git pull origin main

dotnet restore

dotnet ef database update --context GameDbContext --project .\HMJT\CodePursuit.csproj

dotnet ef database update --context AuthDbContext --project .\HMJT\CodePursuit.csproj
```

Before creating or merging a pull request:

1. Confirm the project builds successfully.
2. Restore any newly added packages.
3. Update both local databases to the latest migrations.
4. Run `dotnet format` if required.
5. Run all automated tests.
6. Review changed files with `git status` and `git diff`.
7. Confirm that no required project files are missing.
8. Confirm that the README setup instructions are still correct.
9. Confirm that passwords, secrets or other sensitive information have not been committed.
10. Optionally repeat the dependency vulnerability checks.

A useful final verification is:

```bash
dotnet restore

dotnet ef database update --context GameDbContext --project .\HMJT\CodePursuit.csproj

dotnet ef database update --context AuthDbContext --project .\HMJT\CodePursuit.csproj

dotnet build .\HMJT\CodePursuit.csproj

dotnet test .\HMJT.Tests\HMJT.Tests.csproj

dotnet run --project .\HMJT\CodePursuit.csproj
```

---

## AI Assistance and External Help

AI tools have been used during development as assistance tools.

AI has been used to:

- Suggest implementation approaches
- Explain code
- Assist with debugging
- Help write parts of the code
- Suggest automated tests
- Help review application structure
- Assist with documentation

The group has reviewed and adapted generated suggestions while implementing the project.

Comments have been added throughout the code to document implementation choices and help the group understand and maintain the application.

Automated tests have also been run and verified throughout development.

The group is responsible for understanding, reviewing, adapting and testing all code included in the project.

External documentation, examples and code snippets used as inspiration should also be documented where relevant before final submission.

---

## Access for Examiners

The submitted repository must be accessible to the examiner(s).

The repository contains the application source code, database migrations, configuration, Razor Views, CSS, controllers, models, services, repositories, authentication setup and automated test project.

A fresh clone should not require a specific IDE as long as the required .NET SDK, Git, Entity Framework tools and project dependencies are installed.

For the fastest setup, follow the **Quick Start After Cloning** section near the top of this README.

The expected examiner workflow is:

```bash
dotnet restore

dotnet ef database update --context GameDbContext --project .\HMJT\CodePursuit.csproj

dotnet ef database update --context AuthDbContext --project .\HMJT\CodePursuit.csproj

dotnet build .\HMJT\CodePursuit.csproj

dotnet test .\HMJT.Tests\HMJT.Tests.csproj

dotnet run --project .\HMJT\CodePursuit.csproj
```

After the application starts, open the local address displayed in the terminal.
