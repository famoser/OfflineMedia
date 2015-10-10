using System;
using System.Threading.Tasks;

namespace OfflineMedia.Business.Framework
{
    public class UnitOfWork : IDisposable
    {
        private SqliteDataService _dataService;
        private bool _onlyRead;

        public UnitOfWork(bool onlyRead)
        {
            _onlyRead = onlyRead;
        }

        public async Task<SqliteDataService> GetDataService()
        {
            if (_dataService == null)
            {
                _dataService = await SqliteDataService.GetInstance();
                await _dataService.BeginTransaction(_onlyRead);
            }
            return _dataService;
        }

        public async Task Commit()
        {
            if (_dataService != null)
            {
                await _dataService.CommitTransaction();
                _dataService = null;
            }
        }

        public void Dispose()
        {
            if (_dataService != null)
            {
                _dataService.RollbackTransaction(_onlyRead);
            }
        }
    }
}
