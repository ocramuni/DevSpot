# Software Testing Project: comparison of AI-generated BDD acceptance tests

DevSpot is a .NET 8 web application for managing job postings, with authentication and role support.

## Project Origin

This project is based on the original DevSpot repository by 33Krishna:
https://github.com/33Krishna/DevSpot

All credit for the original work goes to the original author.

## What Changed in This Version

Compared to the original project, the relevant changes are:

- Database switched from SQL Server to SQLite
- Project files reorganized to better fit a .NET solution and project directory structure

## Main Features

- Job postings management
- User authentication and authorization
- Role-based setup with seeded data

## Run Locally

1. Install .NET 8 SDK
2. Start the app:

```bash
dotnet run --project src/DevSpot/DevSpot.csproj
```

3. Open the local URL shown in the terminal

## Run Tests

The project includes unit tests in this branch and alternative BDD acceptance test implementations in dedicated AI branches.

### Available Branches

Acceptance test variants are available in:

- ai/claude
- ai/antigravity
- ai/copilot

Each branch contains Gherkin scenarios and acceptance test implementations, with different structures depending on the AI used.

### Running the Tests

Run tests in the current branch:

```bash
dotnet test
```

Run acceptance tests from a specific AI branch:

1. Switch branch:

```bash
git checkout ai/<branch-name>
```

Example:

```bash
git checkout ai/copilot
```

2. Run the test suite:

```bash
dotnet test
```

### Notes

- Acceptance tests mainly cover login and job posting creation flows.

---
*Developed for the Software Testing course at the University of Udine.*