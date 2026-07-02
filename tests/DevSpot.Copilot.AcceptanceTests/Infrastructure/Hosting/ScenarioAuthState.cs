using DevSpot.Constants;
using Microsoft.AspNetCore.Identity;

namespace DevSpot.Copilot.AcceptanceTests.Infrastructure.Hosting;

public enum ScenarioAuthProfile
{
    Anonymous,
    Employer,
    JobSeeker,
    Admin
}

public sealed class ScenarioAuthState
{
    public static ScenarioAuthState Current { get; } = new();

    public ScenarioAuthProfile Profile { get; set; } = ScenarioAuthProfile.Anonymous;

    public IdentityUser? User { get; set; }

    public void Reset()
    {
        Profile = ScenarioAuthProfile.Anonymous;
        User = null;
    }

    public bool IsAuthenticated => Profile != ScenarioAuthProfile.Anonymous && User != null;

    public string? GetRole()
    {
        return Profile switch
        {
            ScenarioAuthProfile.Admin => Roles.Admin,
            ScenarioAuthProfile.Employer => Roles.Employer,
            ScenarioAuthProfile.JobSeeker => Roles.JobSeeker,
            _ => null
        };
    }
}
