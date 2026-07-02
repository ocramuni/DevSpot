using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;
using DevSpot.Antigravity.AcceptanceTests.Fixtures;
using DevSpot.Antigravity.AcceptanceTests.Infrastructure.Hosting;
using DevSpot.Antigravity.AcceptanceTests.Infrastructure.Database;
using DevSpot.Antigravity.AcceptanceTests.Support.State;
using DevSpot.Antigravity.AcceptanceTests.Support.Forms;
using DevSpot.Antigravity.AcceptanceTests.Support.Http;
using DevSpot.Data;
using System;
using System.Collections.Generic;

namespace DevSpot.Antigravity.AcceptanceTests.Infrastructure.Reqnroll
{
    public static class TestDependencies
    {
        private static readonly AcceptanceTestFixture _fixture = new();

        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();

            // Register Singleton Fixture and WebApplicationFactory
            services.AddSingleton(_fixture);
            services.AddSingleton(_fixture.Factory);

            // Register Scope Tracker to dispose scopes cleanly
            services.AddScoped<ScenarioScopeTracker>();

            // Register DbContext from the running factory context
            services.AddScoped(sp =>
            {
                var factory = sp.GetRequiredService<CustomWebApplicationFactory>();
                var scope = factory.Services.CreateScope();
                sp.GetRequiredService<ScenarioScopeTracker>().Add(scope);
                return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            });

            // Register Database Infrastructure
            services.AddScoped<DatabaseInitializer>();
            services.AddScoped<DatabaseResetter>();
            services.AddScoped(sp =>
            {
                var factory = sp.GetRequiredService<CustomWebApplicationFactory>();
                var scope = factory.Services.CreateScope();
                sp.GetRequiredService<ScenarioScopeTracker>().Add(scope);
                return new DatabaseSeeder(scope.ServiceProvider);
            });
            services.AddScoped<SeedProfileResolver>();

            // Register Support services
            services.AddScoped<ScenarioState>();
            services.AddScoped<FormParser>();
            services.AddScoped<RouteResolver>();

            return services;
        }
    }

    public class ScenarioScopeTracker : IDisposable
    {
        private readonly List<IServiceScope> _scopes = new();

        public void Add(IServiceScope scope) => _scopes.Add(scope);

        public void Dispose()
        {
            foreach (var scope in _scopes)
            {
                scope.Dispose();
            }
        }
    }
}
