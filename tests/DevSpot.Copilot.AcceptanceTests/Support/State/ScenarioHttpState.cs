using DevSpot.Copilot.AcceptanceTests.Support.Forms;

namespace DevSpot.Copilot.AcceptanceTests.Support.State;

public sealed class ScenarioHttpState
{
    public HttpClient? Client { get; set; }

    public HttpResponseMessage? LastResponse { get; set; }

    public string? LastHtml { get; set; }

    public HtmlFormSession? CurrentForm { get; set; }

    public void Reset()
    {
        Client = null;
        LastResponse = null;
        LastHtml = null;
        CurrentForm = null;
    }
}
