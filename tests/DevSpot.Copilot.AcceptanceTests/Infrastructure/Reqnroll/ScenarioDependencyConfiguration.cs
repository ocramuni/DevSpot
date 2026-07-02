using DevSpot.Copilot.AcceptanceTests.Fixtures;
using DevSpot.Copilot.AcceptanceTests.Infrastructure.Database;
using DevSpot.Copilot.AcceptanceTests.Infrastructure.Hosting;
using DevSpot.Copilot.AcceptanceTests.Support.Forms;
using DevSpot.Copilot.AcceptanceTests.Support.Html;
using DevSpot.Copilot.AcceptanceTests.Support.Routing;
using DevSpot.Copilot.AcceptanceTests.Support.State;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace DevSpot.Copilot.AcceptanceTests.Infrastructure.Reqnroll;

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
