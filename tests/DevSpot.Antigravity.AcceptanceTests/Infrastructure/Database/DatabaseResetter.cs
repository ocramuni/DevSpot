using DevSpot.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevSpot.Antigravity.AcceptanceTests.Infrastructure.Database
{
    public class DatabaseResetter
    {
        private readonly ApplicationDbContext _dbContext;

        public DatabaseResetter(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ResetAsync()
        {
            var tableNames = new List<string>();

            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' AND name NOT LIKE '__EFMigrationsHistory';";
                
                var wasOpen = _dbContext.Database.GetDbConnection().State == System.Data.ConnectionState.Open;
                if (!wasOpen)
                {
                    await _dbContext.Database.OpenConnectionAsync();
                }

                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tableNames.Add(reader.GetString(0));
                        }
                    }
                }
                finally
                {
                    if (!wasOpen)
                    {
                        await _dbContext.Database.CloseConnectionAsync();
                    }
                }
            }

            // Disable foreign keys temporarily to avoid delete order issues
            await _dbContext.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;");

            try
            {
                foreach (var tableName in tableNames)
                {
                    await _dbContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{tableName}\";");
                }
                
                // Reset SQLite autoincrement sequences
                await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence;");
            }
            finally
            {
                // Re-enable foreign keys
                await _dbContext.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;");
            }
        }
    }
}
