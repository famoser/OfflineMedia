using System.Threading.Tasks;
using Famoser.SqliteWrapper.Services.Interfaces;
using SQLite.Net.Async;

#pragma warning disable 1998
namespace Famoser.OfflineMedia.UnitTests.Services.Mocks
{
    class SqliteServiceSettingsProviderMock : ISqliteServiceSettingsProvider
    {
        public async Task<string> GetFullPathOfDatabase()
        {
            return "test_db.sqlite3";
        }

        public int GetApplicationId()
        {
            return 5;
        }

        public async Task DoMigration(SQLiteAsyncConnection connection)
        {
        }
    }
}
