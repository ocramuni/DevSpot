using Microsoft.Data.Sqlite;
using DevSpot.AcceptanceTests.Infrastructure.Hosting;

namespace DevSpot.AcceptanceTests.Fixtures
{
    public class AcceptanceTestFixture : IDisposable
    {
        private readonly SqliteConnection _sqliteConnection;
        public CustomWebApplicationFactory Factory { get; }

        public AcceptanceTestFixture()
        {
            _sqliteConnection = new SqliteConnection("DataSource=:memory:");
            _sqliteConnection.Open();

            Factory = new CustomWebApplicationFactory(_sqliteConnection);
        }

        public void Dispose()
        {
            Factory.Dispose();
            _sqliteConnection.Close();
            _sqliteConnection.Dispose();
        }
    }
}
