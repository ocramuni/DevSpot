using DevSpot.Copilot.AcceptanceTests.Support.State;
using DevSpot.Copilot.AcceptanceTests.Support.Html;
using Reqnroll;

namespace DevSpot.Copilot.AcceptanceTests.Steps.Forms;

[Binding]
public sealed class FormSteps
{
    private readonly ScenarioHttpState _httpState;
    private readonly HtmlFormLoader _formLoader;

    public FormSteps(ScenarioHttpState httpState, HtmlFormLoader formLoader)
    {
        _httpState = httpState;
        _formLoader = formLoader;
    }

    [When(@"compilo il modulo con i seguenti valori")]
    public void WhenIFillTheFormWithTheFollowingValues(Table table)
    {
        var form = _httpState.CurrentForm ?? throw new InvalidOperationException("Nessun modulo è stato caricato.");

        foreach (var row in table.Rows)
        {
            var fieldName = row.ContainsKey("Campo") ? row["Campo"] : row["Field"];
            var fieldValue = row.ContainsKey("Valore") ? row["Valore"] : row["Value"];
            form.SetField(fieldName, fieldValue);
        }
    }

    [When(@"invio il modulo")]
    public async Task WhenISubmitTheForm()
    {
        var client = _httpState.Client ?? throw new InvalidOperationException("Nessun client dello scenario disponibile.");
        var form = _httpState.CurrentForm ?? throw new InvalidOperationException("Nessun modulo è stato caricato.");

        HttpResponseMessage response;
        if (form.Method == HttpMethod.Get)
        {
            response = await client.GetAsync(form.Action);
        }
        else
        {
            response = await client.PostAsync(form.Action, form.ToContent());
        }

        _httpState.LastResponse?.Dispose();
        _httpState.LastResponse = response;
        _httpState.LastHtml = await response.Content.ReadAsStringAsync();
        _httpState.CurrentForm = response.IsSuccessStatusCode ? await _formLoader.TryLoadAsync(response) : null;
    }
}
