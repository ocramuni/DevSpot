using DevSpot.Copilot.AcceptanceTests.Fixtures;
using DevSpot.Copilot.AcceptanceTests.Infrastructure.Database;
using DevSpot.Copilot.AcceptanceTests.Infrastructure.Hosting;
using DevSpot.Copilot.AcceptanceTests.Support.State;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;

namespace DevSpot.Copilot.AcceptanceTests.Hooks;

[Binding]
public sealed class AcceptanceHooks
{
    private const string EmployerEmail = "employer@devspot.com";
    private const string JobSeekerEmail = "jobseeker@devspot.com";
    private const string AdminEmail = "admin@devspot.com";

    private readonly AcceptanceTestFixture _fixture;
    private readonly ScenarioContext _scenarioContext;
    private readonly ScenarioAuthState _authState;
    private readonly ScenarioHttpState _httpState;
    private readonly DatabaseResetter _databaseResetter;
    private readonly DatabaseSeeder _databaseSeeder;

    public AcceptanceHooks(
        AcceptanceTestFixture fixture,
        ScenarioContext scenarioContext,
        ScenarioAuthState authState,
        ScenarioHttpState httpState,
        DatabaseResetter databaseResetter,
        DatabaseSeeder databaseSeeder)
    {
        _fixture = fixture;
        _scenarioContext = scenarioContext;
        _authState = authState;
        _httpState = httpState;
        _databaseResetter = databaseResetter;
        _databaseSeeder = databaseSeeder;
    }

    [BeforeScenario(Order = 0)]
    public Task ClearScenarioState()
    {
        _authState.Reset();
        _httpState.Reset();
        return Task.CompletedTask;
    }

    [BeforeScenario(Order = 10)]
    public async Task ResetDatabase()
    {
        await _fixture.InitializeAsync();
        using var scope = _fixture.CreateScope();
        await _databaseResetter.ResetAsync(scope.ServiceProvider);
    }

    [BeforeScenario(Order = 20)]
    public async Task ApplyBaseSeeds()
    {
        using var scope = _fixture.CreateScope();
        await _databaseSeeder.SeedBaseAsync(scope.ServiceProvider);
        await _databaseSeeder.SeedJobPostingsAsync(scope.ServiceProvider);
    }

    [BeforeScenario(Order = 30)]
    public Task ApplyTagSeeds()
    {
        return Task.CompletedTask;
    }

    [BeforeScenario(Order = 40)]
    public async Task ConfigureAuthProfile()
    {
        var tags = _scenarioContext.ScenarioInfo.Tags;
        var authTag = tags.FirstOrDefault(tag => tag.StartsWith("auth:", StringComparison.OrdinalIgnoreCase));

        if (authTag is null || authTag.Equals("auth:anonymous", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        using var scope = _fixture.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        _authState.Profile = authTag.ToLowerInvariant() switch
        {
            "auth:admin" => ScenarioAuthProfile.Admin,
            "auth:jobseeker" => ScenarioAuthProfile.JobSeeker,
            "auth:employer" => ScenarioAuthProfile.Employer,
            _ => ScenarioAuthProfile.Anonymous
        };

        _authState.User = _authState.Profile switch
        {
            ScenarioAuthProfile.Admin => await userManager.FindByEmailAsync(AdminEmail),
            ScenarioAuthProfile.JobSeeker => await userManager.FindByEmailAsync(JobSeekerEmail),
            ScenarioAuthProfile.Employer => await userManager.FindByEmailAsync(EmployerEmail),
            _ => null
        };
    }

    [BeforeScenario(Order = 50)]
    public async Task CreateClient()
    {
        await _fixture.InitializeAsync();
        _httpState.Client = _fixture.CreateClient();
    }

    [AfterScenario(Order = 1000)]
    public Task Cleanup()
    {
        _httpState.LastResponse?.Dispose();
        _httpState.Client?.Dispose();
        return Task.CompletedTask;
    }
}
