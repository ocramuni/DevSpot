using System;
using System.Threading.Tasks;
using Reqnroll;
using DevSpot.AcceptanceTests.Support.State;
using DevSpot.AcceptanceTests.Support.Http;
using DevSpot.AcceptanceTests.Support.Forms;

namespace DevSpot.AcceptanceTests.Steps.Common
{
    [Binding]
    public class NavigationSteps
    {
        private readonly ScenarioState _state;
        private readonly RouteResolver _routeResolver;
        private readonly FormParser _formParser;

        public NavigationSteps(ScenarioState state, RouteResolver routeResolver, FormParser formParser)
        {
            _state = state;
            _routeResolver = routeResolver;
            _formParser = formParser;
        }

        [Given(@"che sono sulla pagina ""(.*)""")]
        [When(@"navigo alla pagina ""(.*)""")]
        public async Task NavigateToPage(string pageName)
        {
            if (_state.HttpClient == null)
            {
                throw new InvalidOperationException("HttpClient non è stato inizializzato.");
            }

            var url = _routeResolver.Resolve(pageName);
            var response = await _state.HttpClient.GetAsync(url);
            
            _state.LastResponse = response;
            _state.LastResponseContent = await response.Content.ReadAsStringAsync();

            try
            {
                _state.CurrentForm = _formParser.ParseForm(_state.LastResponseContent);
            }
            catch
            {
                _state.CurrentForm = null;
            }
        }
    }
}
