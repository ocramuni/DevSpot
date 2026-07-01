using DevSpot.AcceptanceTests.Fixtures;
using DevSpot.AcceptanceTests.Infrastructure.Database;
using DevSpot.AcceptanceTests.Infrastructure.Hosting;
using DevSpot.AcceptanceTests.Support.Forms;
using DevSpot.AcceptanceTests.Support.Html;
using DevSpot.AcceptanceTests.Support.Routing;
using DevSpot.AcceptanceTests.Support.State;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace DevSpot.AcceptanceTests.Infrastructure.Reqnroll;

public static class ScenarioDependencyConfiguration
{
    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<AcceptanceTestFixture>();
        services.AddSingleton(ScenarioAuthState.Current);
        services.AddSingleton<ScenarioHttpState>();
        services.AddSingleton<DatabaseResetter>();
        services.AddSingleton<DatabaseSeeder>();
        services.AddSingleton<HtmlFormLoader>();
        services.AddSingleton<RouteResolver>();

        return services;
    }
}
