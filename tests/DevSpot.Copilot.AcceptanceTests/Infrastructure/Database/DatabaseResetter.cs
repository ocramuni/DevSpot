using DevSpot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DevSpot.Copilot.AcceptanceTests.Infrastructure.Database;

public sealed class DatabaseResetter
{
    public async Task ResetAsync(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.JobPosting.ExecuteDeleteAsync();
    }
}
