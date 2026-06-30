using System.Net;

namespace DevSpot.AcceptanceTests.Support.State;

/// <summary>
/// Scenario-scoped bag of observable HTTP / UI state.
/// A fresh instance is injected for every scenario.
/// </summary>
public sealed class ScenarioState
{
    /// <summary>Auth profile tag value (e.g. "employer"), or null for anonymous.</summary>
    public string? AuthProfile { get; set; }

    /// <summary>The last HTTP response received.</summary>
    public HttpResponseMessage? LastResponse { get; set; }

    /// <summary>Body of <see cref="LastResponse"/> as a string.</summary>
    public string? LastResponseContent { get; set; }

    public HttpStatusCode LastStatusCode =>
        LastResponse?.StatusCode ?? HttpStatusCode.Unused;

    public Uri? LastRedirectLocation =>
        LastResponse?.Headers.Location;
}
