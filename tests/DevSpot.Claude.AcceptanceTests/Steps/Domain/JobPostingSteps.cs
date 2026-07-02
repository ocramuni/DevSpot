using DevSpot.Claude.AcceptanceTests.Fixtures;
using DevSpot.Claude.AcceptanceTests.Infrastructure.Authentication;
using DevSpot.Claude.AcceptanceTests.Support.Assertions;
using DevSpot.Claude.AcceptanceTests.Support.Forms;
using DevSpot.Claude.AcceptanceTests.Support.Http;
using DevSpot.Data;
using DevSpot.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using Xunit;

namespace DevSpot.Claude.AcceptanceTests.Steps.Domain;

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

    // ── Nuovi step per creazione diretta nel DB, cancellazione e verifica ─────

    /// <summary>
    /// Inserisce direttamente nel database un annuncio intestato all'utente
    /// identificato dal profilo di autenticazione (es. "employer", "admin").
    /// Non passa per il form HTTP: utile per preparare lo stato iniziale
    /// di scenari focalizzati su operazioni diverse dalla creazione.
    /// </summary>
    [Given(@"nel database esiste un annuncio con titolo {string} dell'utente {string}")]
    public async Task CreateJobPostingInDb(string title, string userProfile)
    {
        var (email, _) = AuthProfile.CredentialsFor(userProfile);

        using var scope = _fixture.Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = await userManager.FindByEmailAsync(email)
            ?? throw new InvalidOperationException(
                $"Utente con profilo '{userProfile}' (email: {email}) non trovato nel database.");

        var posting = new JobPosting
        {
            Title = title,
            Description = "Descrizione di test",
            Company = "Azienda di test",
            Location = "Luogo di test",
            UserId = user.Id
        };

        db.JobPosting.Add(posting);
        await db.SaveChangesAsync();
    }

    [When(@"invio la richiesta di cancellazione per l'annuncio {string}")]
    public async Task SendDeleteRequest(string title)
    {
        using var scope = _fixture.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var posting = await db.JobPosting.FirstAsync(jp => jp.Title == title);
        await _http.DeleteAsync($"/JobPostings/Delete/{posting.Id}");
    }

    [Then(@"l'annuncio {string} non è più presente nel database")]
    public async Task AssertJobPostingDeleted(string title)
    {
        using var scope = _fixture.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var exists = await db.JobPosting.AnyAsync(jp => jp.Title == title);
        Assert.False(exists,
            $"L'annuncio '{title}' dovrebbe essere stato cancellato ma è ancora presente nel database.");
    }
}
