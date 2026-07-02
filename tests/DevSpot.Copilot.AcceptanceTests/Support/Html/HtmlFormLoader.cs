using AngleSharp;
using AngleSharp.Dom;
using DevSpot.Copilot.AcceptanceTests.Support.Forms;

namespace DevSpot.Copilot.AcceptanceTests.Support.Html;

public sealed class HtmlFormLoader
{
    public async Task<HtmlFormSession> LoadAsync(HttpResponseMessage response)
    {
        return await TryLoadAsync(response) ?? throw new InvalidOperationException("No form element was found in the response.");
    }

    public async Task<HtmlFormSession?> TryLoadAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var address = response.RequestMessage?.RequestUri ?? new Uri("http://localhost/");
        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(request => request.Content(content).Address(address.ToString()));
        var form = document.QuerySelector("form");

        return form is null ? null : CreateSession(form, address);
    }

    private static HtmlFormSession CreateSession(IElement form, Uri pageAddress)
    {
        var method = string.Equals(form.GetAttribute("method"), "get", StringComparison.OrdinalIgnoreCase)
            ? HttpMethod.Get
            : HttpMethod.Post;

        var actionAttribute = form.GetAttribute("action");
        var action = string.IsNullOrWhiteSpace(actionAttribute)
            ? pageAddress
            : new Uri(pageAddress, actionAttribute);

        var session = new HtmlFormSession(action, method);

        foreach (var field in form.QuerySelectorAll("input, textarea, select"))
        {
            var name = field.GetAttribute("name");
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            var value = GetFieldValue(field);
            if (value is null)
            {
                continue;
            }

            session.SetField(name, value);
        }

        return session;
    }

    private static string? GetFieldValue(IElement element)
    {
        var tagName = element.TagName.ToLowerInvariant();
        return tagName switch
        {
            "textarea" => element.TextContent,
            "select" => GetSelectValue(element),
            _ => GetInputValue(element)
        };
    }

    private static string? GetSelectValue(IElement element)
    {
        var selectedOption = element.QuerySelector("option[selected]") ?? element.QuerySelector("option");
        return selectedOption?.GetAttribute("value") ?? selectedOption?.TextContent;
    }

    private static string? GetInputValue(IElement element)
    {
        var type = element.GetAttribute("type")?.ToLowerInvariant();
        if (type is "checkbox")
        {
            return element.HasAttribute("checked") ? element.GetAttribute("value") ?? "true" : "false";
        }

        if (type is "radio")
        {
            return element.HasAttribute("checked") ? element.GetAttribute("value") : null;
        }

        return element.GetAttribute("value") ?? string.Empty;
    }
}
