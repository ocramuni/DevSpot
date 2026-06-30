using DevSpot.AcceptanceTests.Fixtures;
using DevSpot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DevSpot.AcceptanceTests.Infrastructure.Database;

/// <summary>
/// Called once per test run to verify the schema exists.
/// Migrations are applied by the app startup code in Program.cs;
/// this class exists as a named boundary for any future schema-level setup.
/// </summary>
public sealed class DatabaseInitializer
{
    private readonly AcceptanceTestFixture _fixture;

    public DatabaseInitializer(AcceptanceTestFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Ensures the schema is ready. The factory startup already ran Migrate(),
    /// so this is a no-op unless called before the factory is initialised.
    /// </summary>
    public void EnsureSchemaExists()
    {
        using var scope = _fixture.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // Migrate() is idempotent; calling it here is a safety net only.
        db.Database.Migrate();
    }
}
