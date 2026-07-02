using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace DevSpot.Claude.AcceptanceTests.Support.Html;

/// <summary>
/// Thin wrapper over AngleSharp for parsing server-rendered HTML.
/// Provides the primitives that FormSession and assertion helpers need.
/// </summary>
public static class HtmlDocumentParser
{
    private static readonly IConfiguration _config = Configuration.Default;

    /// <param name="html">Raw HTML string.</param>
    /// <param name="baseUrl">
    /// Optional base URL (e.g. "http://localhost/JobPostings/Create").
    /// Providing this lets AngleSharp resolve relative hrefs and form actions correctly.
    /// </param>
    public static async Task<IDocument> ParseAsync(string html, string? baseUrl = null)
    {
        var context = BrowsingContext.New(_config);
        return await context.OpenAsync(req =>
        {
            req.Content(html);
            if (baseUrl is not null)
                req.Address(baseUrl);
        });
    }

    /// <summary>
    /// Extracts the ASP.NET Core antiforgery token from the first hidden
    /// input named __RequestVerificationToken found in the document.
    /// Returns null if not present (MVC actions without [ValidateAntiForgeryToken]).
    /// </summary>
    public static string? ExtractAntiForgeryToken(IDocument document)
    {
        var input = document.QuerySelector("input[name='__RequestVerificationToken']");
        return input?.GetAttribute("value");
    }

    /// <summary>
    /// Returns all input/textarea/select fields within <paramref name="form"/>
    /// as a name→value dictionary, skipping submit buttons.
    /// </summary>
    public static Dictionary<string, string> ExtractFormFields(IHtmlFormElement form)
    {
        var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var element in form.Elements)
        {
            if (element is IHtmlInputElement input)
            {
                if (input.Type is "submit" or "button" or "image" or "reset")
                    continue;
                if (string.IsNullOrEmpty(input.Name))
                    continue;
                // Antiforgery token is managed separately via ExtractAntiForgeryToken.
                // Including it here causes it to be sent twice, breaking validation.
                if (string.Equals(input.Name, "__RequestVerificationToken", StringComparison.OrdinalIgnoreCase))
                    continue;
                // For radio buttons only include the checked option; including all of them
                // would overwrite the checked value with the last unchecked one in the group.
                if (input.Type == "radio" && !input.IsChecked)
                    continue;
                fields[input.Name] = input.Value ?? string.Empty;
            }
            else if (element is IHtmlTextAreaElement textarea && !string.IsNullOrEmpty(textarea.Name))
            {
                fields[textarea.Name] = textarea.Value ?? string.Empty;
            }
            else if (element is IHtmlSelectElement select && !string.IsNullOrEmpty(select.Name))
            {
                fields[select.Name] = select.Value ?? string.Empty;
            }
        }

        return fields;
    }

    /// <summary>
    /// Finds the first form that either has the given id, or whose action
    /// contains <paramref name="actionHint"/> (case-insensitive).
    /// Falls back to the first form if no hint is provided.
    /// </summary>
    public static IHtmlFormElement? FindForm(IDocument document, string? actionHint = null)
    {
        var forms = document.QuerySelectorAll<IHtmlFormElement>("form");

        if (actionHint is null)
            return forms.FirstOrDefault();

        return forms.FirstOrDefault(f =>
            (f.Id?.Contains(actionHint, StringComparison.OrdinalIgnoreCase) ?? false)
            || (f.Action?.Contains(actionHint, StringComparison.OrdinalIgnoreCase) ?? false));
    }

    /// <summary>
    /// Returns true if the rendered HTML contains a validation-summary or
    /// field-level validation span that references <paramref name="fieldName"/>.
    /// </summary>
    public static bool HasValidationErrorFor(IDocument document, string fieldName)
    {
        // Field-level: <span asp-validation-for="FieldName" ...>
        var fieldSpan = document.QuerySelector($"span[data-valmsg-for='{fieldName}']");
        if (fieldSpan != null && !string.IsNullOrWhiteSpace(fieldSpan.TextContent))
            return true;

        // Validation summary
        var summary = document.QuerySelector("[data-valmsg-summary]");
        if (summary != null &&
            summary.TextContent.Contains(fieldName, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    /// <summary>Returns true if any element in the document contains <paramref name="text"/>.</summary>
    public static bool ContainsText(IDocument document, string text) =>
        document.Body?.TextContent.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false;

    /// <summary>Returns true if the document contains an element matching <paramref name="selector"/>.</summary>
    public static bool HasElement(IDocument document, string selector) =>
        document.QuerySelector(selector) is not null;
}
