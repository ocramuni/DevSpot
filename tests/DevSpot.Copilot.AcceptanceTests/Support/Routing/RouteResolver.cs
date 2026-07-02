namespace DevSpot.Copilot.AcceptanceTests.Support.Routing;

public sealed class RouteResolver
{
    private static readonly Dictionary<string, string> Routes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Accesso"] = "/Identity/Account/Login",
        ["Login"] = "/Identity/Account/Login",
        ["Registrazione"] = "/Identity/Account/Register",
        ["Register"] = "/Identity/Account/Register",
        ["Nuovo annuncio"] = "/JobPostings/Create",
        ["Create Job Posting"] = "/JobPostings/Create",
        ["Create"] = "/JobPostings/Create",
        ["Offerte di lavoro"] = "/",
        ["Tutti gli annunci"] = "/",
        ["All Jobs"] = "/",
        ["Index"] = "/",
        ["Home"] = "/"
    };

    public string Resolve(string pageName)
    {
        if (Routes.TryGetValue(pageName, out var route))
        {
            return route;
        }

        throw new InvalidOperationException($"Non esiste alcuna corrispondenza di route per la pagina '{pageName}'.");
    }
}
