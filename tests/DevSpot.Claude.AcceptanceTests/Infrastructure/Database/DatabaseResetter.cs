using DevSpot.Claude.AcceptanceTests.Fixtures;
using DevSpot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DevSpot.Claude.AcceptanceTests.Infrastructure.Database;

/// <summary>
/// Restores a clean database state before each scenario.
/// Deletes all rows in FK-safe order, leaving the schema intact.
/// After this, DatabaseSeeder re-populates base reference data.
/// </summary>
public sealed class DatabaseResetter
{
    private readonly AcceptanceTestFixture _fixture;

    public DatabaseResetter(AcceptanceTestFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task ResetAsync()
    {
        using var scope = _fixture.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Delete in FK dependency order to avoid constraint violations.
        // SQLite FK enforcement is ON (set by EF Core's SQLite provider).
        await db.Database.ExecuteSqlRawAsync("DELETE FROM \"JobPosting\"");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM \"AspNetUserTokens\"");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM \"AspNetUserLogins\"");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM \"AspNetUserClaims\"");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM \"AspNetUserRoles\"");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM \"AspNetUsers\"");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM \"AspNetRoleClaims\"");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM \"AspNetRoles\"");
    }
}
