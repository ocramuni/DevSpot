using DevSpot.AcceptanceTests.Support.Html;
using DevSpot.AcceptanceTests.Support.Routing;
using DevSpot.AcceptanceTests.Support.State;
using Reqnroll;

namespace DevSpot.AcceptanceTests.Steps.Common;

[Binding]
public sealed class NavigationSteps
{
    private readonly ScenarioHttpState _httpState;
    private readonly RouteResolver _routeResolver;
    private readonly HtmlFormLoader _formLoader;

    public NavigationSteps(
        ScenarioHttpState httpState,
        RouteResolver routeResolver,
        HtmlFormLoader formLoader)
    {
        _httpState = httpState;
        _routeResolver = routeResolver;
        _formLoader = formLoader;
    }

    [Given(@"che sono sulla pagina ""(.*)""")]
    public async Task GivenIAmOnThePage(string pageName)
    {
        await NavigateToPageAsync(pageName);
    }

    [When(@"vado alla pagina ""(.*)""")]
    public async Task WhenIGoToThePage(string pageName)
    {
        await NavigateToPageAsync(pageName);
    }

    [When(@"seguo il reindirizzamento")]
    public async Task WhenIFollowTheRedirect()
    {
        var response = _httpState.LastResponse ?? throw new InvalidOperationException("Nessuna risposta disponibile da seguire.");
        if (response.Headers.Location is null)
        {
            throw new InvalidOperationException("L'ultima risposta non era un reindirizzamento.");
        }

        var client = _httpState.Client ?? throw new InvalidOperationException("Nessun client dello scenario disponibile.");
        var location = response.Headers.Location ?? throw new InvalidOperationException("La risposta di reindirizzamento non conteneva l'intestazione Location.");
        var target = location.IsAbsoluteUri ? location : new Uri(response.RequestMessage!.RequestUri!, location);
        var followUpResponse = await client.GetAsync(target);

        _httpState.LastResponse?.Dispose();
        _httpState.LastResponse = followUpResponse;
        _httpState.LastHtml = await followUpResponse.Content.ReadAsStringAsync();
        _httpState.CurrentForm = followUpResponse.IsSuccessStatusCode ? await _formLoader.TryLoadAsync(followUpResponse) : null;
    }

    private async Task NavigateToPageAsync(string pageName)
    {
        var client = _httpState.Client ?? throw new InvalidOperationException("Nessun client dello scenario disponibile.");
        var response = await client.GetAsync(_routeResolver.Resolve(pageName));

        _httpState.LastResponse?.Dispose();
        _httpState.LastResponse = response;
        _httpState.LastHtml = await response.Content.ReadAsStringAsync();
        _httpState.CurrentForm = response.IsSuccessStatusCode ? await _formLoader.TryLoadAsync(response) : null;
    }
}
