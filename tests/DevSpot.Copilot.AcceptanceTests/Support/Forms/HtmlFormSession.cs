using System.Net.Http.Headers;

namespace DevSpot.Copilot.AcceptanceTests.Support.Forms;

public sealed class HtmlFormSession
{
    private readonly Dictionary<string, string> _fields = new(StringComparer.OrdinalIgnoreCase);

    public HtmlFormSession(Uri action, HttpMethod method)
    {
        Action = action;
        Method = method;
    }

    public Uri Action { get; }

    public HttpMethod Method { get; }

    public IReadOnlyDictionary<string, string> Fields => _fields;

    public void SetField(string name, string value)
    {
        _fields[name] = value;
    }

    public void RemoveField(string name)
    {
        _fields.Remove(name);
    }

    public FormUrlEncodedContent ToContent()
    {
        return new FormUrlEncodedContent(_fields);
    }
}
