namespace DevSpot.AcceptanceTests.Infrastructure.Authentication;

/// <summary>
/// Known test authentication profiles that map to seeded users.
/// </summary>
public static class AuthProfile
{
    public const string Admin = "admin";
    public const string Employer = "employer";
    public const string JobSeeker = "jobseeker";

    public static (string Email, string Password) CredentialsFor(string profile) =>
        profile.ToLowerInvariant() switch
        {
            Admin     => ("admin@devspot.com",     "Admin123!"),
            Employer  => ("employer@devspot.com",  "Employer123!"),
            JobSeeker => ("jobseeker@devspot.com", "JobSeeker123!"),
            _         => throw new InvalidOperationException($"Unknown auth profile: '{profile}'")
        };
}
