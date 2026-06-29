using DevSpot.Data;
using DevSpot.Models;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Repositories
{
    public class JobPostingRepository : IRepository<JobPosting>
    {
        private readonly ApplicationDbContext _context;
        public JobPostingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(JobPosting entity)
        {
            await _context.JobPosting.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var jobPosting = await _context.JobPosting.FindAsync(id);

            if (jobPosting == null)
            {
                throw new KeyNotFoundException();
            }

            _context.JobPosting.Remove(jobPosting);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<JobPosting>> GetAllAsync()
        {
            return await _context.JobPosting.ToListAsync();
        }

        public async Task<JobPosting> GetByIdAsync(int id)
        {
            var jobPosting = await _context.JobPosting.FindAsync(id);

            if (jobPosting == null)
            {
                throw new KeyNotFoundException();
            }

            return jobPosting;
        }

        public async Task UpdateAsync(JobPosting entity)
        {
            _context.JobPosting.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
