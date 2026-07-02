using AngleSharp.Html.Dom;
using DevSpot.Claude.AcceptanceTests.Support.Html;
using DevSpot.Claude.AcceptanceTests.Support.Http;
using DevSpot.Claude.AcceptanceTests.Support.State;

namespace DevSpot.Claude.AcceptanceTests.Support.Forms;

/// <summary>
/// Manages the lifecycle of a single HTML form interaction:
/// load → discover → fill → submit.
/// Antiforgery token extraction is automatic; step definitions
/// never see or touch it.
/// </summary>
public sealed class FormSession
{
    private readonly HttpClientSession _http;
    private readonly ScenarioState _state;

    private string? _formAction;
    private Dictionary<string, string> _fields = new(StringComparer.OrdinalIgnoreCase);
    private string? _antiForgeryToken;
    private bool _formLoaded;

    public FormSession(HttpClientSession http, ScenarioState state)
    {
        _http = http;
        _state = state;
    }

    /// <summary>
    /// GETs <paramref name="url"/>, finds the first form (or the one matching
    /// <paramref name="formHint"/>), and prepopulates fields from the HTML.
    /// </summary>
    public async Task LoadAsync(string url, string? formHint = null)
    {
        await _http.GetAsync(url);

        var baseUrl = $"http://localhost{url}";
        var doc = await HtmlDocumentParser.ParseAsync(_state.LastResponseContent!, baseUrl);

        var form = HtmlDocumentParser.FindForm(doc, formHint)
            ?? throw new InvalidOperationException(
                $"No form found on page '{url}' (hint: '{formHint}').");

        _antiForgeryToken = HtmlDocumentParser.ExtractAntiForgeryToken(doc);

        // AngleSharp returns the raw attribute value for form.Action (not the resolved URL).
        // Resolve manually: relative action resolved against the loaded page URL.
        var rawAction = form.Action;
        if (string.IsNullOrWhiteSpace(rawAction))
        {
            _formAction = url;
        }
        else
        {
            var pageUri = new Uri("http://localhost" + url);
            _formAction = new Uri(pageUri, rawAction).PathAndQuery;
        }

        _fields = HtmlDocumentParser.ExtractFormFields(form);
        _formLoaded = true;
    }

    /// <summary>Sets a form field value by its HTML name attribute.</summary>
    public void Fill(string fieldName, string value)
    {
        EnsureLoaded();
        _fields[fieldName] = value;
    }

    /// <summary>Sets multiple form fields from a name→value table.</summary>
    public void FillMany(IEnumerable<(string Name, string Value)> pairs)
    {
        foreach (var (name, value) in pairs)
            Fill(name, value);
    }

    /// <summary>
    /// Clears a field (sets it to empty string), simulating a user
    /// leaving a required field blank.
    /// </summary>
    public void Clear(string fieldName)
    {
        EnsureLoaded();
        _fields[fieldName] = string.Empty;
    }

    /// <summary>POSTs the form to its action URL, including the antiforgery token if present.</summary>
    public async Task<HttpResponseMessage> SubmitAsync()
    {
        EnsureLoaded();

        var data = new List<KeyValuePair<string, string>>(
            _fields.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value)));

        if (_antiForgeryToken is not null)
            data.Add(new("__RequestVerificationToken", _antiForgeryToken));

        return await _http.PostFormAsync(_formAction!, data);
    }

    private void EnsureLoaded()
    {
        if (!_formLoaded)
            throw new InvalidOperationException(
                "FormSession: LoadAsync() must be called before filling or submitting.");
    }
}
