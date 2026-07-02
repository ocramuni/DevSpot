using System;
using System.Threading.Tasks;
using Xunit;
using Reqnroll;
using DevSpot.Data;
using DevSpot.Models;
using Microsoft.EntityFrameworkCore;
using DevSpot.Antigravity.AcceptanceTests.Support.State;

namespace DevSpot.Antigravity.AcceptanceTests.Steps.Domain
{
    [Binding]
    public class JobPostingSteps
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ScenarioState _state;

        public JobPostingSteps(ApplicationDbContext dbContext, ScenarioState state)
        {
            _dbContext = dbContext;
            _state = state;
        }

        [Given(@"(?:che )?esiste un annuncio di lavoro con titolo ""(.*)"" inserito da ""(.*)""")]
        public async Task GivenAJobPostingExistsWithTitleAndUser(string title, string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new InvalidOperationException($"L'utente con email '{email}' non esiste nel database.");
            }

            var jobPosting = new JobPosting
            {
                Title = title,
                Description = "Descrizione di prova dell'annuncio",
                Company = "Azienda di prova",
                Location = "Milano",
                PostedDate = DateTime.Now,
                IsApproved = true,
                UserId = user.Id
            };

            await _dbContext.JobPosting.AddAsync(jobPosting);
            await _dbContext.SaveChangesAsync();
        }

        [When(@"elimino l'annuncio con titolo ""(.*)""")]
        public async Task WhenIDeleteJobPostingByTitle(string title)
        {
            if (_state.HttpClient == null)
            {
                throw new InvalidOperationException("HttpClient non è stato inizializzato.");
            }

            var jobPosting = await _dbContext.JobPosting.FirstOrDefaultAsync(jp => jp.Title == title);
            if (jobPosting == null)
            {
                throw new InvalidOperationException($"L'annuncio di lavoro con titolo '{title}' non è stato trovato per l'eliminazione.");
            }

            var url = $"/JobPostings/Delete/{jobPosting.Id}";
            var response = await _state.HttpClient.DeleteAsync(url);

            _state.LastResponse = response;
            _state.LastResponseContent = await response.Content.ReadAsStringAsync();
        }

        [Then(@"dovrei vedere l'annuncio con titolo ""(.*)"" nella pagina")]
        public void ThenIShouldSeeJobPostingWithTitleOnPage(string title)
        {
            Assert.NotNull(_state.LastResponseContent);
            Assert.Contains(title, _state.LastResponseContent, StringComparison.OrdinalIgnoreCase);
        }

        [Then(@"non dovrei vedere l'annuncio con titolo ""(.*)"" nella pagina")]
        public void ThenIShouldNotSeeJobPostingWithTitleOnPage(string title)
        {
            Assert.NotNull(_state.LastResponseContent);
            Assert.DoesNotContain(title, _state.LastResponseContent, StringComparison.OrdinalIgnoreCase);
        }

        [Then(@"un annuncio di lavoro con titolo ""(.*)"" dovrebbe esistere nel sistema")]
        public async Task JobPostingShouldExist(string title)
        {
            var exists = await _dbContext.JobPosting.AnyAsync(jp => jp.Title == title);
            Assert.True(exists, $"L'annuncio di lavoro con titolo '{title}' non è stato trovato nel database.");
        }

        [Then(@"un annuncio di lavoro con titolo ""(.*)"" non dovrebbe esistere nel sistema")]
        public async Task JobPostingShouldNotExist(string title)
        {
            var exists = await _dbContext.JobPosting.AnyAsync(jp => jp.Title == title);
            Assert.False(exists, $"L'annuncio di lavoro con titolo '{title}' è stato trovato nel database, ma non dovrebbe esistere.");
        }
    }
}
