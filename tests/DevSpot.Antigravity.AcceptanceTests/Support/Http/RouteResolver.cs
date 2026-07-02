using System;

namespace DevSpot.Antigravity.AcceptanceTests.Support.Http
{
    public class RouteResolver
    {
        public string Resolve(string pageName)
        {
            return pageName.ToLowerInvariant() switch
            {
                "index" or "home" or "job postings" or "job postings index" => "/JobPostings",
                "create job posting" or "create job" => "/JobPostings/Create",
                "register" or "registration" => "/Identity/Account/Register",
                "login" or "sign in" => "/Identity/Account/Login",
                _ => throw new ArgumentException($"Unknown page name: {pageName}")
            };
        }
    }
}
