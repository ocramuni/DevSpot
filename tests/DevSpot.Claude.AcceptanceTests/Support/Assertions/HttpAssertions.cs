using System.Net;
using DevSpot.Claude.AcceptanceTests.Support.Html;
using DevSpot.Claude.AcceptanceTests.Support.State;
using Xunit;

namespace DevSpot.Claude.AcceptanceTests.Support.Assertions;

/// <summary>
/// Assertion helpers that map to business-readable step outcomes.
/// All methods throw <see cref="Xunit.Sdk.XunitException"/> on failure
/// so xUnit reports the scenario as failed.
/// </summary>
public sealed class HttpAssertions
{
    private readonly ScenarioState _state;

    public HttpAssertions(ScenarioState state)
    {
        _state = state;
    }

    public void AssertStatusCode(HttpStatusCode expected) =>
        Assert.Equal(expected, _state.LastStatusCode);

    public void AssertSuccess() =>
        Assert.True(
            (int)_state.LastStatusCode is >= 200 and < 300,
            $"Expected success status but got {_state.LastStatusCode}.");

    public void AssertRedirect(string expectedPath)
    {
        Assert.True(_state.LastResponse?.IsRedirect() ?? false,
            $"Expected a redirect but got {_state.LastStatusCode}.");

        var location = _state.LastRedirectLocation?.OriginalString
            ?? _state.LastRedirectLocation?.ToString()
            ?? string.Empty;

        Assert.True(
            location.Contains(expectedPath, StringComparison.OrdinalIgnoreCase),
            $"Expected redirect to '{expectedPath}' but got '{location}'.");
    }

    public void AssertRedirectToLogin()
    {
        Assert.True(_state.LastResponse?.IsRedirect() ?? false,
            $"Expected a redirect to login but got {_state.LastStatusCode}.");

        var location = _state.LastRedirectLocation?.OriginalString ?? string.Empty;
        Assert.True(
            location.Contains("Login", StringComparison.OrdinalIgnoreCase) ||
            location.Contains("Account", StringComparison.OrdinalIgnoreCase),
            $"Expected redirect to login page but location was '{location}'.");
    }

    public async Task AssertValidationErrorForAsync(string fieldName)
    {
        Assert.False(string.IsNullOrEmpty(_state.LastResponseContent),
            "No response content to inspect for validation errors.");

        var doc = await HtmlDocumentParser.ParseAsync(_state.LastResponseContent!);
        Assert.True(
            HtmlDocumentParser.HasValidationErrorFor(doc, fieldName),
            $"Expected a validation error for field '{fieldName}' but none was found in the HTML.");
    }

    public async Task AssertPageContainsTextAsync(string text)
    {
        Assert.False(string.IsNullOrEmpty(_state.LastResponseContent),
            "No response content to inspect.");

        var doc = await HtmlDocumentParser.ParseAsync(_state.LastResponseContent!);
        Assert.True(
            HtmlDocumentParser.ContainsText(doc, text),
            $"Expected page to contain '{text}' but it did not.");
    }

    public async Task AssertPageDoesNotContainTextAsync(string text)
    {
        Assert.False(string.IsNullOrEmpty(_state.LastResponseContent),
            "No response content to inspect.");

        var doc = await HtmlDocumentParser.ParseAsync(_state.LastResponseContent!);
        Assert.False(
            HtmlDocumentParser.ContainsText(doc, text),
            $"Expected page NOT to contain '{text}' but it did.");
    }
}

internal static class HttpResponseMessageAssertionExtensions
{
    internal static bool IsRedirect(this HttpResponseMessage response) =>
        (int)response.StatusCode is >= 300 and < 400;
}
