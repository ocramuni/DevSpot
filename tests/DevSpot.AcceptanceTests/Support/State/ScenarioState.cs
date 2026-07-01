using System.Net.Http;
using DevSpot.AcceptanceTests.Support.Forms;

namespace DevSpot.AcceptanceTests.Support.State
{
    public class ScenarioState
    {
        public HttpClient? HttpClient { get; set; }
        public string? CurrentUserEmail { get; set; }
        public string? CurrentUserRole { get; set; }
        public HttpResponseMessage? LastResponse { get; set; }
        public string? LastResponseContent { get; set; }
        public FormSession? CurrentForm { get; set; }
    }
}
