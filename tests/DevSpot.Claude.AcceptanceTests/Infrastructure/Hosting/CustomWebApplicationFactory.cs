using DevSpot.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DevSpot.Claude.AcceptanceTests.Infrastructure.Hosting;

/// <summary>
/// Replaces the application DbContext with a SQLite in-memory database
/// backed by the shared <see cref="SqliteConnection"/> so the database
/// persists for the entire test run.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _sharedConnection;

    public CustomWebApplicationFactory(SqliteConnection sharedConnection)
    {
        _sharedConnection = sharedConnection;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext options so we can replace the connection.
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

            // Re-register with the shared in-memory SQLite connection.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(_sharedConnection));
        });

        // Suppress static file serving warnings in tests.
        builder.ConfigureAppConfiguration((_, config) => { });
    }
}
