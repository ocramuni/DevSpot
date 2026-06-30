using DevSpot.AcceptanceTests.Support.Assertions;
using DevSpot.AcceptanceTests.Support.Forms;
using Reqnroll;

namespace DevSpot.AcceptanceTests.Steps.Forms;

[Binding]
public sealed class FormSteps
{
    private readonly FormSession _form;
    private readonly HttpAssertions _assert;

    public FormSteps(FormSession form, HttpAssertions assert)
    {
        _form = form;
        _assert = assert;
    }

    [Given("sono sulla pagina del modulo {string}")]
    [When("navigo alla pagina del modulo {string}")]
    public async Task LoadFormPage(string path)
    {
        await _form.LoadAsync(path);
    }

    [When("compilo il modulo con i seguenti dati")]
    public void FillFormFromTable(Table table)
    {
        foreach (var row in table.Rows)
        {
            var field = row["Campo"];
            var value = row["Valore"];
            _form.Fill(field, value);
        }
    }

    [When("invio il modulo")]
    public async Task SubmitForm()
    {
        await _form.SubmitAsync();
    }

    [Then("vedo un errore di validazione per il campo {string}")]
    public async Task AssertValidationErrorFor(string fieldName)
    {
        await _assert.AssertValidationErrorForAsync(fieldName);
    }

    [Then("vengo reindirizzato a {string}")]
    public void AssertRedirectedTo(string path)
    {
        _assert.AssertRedirect(path);
    }

    [Then("vengo reindirizzato alla pagina di accesso")]
    public void AssertRedirectedToLogin()
    {
        _assert.AssertRedirectToLogin();
    }

    [Then("il codice di risposta è {int}")]
    public void AssertStatusCode(int statusCode)
    {
        _assert.AssertStatusCode((System.Net.HttpStatusCode)statusCode);
    }

    [Then("la pagina contiene {string}")]
    public async Task AssertPageContains(string text)
    {
        await _assert.AssertPageContainsTextAsync(text);
    }

    [Then("la pagina non contiene {string}")]
    public async Task AssertPageDoesNotContain(string text)
    {
        await _assert.AssertPageDoesNotContainTextAsync(text);
    }
}
