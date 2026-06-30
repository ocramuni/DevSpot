using DevSpot.AcceptanceTests.Fixtures;
using DevSpot.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DevSpot.AcceptanceTests.Infrastructure.Database;

/// <summary>
/// Re-runs the application's own seed logic after each reset.
/// Delegates entirely to <see cref="RoleSeeder"/> and <see cref="UserSeeder"/>
/// so there is a single source of truth for reference data.
/// </summary>
public sealed class DatabaseSeeder
{
    private readonly AcceptanceTestFixture _fixture;

    public DatabaseSeeder(AcceptanceTestFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>Seed roles and the three standard users (Admin, Employer, JobSeeker).</summary>
    public async Task SeedBaseAsync()
    {
        using var scope = _fixture.Factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        await RoleSeeder.RoleSeederAsync(services);
        await UserSeeder.SeedUserAsync(services);
    }
}
