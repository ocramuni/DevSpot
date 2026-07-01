using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevSpot.AcceptanceTests.Infrastructure.Hosting
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            UserManager<IdentityUser> userManager)
            : base(options, logger, encoder)
        {
            _userManager = userManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Test-User-Email", out var emailValues))
            {
                // Fallback to cookie authentication if no header is present
                var cookieResult = await Context.AuthenticateAsync(IdentityConstants.ApplicationScheme);
                if (cookieResult.Succeeded)
                {
                    return cookieResult;
                }
                return AuthenticateResult.NoResult();
            }

            var email = emailValues.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return AuthenticateResult.NoResult();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return AuthenticateResult.Fail($"Test user '{email}' not found.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? email),
                new Claim(ClaimTypes.Email, email)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, "TestScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            return AuthenticateResult.Success(ticket);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            await Context.ChallengeAsync(IdentityConstants.ApplicationScheme, properties);
        }
    }
}
