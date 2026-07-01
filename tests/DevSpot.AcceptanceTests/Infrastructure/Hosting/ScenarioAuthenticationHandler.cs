using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace DevSpot.AcceptanceTests.Infrastructure.Hosting;

public static class ScenarioAuthenticationDefaults
{
    public const string Scheme = "Scenario";
}

public sealed class ScenarioAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public ScenarioAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authState = ScenarioAuthState.Current;

        if (!authState.IsAuthenticated || authState.User == null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, authState.User.Id),
            new(ClaimTypes.Name, authState.User.UserName ?? authState.User.Email ?? authState.User.Id),
            new(ClaimTypes.Email, authState.User.Email ?? string.Empty)
        };

        var role = authState.GetRole();
        if (!string.IsNullOrWhiteSpace(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, ScenarioAuthenticationDefaults.Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, ScenarioAuthenticationDefaults.Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
