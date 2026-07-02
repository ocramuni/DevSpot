using DevSpot.Copilot.AcceptanceTests.Fixtures;
using DevSpot.Copilot.AcceptanceTests.Support.Routing;
using DevSpot.Copilot.AcceptanceTests.Support.State;
using DevSpot.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using Xunit;
using System.Text.RegularExpressions;

namespace DevSpot.Copilot.AcceptanceTests.Steps.Common;

[Binding]
public sealed class AssertionSteps
{
    private static readonly Regex RequiredFieldMessagePattern = new(@"^Il campo (.+) è obbligatorio\.$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private readonly ScenarioHttpState _httpState;
    private readonly RouteResolver _routeResolver;
    private readonly AcceptanceTestFixture _fixture;

    public AssertionSteps(
        ScenarioHttpState httpState,
        RouteResolver routeResolver,
        AcceptanceTestFixture fixture)
    {
        _httpState = httpState;
        _routeResolver = routeResolver;
        _fixture = fixture;
    }

    [Then(@"dovrei essere reindirizzato alla pagina ""(.*)""")]
    public void ThenIShouldBeRedirectedToThePage(string pageName)
    {
        var response = _httpState.LastResponse ?? throw new InvalidOperationException("Nessuna risposta disponibile.");
        if (response.Headers.Location is null)
        {
            throw new InvalidOperationException("L'ultima risposta non era un reindirizzamento.");
        }

        var expectedPath = _routeResolver.Resolve(pageName);
        var actualPath = response.Headers.Location.IsAbsoluteUri ? response.Headers.Location.AbsolutePath : response.Headers.Location.OriginalString;

        Assert.Contains(expectedPath, actualPath, StringComparison.OrdinalIgnoreCase);
    }

    [Then(@"dovrei vedere il messaggio di convalida ""(.*)""")]
    public void ThenIShouldSeeTheValidationMessage(string message)
    {
        var html = _httpState.LastHtml ?? throw new InvalidOperationException("Nessuna risposta HTML disponibile.");
        Assert.Contains(TranslateValidationMessage(message), html, StringComparison.OrdinalIgnoreCase);
    }

    [Then(@"dovrei vedere ""(.*)""")]
    public void ThenIShouldSeeText(string expectedText)
    {
        var html = _httpState.LastHtml ?? throw new InvalidOperationException("Nessuna risposta HTML disponibile.");
        Assert.Contains(expectedText, html, StringComparison.OrdinalIgnoreCase);
    }

    [Then(@"non dovrei vedere ""(.*)""")]
    public void ThenIShouldNotSeeText(string unexpectedText)
    {
        var html = _httpState.LastHtml ?? throw new InvalidOperationException("Nessuna risposta HTML disponibile.");
        Assert.DoesNotContain(unexpectedText, html, StringComparison.OrdinalIgnoreCase);
    }

    [Then(@"il database dovrebbe contenere un annuncio di lavoro intitolato ""(.*)""")]
    public async Task ThenTheDatabaseShouldContainAJobPostingTitled(string title)
    {
        using var scope = _fixture.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var exists = await dbContext.JobPosting.AnyAsync(jobPosting => jobPosting.Title == title);
        Assert.True(exists);
    }

    [Then(@"il database non dovrebbe contenere un annuncio di lavoro intitolato ""(.*)""")]
    public async Task ThenTheDatabaseShouldNotContainAJobPostingTitled(string title)
    {
        using var scope = _fixture.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var exists = await dbContext.JobPosting.AnyAsync(jobPosting => jobPosting.Title == title);
        Assert.False(exists);
    }

    [Then(@"il database dovrebbe contenere un utente con email ""(.*)"" nel ruolo ""(.*)""")]
    public async Task ThenTheDatabaseShouldContainAUserWithEmailInRole(string email, string role)
    {
        using var scope = _fixture.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);
        Assert.True(await userManager.IsInRoleAsync(user!, role));
    }

    private static string TranslateValidationMessage(string expectedMessage)
    {
        var match = RequiredFieldMessagePattern.Match(expectedMessage);
        if (!match.Success)
        {
            return expectedMessage switch
            {
                "Il tentativo di accesso non è valido." => "Invalid login attempt.",
                _ => expectedMessage
            };
        }

        return $"The {match.Groups[1].Value} field is required.";
    }
}
