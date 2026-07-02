using DevSpot.Data;
using DevSpot.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DevSpot.Copilot.AcceptanceTests.Infrastructure.Database;

public sealed class DatabaseSeeder
{
    public async Task SeedBaseAsync(IServiceProvider serviceProvider)
    {
        await RoleSeeder.RoleSeederAsync(serviceProvider);
        await UserSeeder.SeedUserAsync(serviceProvider);
    }

    public async Task SeedJobPostingsAsync(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        if (await dbContext.JobPosting.AnyAsync())
        {
            return;
        }

        var admin = await userManager.FindByEmailAsync("admin@devspot.com") ?? throw new InvalidOperationException("Seeded admin user was not found.");
        var employer = await userManager.FindByEmailAsync("employer@devspot.com") ?? throw new InvalidOperationException("Seeded employer user was not found.");

        dbContext.JobPosting.AddRange(
            new JobPosting
            {
                Title = "Employer Seed Posting",
                Description = "Seeded employer posting",
                Company = "DevSpot",
                Location = "Remote",
                IsApproved = true,
                UserId = employer.Id
            },
            new JobPosting
            {
                Title = "Admin Seed Posting",
                Description = "Seeded admin posting",
                Company = "DevSpot",
                Location = "Remote",
                IsApproved = true,
                UserId = admin.Id
            });

        await dbContext.SaveChangesAsync();
    }
}
