using DevSpot.Claude.AcceptanceTests.Fixtures;
using DevSpot.Claude.AcceptanceTests.Infrastructure.Database;
using DevSpot.Claude.AcceptanceTests.Infrastructure.Reqnroll;
using DevSpot.Claude.AcceptanceTests.Support.Http;
using DevSpot.Claude.AcceptanceTests.Support.State;
using Reqnroll;

namespace DevSpot.Claude.AcceptanceTests.Hooks;

/// <summary>
/// Lifecycle hooks for each test run and each scenario.
///
/// Test-run lifecycle (static):
///   [BeforeTestRun]  – create the shared fixture (factory + SQLite connection)
///   [AfterTestRun]   – dispose the shared fixture
///
/// Scenario lifecycle (instance, in execution order):
///   Order =  0  – (implicit) Reqnroll MSDI creates fresh scoped services
///   Order = 10  – reset database
///   Order = 20  – apply base seeds (roles + users)
///   Order = 30  – apply tag-specific seeds
///   Order = 40  – capture auth profile from scenario tags into ScenarioState
///   Order = 50  – initialize HttpClient; perform login if auth profile is set
///   Order = 1000 (AfterScenario) – diagnostics on failure
/// </summary>
[Binding]
public sealed class ScenarioHooks
{
    private readonly ScenarioContext _scenarioContext;
    private readonly ScenarioState _state;
    private readonly DatabaseResetter _resetter;
    private readonly DatabaseSeeder _seeder;
    private readonly HttpClientSession _httpSession;
    private readonly AcceptanceTestFixture _fixture;

    public ScenarioHooks(
        ScenarioContext scenarioContext,
        ScenarioState state,
        DatabaseResetter resetter,
        DatabaseSeeder seeder,
        HttpClientSession httpSession,
        AcceptanceTestFixture fixture)
    {
        _scenarioContext = scenarioContext;
        _state = state;
        _resetter = resetter;
        _seeder = seeder;
        _httpSession = httpSession;
        _fixture = fixture;
    }

    // ── Test-run hooks ────────────────────────────────────────────────────────

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        var fixture = new AcceptanceTestFixture();
        fixture.Initialize();
        DependencyRegistration.SharedFixture = fixture;
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        DependencyRegistration.SharedFixture?.Dispose();
    }

    // ── Scenario hooks ────────────────────────────────────────────────────────

    [BeforeScenario(Order = 10)]
    public async Task ResetDatabaseAsync()
    {
        await _resetter.ResetAsync();
    }

    [BeforeScenario(Order = 20)]
    public async Task SeedBaseDataAsync()
    {
        await _seeder.SeedBaseAsync();
    }

    [BeforeScenario(Order = 30)]
    public Task ApplyTagSeedsAsync()
    {
        // Tag-based seed profiles are resolved here.
        // The base seed already covers @seed:roles and @seed:users.
        // Future @seed:jobpostings etc. would be handled here.
        return Task.CompletedTask;
    }

    [BeforeScenario(Order = 40)]
    public Task CaptureAuthProfileAsync()
    {
        _state.AuthProfile = _scenarioContext.ScenarioInfo.Tags
            .FirstOrDefault(t => t.StartsWith("auth:", StringComparison.OrdinalIgnoreCase))
            ?.Substring("auth:".Length);

        return Task.CompletedTask;
    }

    [BeforeScenario(Order = 50)]
    public async Task InitializeHttpClientAsync()
    {
        _httpSession.Initialize(_fixture.Factory);

        if (_state.AuthProfile is not null)
            await _httpSession.LoginAsync(_state.AuthProfile);
    }

    [AfterScenario(Order = 1000)]
    public void DiagnoseOnFailure()
    {
        if (_scenarioContext.TestError is null)
            return;

        Console.Error.WriteLine($"[AccTest] Scenario FAILED: {_scenarioContext.ScenarioInfo.Title}");

        if (_state.LastResponse is not null)
            Console.Error.WriteLine($"[AccTest] Last HTTP status: {_state.LastStatusCode}");

        if (!string.IsNullOrEmpty(_state.LastResponseContent))
        {
            const int maxChars = 2000;
            var excerpt = _state.LastResponseContent.Length > maxChars
                ? _state.LastResponseContent[..maxChars] + "…"
                : _state.LastResponseContent;
            Console.Error.WriteLine($"[AccTest] Last response body:\n{excerpt}");
        }
    }
}
