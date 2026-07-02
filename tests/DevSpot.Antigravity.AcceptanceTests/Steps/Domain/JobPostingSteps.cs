using System.Threading.Tasks;
using Xunit;
using Reqnroll;
using DevSpot.Data;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Antigravity.AcceptanceTests.Steps.Domain
{
    [Binding]
    public class JobPostingSteps
    {
        private readonly ApplicationDbContext _dbContext;

        public JobPostingSteps(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
