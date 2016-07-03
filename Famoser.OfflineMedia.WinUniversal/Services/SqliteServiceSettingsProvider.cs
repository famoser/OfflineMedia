using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Famoser.SqliteWrapper.Services.Interfaces;
using SQLite.Net.Async;

namespace Famoser.OfflineMedia.WinUniversal.Services
{
    public class SqliteServiceSettingsProvider : ISqliteServiceSettingsProvider
    {
        public async Task<string> GetFullPathOfDatabase()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("database.sqlite3", CreationCollisionOption.OpenIfExists);
            return file.Path;
        }

        public int GetApplicationId()
        {
            return 870706572;
        }

        public async Task DoMigration(SQLiteAsyncConnection connection)
        {
            //check for database version (initial value is 0)
            var version = await connection.ExecuteScalarAsync<int>("PRAGMA user_version");
            if (version == 0)
            {
                //do any migration needed
                //await connection.ExecuteAsync("DELETE * FROM MyModel");

                //increment database version
                await connection.ExecuteAsync("PRAGMA user_version = 1");
            }
        }
    }
}
