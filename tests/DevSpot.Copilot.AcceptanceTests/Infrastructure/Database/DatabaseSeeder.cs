using DevSpot.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DevSpot.Copilot.AcceptanceTests.Infrastructure.Database;

public sealed class DatabaseSeeder
{
    public async Task SeedBaseAsync(IServiceProvider serviceProvider)
    {
        await RoleSeeder.RoleSeederAsync(serviceProvider);
        await UserSeeder.SeedUserAsync(serviceProvider);
    }
}
