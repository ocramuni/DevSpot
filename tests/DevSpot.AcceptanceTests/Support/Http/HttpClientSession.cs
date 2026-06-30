using DevSpot.AcceptanceTests.Fixtures;
using DevSpot.AcceptanceTests.Infrastructure.Authentication;
using DevSpot.AcceptanceTests.Infrastructure.Hosting;
using DevSpot.AcceptanceTests.Support.Html;
using DevSpot.AcceptanceTests.Support.State;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DevSpot.AcceptanceTests.Support.Http;

/// <summary>
/// Wraps the per-scenario HttpClient.
/// Tracks the last response so step definitions and assertions can inspect it.
/// Handles real cookie-based login for @auth:* tags.
/// </summary>
public sealed class HttpClientSession
{
    private readonly ScenarioState _state;
    private HttpClient? _client;

    public HttpClient Client => _client ?? throw new InvalidOperationException(
        "HttpClientSession has not been initialized. Call Initialize() first.");

    public HttpClientSession(ScenarioState state)
    {
        _state = state;
    }

    public void Initialize(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
    }

    public async Task<HttpResponseMessage> GetAsync(string path)
    {
        var response = await Client.GetAsync(path);
        await CaptureAsync(response);
        return response;
    }

    public async Task<HttpResponseMessage> PostFormAsync(
        string path,
        IEnumerable<KeyValuePair<string, string>> formData)
    {
        var content = new FormUrlEncodedContent(formData);
        var response = await Client.PostAsync(path, content);
        await CaptureAsync(response);
        return response;
    }

    /// <summary>
    /// Follows a redirect response (3xx) and captures the result.
    /// Useful when AllowAutoRedirect=false and we need to land on the target page.
    /// </summary>
    public async Task<HttpResponseMessage> FollowRedirectAsync(HttpResponseMessage redirect)
    {
        var location = redirect.Headers.Location
            ?? throw new InvalidOperationException("Response has no Location header.");

        // Location may be relative.
        var path = location.IsAbsoluteUri ? location.PathAndQuery : location.OriginalString;
        return await GetAsync(path);
    }

    /// <summary>
    /// Logs in as the user described by <paramref name="profile"/> using real form submission.
    /// After a successful login the client's cookie container holds the auth cookie.
    /// </summary>
    public async Task LoginAsync(string profile)
    {
        var (email, password) = AuthProfile.CredentialsFor(profile);

        // 1. GET the login page to obtain the antiforgery token.
        var loginPage = await GetAsync("/Identity/Account/Login");
        loginPage.EnsureSuccessStatusCode();

        var doc = await HtmlDocumentParser.ParseAsync(
            _state.LastResponseContent!,
            "http://localhost/Identity/Account/Login");
        var token = HtmlDocumentParser.ExtractAntiForgeryToken(doc);

        // 2. POST the credentials.
        var fields = new List<KeyValuePair<string, string>>
        {
            new("Input.Email",    email),
            new("Input.Password", password),
            new("Input.RememberMe", "false")
        };

        if (token is not null)
            fields.Add(new("__RequestVerificationToken", token));

        var loginResponse = await PostFormAsync("/Identity/Account/Login", fields);

        // A successful login returns 302 → home page. We accept any redirect as success.
        if (!loginResponse.IsRedirect())
            throw new InvalidOperationException(
                $"Login as '{profile}' failed. Status: {loginResponse.StatusCode}. " +
                $"Body: {_state.LastResponseContent}");
    }

    private async Task CaptureAsync(HttpResponseMessage response)
    {
        _state.LastResponse = response;
        _state.LastResponseContent = await response.Content.ReadAsStringAsync();
    }
}

internal static class HttpResponseMessageExtensions
{
    internal static bool IsRedirect(this HttpResponseMessage response) =>
        (int)response.StatusCode is >= 300 and < 400;
}
