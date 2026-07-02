using DevSpot.Data;
using DevSpot.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace DevSpot.Copilot.AcceptanceTests.Infrastructure.Hosting;

public sealed class AcceptanceWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;
    private readonly ScenarioAuthState _authState;

    public AcceptanceWebApplicationFactory(SqliteConnection connection, ScenarioAuthState authState)
    {
        _connection = connection;
        _authState = authState;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            var dbContextOptions = services
                .Where(service => service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>))
                .ToList();

            foreach (var descriptor in dbContextOptions)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));
            services.AddSingleton(_authState);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ScenarioAuthenticationDefaults.Scheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            }).AddScheme<AuthenticationSchemeOptions, ScenarioAuthenticationHandler>(ScenarioAuthenticationDefaults.Scheme, _ => { });

            services.AddSingleton<IEmailSender, global::DevSpot.Services.NoOpEmailSender>();
        });
    }
}
