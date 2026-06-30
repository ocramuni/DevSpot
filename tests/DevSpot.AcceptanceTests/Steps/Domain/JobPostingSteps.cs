using DevSpot.AcceptanceTests.Fixtures;
using DevSpot.AcceptanceTests.Support.Assertions;
using DevSpot.AcceptanceTests.Support.Forms;
using DevSpot.AcceptanceTests.Support.Http;
using DevSpot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using Xunit;

namespace DevSpot.AcceptanceTests.Steps.Domain;

[Binding]
public sealed class JobPostingSteps
{
    private readonly FormSession _form;
    private readonly HttpClientSession _http;
    private readonly HttpAssertions _assert;
    private readonly AcceptanceTestFixture _fixture;

    public JobPostingSteps(
        FormSession form,
        HttpClientSession http,
        HttpAssertions assert,
        AcceptanceTestFixture fixture)
    {
        _form = form;
        _http = http;
        _assert = assert;
        _fixture = fixture;
    }

    [Given("sono sulla pagina di creazione dell'annuncio")]
    public async Task NavigateToCreatePage()
    {
        await _form.LoadAsync("/JobPostings/Create");
    }

    [When("creo un annuncio di lavoro con i seguenti dati")]
    public async Task CreateJobPosting(Table table)
    {
        await _form.LoadAsync("/JobPostings/Create");

        foreach (var row in table.Rows)
            _form.Fill(row["Campo"], row["Valore"]);

        await _form.SubmitAsync();
    }

    [Then("vengo reindirizzato alla pagina degli annunci")]
    public void AssertRedirectedToIndex()
    {
        _assert.AssertRedirect("/");
    }

    [Then("l'annuncio {string} viene salvato nel database")]
    public async Task AssertJobPostingIsSaved(string title)
    {
        using var scope = _fixture.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var exists = await db.JobPosting.AnyAsync(jp => jp.Title == title);
        Assert.True(exists, $"Era atteso un annuncio con titolo '{title}' nel database, ma non è stato trovato.");
    }

    [Then("nessun annuncio {string} viene salvato nel database")]
    public async Task AssertJobPostingIsNotSaved(string title)
    {
        using var scope = _fixture.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var exists = await db.JobPosting.AnyAsync(jp => jp.Title == title);
        Assert.False(exists, $"Era atteso che nessun annuncio con titolo '{title}' fosse nel database, ma ne è stato trovato uno.");
    }

    [Then("la pagina degli annunci mostra {int} annunci")]
    public async Task AssertListingCount(int expectedCount)
    {
        var response = await _http.GetAsync("/");
        response.EnsureSuccessStatusCode();

        using var scope = _fixture.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var count = await db.JobPosting.CountAsync();
        Assert.Equal(expectedCount, count);
    }
}
