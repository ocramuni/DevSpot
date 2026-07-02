using System;
using System.Threading.Tasks;
using Xunit;
using Reqnroll;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using DevSpot.Antigravity.AcceptanceTests.Infrastructure.Hosting;

namespace DevSpot.Antigravity.AcceptanceTests.Steps.Domain
{
    [Binding]
    public class AccountSteps
    {
        private readonly CustomWebApplicationFactory _factory;

        public AccountSteps(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Then(@"un utente con email ""(.*)"" dovrebbe esistere")]
        public async Task UserShouldExist(string email)
        {
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);
        }

        [Then(@"l'utente ""(.*)"" dovrebbe avere il ruolo ""(.*)""")]
        public async Task UserShouldHaveRole(string email, string roleName)
        {
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);

            var hasRole = await userManager.IsInRoleAsync(user, roleName);
            Assert.True(hasRole, $"L'utente '{email}' non possiede il ruolo '{roleName}'.");
        }
    }
}
