using DevSpot.Claude.AcceptanceTests.Infrastructure.Hosting;
using Microsoft.Data.Sqlite;

namespace DevSpot.Claude.AcceptanceTests.Fixtures;

/// <summary>
/// Owns the shared test infrastructure for the entire test run.
/// One instance lives for the lifetime of all scenarios (created in [BeforeTestRun], disposed in [AfterTestRun]).
/// The SqliteConnection is kept open so the in-memory database persists between scenarios.
/// </summary>
public sealed class AcceptanceTestFixture : IDisposable
{
    private bool _initialized;

    public SqliteConnection SharedConnection { get; private set; } = null!;
    public CustomWebApplicationFactory Factory { get; private set; } = null!;

    public void Initialize()
    {
        if (_initialized) return;

        SharedConnection = new SqliteConnection("Data Source=:memory:");
        SharedConnection.Open();

        Factory = new CustomWebApplicationFactory(SharedConnection);

        // Accessing Services triggers app startup (including Migrate + seed).
        _ = Factory.Services;

        _initialized = true;
    }

    public void Dispose()
    {
        Factory?.Dispose();
        SharedConnection?.Dispose();
    }
}
