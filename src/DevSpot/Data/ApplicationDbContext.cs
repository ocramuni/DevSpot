using DevSpot.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<JobPosting> JobPosting { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
    }
}
