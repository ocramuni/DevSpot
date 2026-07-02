using DevSpot.Claude.AcceptanceTests.Fixtures;
using DevSpot.Claude.AcceptanceTests.Infrastructure.Database;
using DevSpot.Claude.AcceptanceTests.Support.Assertions;
using DevSpot.Claude.AcceptanceTests.Support.Forms;
using DevSpot.Claude.AcceptanceTests.Support.Http;
using DevSpot.Claude.AcceptanceTests.Support.State;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace DevSpot.Claude.AcceptanceTests.Infrastructure.Reqnroll;

/// <summary>
/// Configures the DI container for each scenario.
/// [ScenarioDependencies] is called once per scenario; services registered as
/// AddScoped() get a fresh instance per scenario.
/// The AcceptanceTestFixture is registered as a singleton pointing at the
/// static instance that lives for the entire test run.
/// </summary>
[Binding]
public sealed class DependencyRegistration
{
    // Set by [BeforeTestRun] in ScenarioHooks; shared for the full test run.
    internal static AcceptanceTestFixture SharedFixture { get; set; } = null!;

    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();

        // ── Shared (test-run scoped) ─────────────────────────────────────────
        services.AddSingleton(SharedFixture);

        // ── Scenario-scoped ─────────────────────────────────────────────────
        services.AddScoped<ScenarioState>();
        services.AddScoped<HttpClientSession>();
        services.AddScoped<FormSession>();
        services.AddScoped<DatabaseResetter>();
        services.AddScoped<DatabaseSeeder>();
        services.AddScoped<HttpAssertions>();

        return services;
    }
}
