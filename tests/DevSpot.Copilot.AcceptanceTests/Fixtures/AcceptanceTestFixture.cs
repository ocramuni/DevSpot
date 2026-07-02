using System.Globalization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using DevSpot.Copilot.AcceptanceTests.Infrastructure.Hosting;

namespace DevSpot.Copilot.AcceptanceTests.Fixtures;

public sealed class AcceptanceTestFixture : IAsyncDisposable
{
    private readonly SemaphoreSlim _initializationGate = new(1, 1);
    private readonly ScenarioAuthState _authState;
    private bool _initialized;
    private CultureInfo? _originalCulture;
    private CultureInfo? _originalUICulture;
    private SqliteConnection? _connection;

    public AcceptanceTestFixture(ScenarioAuthState authState)
    {
        _authState = authState;
    }

    public AcceptanceWebApplicationFactory Factory { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        await _initializationGate.WaitAsync();
        try
        {
            if (_initialized)
            {
                return;
            }

            _originalCulture = CultureInfo.CurrentCulture;
            _originalUICulture = CultureInfo.CurrentUICulture;
            var italianCulture = CultureInfo.GetCultureInfo("it-IT");
            CultureInfo.DefaultThreadCurrentCulture = italianCulture;
            CultureInfo.DefaultThreadCurrentUICulture = italianCulture;
            CultureInfo.CurrentCulture = italianCulture;
            CultureInfo.CurrentUICulture = italianCulture;

            _connection = new SqliteConnection("Data Source=:memory:");
            await _connection.OpenAsync();

            Factory = new AcceptanceWebApplicationFactory(_connection, _authState);
            using var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });

            _initialized = true;
        }
        finally
        {
            _initializationGate.Release();
        }
    }

    public HttpClient CreateClient()
    {
        return Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
    }

    public IServiceScope CreateScope()
    {
        return Factory.Services.CreateScope();
    }

    public async ValueTask DisposeAsync()
    {
        Factory?.Dispose();

        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }

        if (_originalCulture != null)
        {
            CultureInfo.DefaultThreadCurrentCulture = _originalCulture;
            CultureInfo.CurrentCulture = _originalCulture;
        }

        if (_originalUICulture != null)
        {
            CultureInfo.DefaultThreadCurrentUICulture = _originalUICulture;
            CultureInfo.CurrentUICulture = _originalUICulture;
        }

        _initializationGate.Dispose();
    }
}
