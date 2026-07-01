using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using DevSpot.Constants;
using DevSpot.Data;
using DevSpot.AcceptanceTests.Support.State;
using DevSpot.AcceptanceTests.Infrastructure.Database;
using DevSpot.AcceptanceTests.Infrastructure.Hosting;

namespace DevSpot.AcceptanceTests.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioState _state;
        private readonly DatabaseResetter _resetter;
        private readonly CustomWebApplicationFactory _factory;
        private readonly SeedProfileResolver _seedResolver;
        private readonly ScenarioContext _scenarioContext;

        public Hooks(
            ScenarioState state,
            DatabaseResetter resetter,
            CustomWebApplicationFactory factory,
            SeedProfileResolver seedResolver,
            ScenarioContext scenarioContext)
        {
            _state = state;
            _resetter = resetter;
            _factory = factory;
            _seedResolver = seedResolver;
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario(Order = 0)]
        public void ClearScenarioScopedState()
        {
            _state.CurrentUserEmail = null;
            _state.CurrentUserRole = null;
            _state.LastResponse = null;
            _state.LastResponseContent = null;
            _state.CurrentForm = null;
            _state.HttpClient = null;
        }

        [BeforeScenario(Order = 10)]
        public async Task ResetDatabaseState()
        {
            await _resetter.ResetAsync();
        }

        [BeforeScenario(Order = 20)]
        public async Task ApplyBaseSeeds()
        {
            // Always run RoleSeeder as the base setup so roles exist
            using var scope = _factory.Services.CreateScope();
            await RoleSeeder.RoleSeederAsync(scope.ServiceProvider);
        }

        [BeforeScenario(Order = 30)]
        public async Task ApplyTagSpecificSeeds()
        {
            var tags = _scenarioContext.ScenarioInfo.Tags;
            await _seedResolver.ResolveAndApplySeedsAsync(tags);
        }

        [BeforeScenario(Order = 40)]
        public void ConfigureScenarioAuthProfile()
        {
            var tags = _scenarioContext.ScenarioInfo.Tags;

            if (tags.Contains("auth:anonymous"))
            {
                _state.CurrentUserEmail = null;
                _state.CurrentUserRole = null;
            }
            else if (tags.Contains("auth:jobseeker"))
            {
                _state.CurrentUserEmail = "jobseeker@devspot.com";
                _state.CurrentUserRole = Roles.JobSeeker;
            }
            else if (tags.Contains("auth:employer"))
            {
                _state.CurrentUserEmail = "employer@devspot.com";
                _state.CurrentUserRole = Roles.Employer;
            }
            else if (tags.Contains("auth:admin"))
            {
                _state.CurrentUserEmail = "admin@devspot.com";
                _state.CurrentUserRole = Roles.Admin;
            }
        }

        [BeforeScenario(Order = 50)]
        public void CreateHttpClientAndHttpState()
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false // We handle redirects manually
            };

            var client = _factory.CreateClient(options);

            if (!string.IsNullOrEmpty(_state.CurrentUserEmail))
            {
                client.DefaultRequestHeaders.Add("X-Test-User-Email", _state.CurrentUserEmail);
            }

            _state.HttpClient = client;
        }

        [AfterScenario(Order = 1000)]
        public void FailureDiagnosticsAndCleanup()
        {
            if (_scenarioContext.TestError != null)
            {
                Console.WriteLine($"Scenario '{_scenarioContext.ScenarioInfo.Title}' FAILED with error: {_scenarioContext.TestError.Message}");
                if (_state.LastResponse != null)
                {
                    Console.WriteLine($"Last Status Code: {(int)_state.LastResponse.StatusCode} {_state.LastResponse.StatusCode}");
                    if (_state.LastResponse.Headers.Location != null)
                    {
                        Console.WriteLine($"Last Redirect Location: {_state.LastResponse.Headers.Location}");
                    }
                }
                if (!string.IsNullOrEmpty(_state.LastResponseContent))
                {
                    Console.WriteLine("Last Response Content Preview (first 1000 chars):");
                    Console.WriteLine(_state.LastResponseContent.Length > 1000 
                        ? _state.LastResponseContent[..1000] 
                        : _state.LastResponseContent);
                }
            }

            _state.HttpClient?.Dispose();
        }
    }
}
