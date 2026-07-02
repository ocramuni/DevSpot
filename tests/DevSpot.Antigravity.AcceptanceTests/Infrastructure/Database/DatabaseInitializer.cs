using DevSpot.Data;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Antigravity.AcceptanceTests.Infrastructure.Database
{
    public class DatabaseInitializer
    {
        private readonly ApplicationDbContext _dbContext;

        public DatabaseInitializer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InitializeAsync()
        {
            await _dbContext.Database.MigrateAsync();
        }
    }
}
