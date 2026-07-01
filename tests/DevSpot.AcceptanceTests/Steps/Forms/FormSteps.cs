using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Reqnroll;
using DevSpot.AcceptanceTests.Support.State;
using DevSpot.AcceptanceTests.Support.Forms;

namespace DevSpot.AcceptanceTests.Steps.Forms
{
    [Binding]
    public class FormSteps
    {
        private readonly ScenarioState _state;

        public FormSteps(ScenarioState state)
        {
            _state = state;
        }

        [When(@"compilo il modulo con i seguenti valori:")]
        public void FillForm(DataTable table)
        {
            if (_state.CurrentForm == null)
            {
                throw new InvalidOperationException("Nessun modulo attivo è caricato. Assicurati di aver navigato in una pagina con un modulo.");
            }

            foreach (var row in table.Rows)
            {
                var fieldName = row[0];
                var fieldValue = row[1];

                var actualKey = _state.CurrentForm.Fields.Keys
                    .FirstOrDefault(k => k.Equals(fieldName, StringComparison.OrdinalIgnoreCase) 
                                         || k.EndsWith("." + fieldName, StringComparison.OrdinalIgnoreCase));

                if (actualKey != null)
                {
                    _state.CurrentForm.SetField(actualKey, fieldValue);
                }
                else
                {
                    _state.CurrentForm.SetField(fieldName, fieldValue);
                }
            }
        }

        [When(@"invio il modulo")]
        [Given(@"invio il modulo")]
        public async Task SubmitForm()
        {
            if (_state.HttpClient == null)
            {
                throw new InvalidOperationException("HttpClient non è stato inizializzato.");
            }
            if (_state.CurrentForm == null)
            {
                throw new InvalidOperationException("Nessun modulo attivo è caricato per essere inviato.");
            }

            var actionUrl = _state.CurrentForm.Action;
            
            var baseUri = _state.LastResponse?.RequestMessage?.RequestUri;
            Uri requestUri;
            if (baseUri != null)
            {
                requestUri = new Uri(baseUri, actionUrl);
            }
            else
            {
                requestUri = new Uri(actionUrl, UriKind.RelativeOrAbsolute);
            }

            var requestContent = _state.CurrentForm.ToFormUrlEncodedContent();

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = requestContent
            };

            var response = await _state.HttpClient.SendAsync(request);

            _state.LastResponse = response;
            _state.LastResponseContent = await response.Content.ReadAsStringAsync();
        }

        [Then(@"dovrei essere reindirizzato alla pagina ""(.*)""")]
        public async Task ShouldBeRedirectedToPage(string pageName)
        {
            Assert.NotNull(_state.LastResponse);
            
            var statusCode = (int)_state.LastResponse.StatusCode;
            Assert.True(statusCode >= 300 && statusCode < 400, $"Atteso codice di stato di reindirizzamento (3xx), ma ottenuto: {statusCode}");

            var location = _state.LastResponse.Headers.Location;
            Assert.NotNull(location);

            if (_state.HttpClient != null)
            {
                var response = await _state.HttpClient.GetAsync(location);
                _state.LastResponse = response;
                _state.LastResponseContent = await response.Content.ReadAsStringAsync();
            }
        }

        [Then(@"dovrei vedere un errore di validazione per ""(.*)""")]
        [Then(@"dovrei vedere un errore di validazione per il campo ""(.*)""")]
        public void ShouldSeeValidationError(string fieldName)
        {
            Assert.NotNull(_state.LastResponseContent);
            
            var hasValidationError = _state.LastResponseContent.Contains($"data-valmsg-for=\"{fieldName}\"", StringComparison.OrdinalIgnoreCase)
                                     || _state.LastResponseContent.Contains($"data-valmsg-for=\"Input.{fieldName}\"", StringComparison.OrdinalIgnoreCase)
                                     || _state.LastResponseContent.Contains($"field-validation-error", StringComparison.OrdinalIgnoreCase);

            Assert.True(hasValidationError, $"Atteso errore di validazione per il campo '{fieldName}', ma non è stato trovato nella risposta HTML.");
        }

        [Then(@"dovrei vedere il messaggio di errore di validazione ""(.*)""")]
        public void ShouldSeeValidationErrorMessage(string errorMessage)
        {
            Assert.NotNull(_state.LastResponseContent);
            Assert.Contains(errorMessage, _state.LastResponseContent, StringComparison.OrdinalIgnoreCase);
        }

        [Then(@"dovrei vedere degli errori di validazione")]
        public void ShouldSeeValidationErrors()
        {
            Assert.NotNull(_state.LastResponseContent);
            Assert.Contains("field-validation-error", _state.LastResponseContent, StringComparison.OrdinalIgnoreCase);
        }

        [Then(@"dovrei ottenere uno stato di risposta ""(.*)""")]
        public void ShouldGetStatus(string statusName)
        {
            Assert.NotNull(_state.LastResponse);
            var expectedStatus = Enum.Parse<HttpStatusCode>(statusName, ignoreCase: true);
            Assert.Equal(expectedStatus, _state.LastResponse.StatusCode);
        }

        [Then(@"dovrei essere reindirizzato alla pagina di login")]
        public void ShouldRedirectToLogin()
        {
            Assert.NotNull(_state.LastResponse);
            var statusCode = (int)_state.LastResponse.StatusCode;
            Assert.True(statusCode >= 300 && statusCode < 400, $"Atteso reindirizzamento (3xx), ma ottenuto: {statusCode}");

            var location = _state.LastResponse.Headers.Location;
            Assert.NotNull(location);
            Assert.Contains("Login", location.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
