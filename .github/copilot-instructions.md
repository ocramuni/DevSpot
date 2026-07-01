# GitHub Copilot Instructions

You are working inside an existing .NET 8 repository.

## Objective

Analyze the real codebase and implement a complete acceptance-testing solution for the existing ASP.NET Core application.

Create a separate acceptance test project for a .NET 8 ASP.NET Core app that uses Razor Pages / MVC classic server-rendered forms and SQLite.

## Technology requirements

- .NET 8
- Reqnroll
- xUnit
- Reqnroll.Microsoft.Extensions.DependencyInjection
- Microsoft.AspNetCore.Mvc.Testing
- SQLite in-memory
- Existing EF Core migrations
- Existing application seed logic
- HttpClient-based integration testing
- HTML parsing for form handling and antiforgery support
- No Selenium
- No Playwright
- No browser automation

## Important constraints

- First inspect the repository and identify the real application structure before writing code.
- Do not guess project names, namespaces, DbContexts, routes, migrations, seed classes, or auth setup.
- Reuse existing app code whenever possible.
- Prefer minimal, conservative changes to application code.
- Do not leave TODOs, pseudocode, placeholders, or empty methods.
- The result must compile and run with `dotnet test`.

## Application context

- The application uses ASP.NET Core with Razor Pages / MVC classic forms.
- The production database is SQLite.
- Migrations already exist.
- Seed logic already exists and should be reused.
- The goal is to test form behavior using in-process integration tests, not browser UI tests.

## Architecture decisions already made

1. Use a separate test project, preferably named `DevSpot.AcceptanceTests`.
2. Use xUnit as the runner.
3. Use Reqnroll for Gherkin BDD tests.
4. Use `Reqnroll.Microsoft.Extensions.DependencyInjection`.
5. Use `WebApplicationFactory<Program>`.
6. Use SQLite in-memory with one shared open `SqliteConnection`.
7. Apply real migrations with `Database.Migrate()`.
8. Reuse existing seed logic from the app.
9. Support anonymous and authenticated scenarios.
10. Keep step definitions thin and business-readable.

## Test design requirements

- Test Razor Pages / MVC forms through `HttpClient` against the in-memory ASP.NET Core host.
- For form POST scenarios:
  1. GET the page containing the form.
  2. Parse the HTML.
  3. Extract form action, method, hidden fields, antiforgery token, and cookies.
  4. Build a reusable form session object.
  5. Fill fields.
  6. Submit the form.
  7. Assert response, redirect, validation messages, authorization behavior, and database state.
- Hide low-level HTML and antiforgery mechanics behind helper/services.
- Do not expose selectors or antiforgery details in Gherkin steps.

## Required internal structure

Create something close to this, but adapt names and namespaces to the actual repository:

```text
tests/DevSpot.AcceptanceTests/
├─ Features/
│  ├─ JobPosting/
│  └─ Account/
├─ Steps/
│  ├─ Common/
│  ├─ Forms/
│  └─ Domain/
├─ Hooks/
├─ Fixtures/
├─ Infrastructure/
│  ├─ Hosting/
│  ├─ Database/
│  ├─ Authentication/
│  └─ Reqnroll/
├─ Support/
│  ├─ Http/
│  ├─ Html/
│  ├─ Forms/
│  ├─ State/
│  └─ Assertions/
├─ reqnroll.json
├─ appsettings.Test.json
└─ DevSpot.AcceptanceTests.csproj
```

## Core classes and responsibilities

- `AcceptanceTestFixture`
  - Owns shared infrastructure.
  - Opens and keeps alive the SQLite in-memory connection.
  - Creates the custom `WebApplicationFactory`.
  - Disposes resources cleanly.
- `CustomWebApplicationFactory`
  - Derives from `WebApplicationFactory<Program>`.
  - Replaces the app DbContext with SQLite in-memory.
  - Configures test authentication if needed.
  - Keeps app startup as close as possible to production.
- `DatabaseInitializer`
  - Applies migrations using `Database.Migrate()`.
- `DatabaseResetter`
  - Resets data state before each scenario.
- `DatabaseSeeder`
  - Reuses existing app seed code.
- `SeedProfileResolver`
  - maps tags like `@seed:roles`, `@seed:users`
- Typed scenario-scoped state classes.
- Support services for:
  - Navigation.
  - Route resolution.
  - Scenario HTTP state.
  - HTML loading and parsing.
  - Form discovery.
  - Antiforgery extraction.
  - Form session building.
  - Form filling.
  - Form submission.
  - Reusable assertions.

## Scenario lifecycle

Implement ordered Reqnroll hooks with this exact sequence:

- `BeforeScenario(Order = 0)`: Clear scenario-scoped state.
- `BeforeScenario(Order = 10)`: Reset database state.
- `BeforeScenario(Order = 20)`: Apply base seeds.
- `BeforeScenario(Order = 30)`: Apply tag-specific seeds.
- `BeforeScenario(Order = 40)`: Configure auth profile.
- `BeforeScenario(Order = 50)`: Create/configure `HttpClient` and HTTP state.
- Do not auto-load forms globally unless explicitly required.
- Forms should normally be loaded by navigation steps.
- `AfterScenario(Order = 1000)`: Failure diagnostics and cleanup.

## Tag conventions

Support at least:

- `@seed:roles`
- `@seed:users`
- `@auth:anonymous`
- `@auth:jobseeker`
- `@auth:employer`
- `@auth:admin`

## Step conventions

Keep feature files business-readable.

Use steps like:

- `Given I am on the "Contact" page`
- `When I fill the form with the following values`
- `And I submit the form`
- `Then I should see a validation error for the field "Email"`
- `Then I should be redirected to the confirmation page`

Use Gherkin tables for form values.

Keep step definitions very thin and delegate behavior to services.

## Authentication requirements

- Implement fake test authentication with configurable claims and roles.
- Default scenario should be anonymous.
- Enable authenticated behavior only through tags or explicit steps.
- Support at least `anonymous`, `user`, and `admin`.

## Initial coverage

Generate realistic initial feature files and step definitions based on actual forms found in the repository:

1. A public or semi-public form with server-side validation.
2. An authenticated form accessible only to logged-in users.
3. A valid submit with redirect and persistence.
4. An invalid submit with validation errors.
5. An unauthorized or redirect-to-login scenario.

## Workflow

1. Inspect the repository and identify:
   - Main web application project.
   - `Program`.
   - Real DbContext.
   - Migrations.
   - Seed logic.
   - Auth setup.
   - Relevant forms/pages/controllers.
   - Namespaces and conventions.
2. Produce a short implementation plan.
3. Create the acceptance test project and project references.
4. Add required packages.
5. Implement all infrastructure and support services.
6. Add realistic feature files and step definitions based on the real app.
7. Run build/tests and fix issues.
8. Summarize:
   - Files created or modified.
   - Commands run.
   - Assumptions made.
   - Final build/test status.

## Output format

- First: Short repository analysis.
- Second: Short implementation plan.
- Third: File tree of created/modified files.
- Then: Full code for all new files and modified files.
- Finally: Commands executed and final status.

Inspect the repository first, then implement the solution.
