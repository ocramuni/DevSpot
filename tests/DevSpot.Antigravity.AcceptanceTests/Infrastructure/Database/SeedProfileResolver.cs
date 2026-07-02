using Reqnroll;

namespace DevSpot.Antigravity.AcceptanceTests.Infrastructure.Database
{
    public class SeedProfileResolver
    {
        private readonly DatabaseSeeder _seeder;

        public SeedProfileResolver(DatabaseSeeder seeder)
        {
            _seeder = seeder;
        }

        public async Task ResolveAndApplySeedsAsync(IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                if (tag.Equals("seed:roles", StringComparison.OrdinalIgnoreCase))
                {
                    await _seeder.SeedRolesAsync();
                }
                else if (tag.Equals("seed:users", StringComparison.OrdinalIgnoreCase))
                {
                    await _seeder.SeedUsersAsync();
                }
            }
        }
    }
}
