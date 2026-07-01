using DevSpot.Data;

namespace DevSpot.AcceptanceTests.Infrastructure.Database
{
    public class DatabaseSeeder
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SeedRolesAsync()
        {
            await RoleSeeder.RoleSeederAsync(_serviceProvider);
        }

        public async Task SeedUsersAsync()
        {
            await UserSeeder.SeedUserAsync(_serviceProvider);
        }
    }
}
