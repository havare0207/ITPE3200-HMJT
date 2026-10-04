# ITPE3200-HMJT

## Project Overview

This project is developed as part of the ITPE3200 course.

The application is a gamified coding and trivia game built with ASP.NET Core MVC. The project includes gameplay logic, player progression, question handling, custom game creation, database functionality, and automated testing.

## Technology Stack

- .NET SDK: 10.0.400
- Framework: ASP.NET Core MVC
- Language: C#
- Frontend: Razor Views, HTML, CSS, and Bootstrap
- Node.js: v24.13.1
- Database access: Entity Framework Core
- Testing framework: xUnit

## Requirements

The following software should be installed before running the project:

- .NET SDK 10.0.400
- Node.js v24.13.1
- Git
- A modern web browser

The installed versions can be checked with:

```bash
dotnet --version
node --version
```

## How to Run the Web Application

Clone the repository using Git:

```bash
git clone <REPOSITORY-URL>
```

Navigate to the project folder:

```bash
cd ITPE3200-HMJT
```

Restore the required .NET packages:

```bash
dotnet restore
```

## Run Application

From the repository root, start the application with:

```bash
dotnet run --project ./HMJT/CodePursuit.csproj
```

When the application starts, the terminal will display a local address similar to:

```text
http://localhost:xxxx
```

Open the displayed address in a web browser.

The gameplay page can be accessed through the **Play** link in the navigation menu.

## Run Tests

The automated test project is located in:

```text
HMJT.Tests
```

Run all automated tests from the repository root with:

```bash
dotnet test ./HMJT.Tests/HMJT.Tests.csproj
```

All automated tests should pass before changes are merged into the main branch or before the project is submitted.

## Database

The project uses Entity Framework Core for database functionality.

If database migrations are required in the final version of the project, they should be applied before starting the application.

Example:

```bash
dotnet ef database update --project ./HMJT/CodePursuit.csproj
```

This section should be updated before final submission with the exact database setup procedure required by the finished application.

## Gameplay

The gameplay system supports:

- 1–6 players
- Player names entered in sequential order
- Difficulty selection
- Random starting player
- Manual dice rolling
- Category selection based on dice roll
- Multiple-choice questions with A, B, C, and D answers
- Correct and incorrect answer handling
- Wedge collection for different categories
- Prevention of duplicate wedges
- Turn switching between players
- Final round after collecting all required wedges
- Final category selection by the other players
- Winner handling
- Resetting the current game

The project also includes functionality for managing questions and game sets.

## Testing

Automated tests are written with xUnit.

The tests cover important parts of the application, including:

- Dice logic
- Category selection
- Question selection
- Answer validation
- Wedge awarding
- Player turn order
- Game state
- Final-round logic
- Winner handling
- Controller and game-flow behavior
- Player setup validation
- Reset functionality
...

Additional tests may be added as more functionality is implemented.

Tests have been run throughout development to verify that the implemented functionality works as intended.

## AI Assistance and External Help

AI tools have been used during development as an assistance tool.

AI has been used to help write parts of the code line by line, suggest implementation approaches, explain code, assist with debugging, and suggest automated tests.

The group has reviewed the code line by line while implementing it and has checked that the code works as intended.

Comments have been added throughout the code to help document the implementation and to make it easier for the group to understand and remember what different parts of the code do.

Automated tests have also been run and verified throughout development.

The group is responsible for understanding, reviewing, adapting, and testing all code included in the project.

External documentation, examples, or code snippets used as inspiration should also be documented where relevant before final submission.

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
- Automated test project
- Project configuration files

The application can be started from the repository root with:

```bash
dotnet run --project ./HMJT/CodePursuit.csproj
```

The automated tests can be run with:

```bash
dotnet test ./HMJT.Tests/HMJT.Tests.csproj
```

The project should not require a specific IDE to run, as long as the required .NET SDK, Node.js version, and project dependencies are installed.

## Development Workflow

Development is done using separate Git branches.

Changes are pushed to GitHub and merged into the `main` branch through pull requests and code review.

Before creating or merging a pull request, the group should:

1. Make sure the project builds successfully.
2. Run all automated tests.
3. Review the changed files.
4. Confirm that no required files are missing.
5. Confirm that the README contains the correct setup instructions.
