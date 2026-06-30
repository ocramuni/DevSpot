using DevSpot.AcceptanceTests.Support.Http;
using DevSpot.AcceptanceTests.Support.State;
using Reqnroll;

namespace DevSpot.AcceptanceTests.Steps.Common;

[Binding]
public sealed class NavigationSteps
{
    private readonly HttpClientSession _http;
    private readonly ScenarioState _state;

    public NavigationSteps(HttpClientSession http, ScenarioState state)
    {
        _http = http;
        _state = state;
    }

    [Given("sono sulla pagina {string}")]
    [When("visito la pagina {string}")]
    public async Task NavigateTo(string path)
    {
        await _http.GetAsync(path);
    }

    [When("seguo il reindirizzamento")]
    public async Task FollowRedirect()
    {
        var last = _state.LastResponse
            ?? throw new InvalidOperationException("Nessuna risposta precedente da cui seguire il reindirizzamento.");
        await _http.FollowRedirectAsync(last);
    }
}
