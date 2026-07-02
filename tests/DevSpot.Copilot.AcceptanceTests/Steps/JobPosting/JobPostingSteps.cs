using DevSpot.Copilot.AcceptanceTests.Fixtures;
using DevSpot.Copilot.AcceptanceTests.Support.State;
using DevSpot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;

namespace DevSpot.Copilot.AcceptanceTests.Steps.JobPosting;

[Binding]
public sealed class JobPostingSteps
{
    private readonly AcceptanceTestFixture _fixture;
    private readonly ScenarioHttpState _httpState;

    public JobPostingSteps(AcceptanceTestFixture fixture, ScenarioHttpState httpState)
    {
        _fixture = fixture;
        _httpState = httpState;
    }

    [When(@"cancello l'offerta di lavoro intitolata ""(.*)""")]
    public async Task WhenIDeleteTheJobPostingTitled(string title)
    {
        var client = _httpState.Client ?? throw new InvalidOperationException("Nessun client dello scenario disponibile.");

        using var scope = _fixture.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var jobPosting = await dbContext.JobPosting.FirstOrDefaultAsync(jobPosting => jobPosting.Title == title)
            ?? throw new InvalidOperationException($"Non è stata trovata alcuna offerta di lavoro con il titolo '{title}'.");

        var response = await client.DeleteAsync($"/JobPostings/Delete/{jobPosting.Id}");

        _httpState.LastResponse?.Dispose();
        _httpState.LastResponse = response;
        _httpState.LastHtml = await response.Content.ReadAsStringAsync();
        _httpState.CurrentForm = null;
    }
}