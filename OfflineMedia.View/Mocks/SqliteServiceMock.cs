using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Famoser.SqliteWrapper.Entities;
using Famoser.SqliteWrapper.Services.Interfaces;

namespace Famoser.OfflineMedia.View.Mocks
{
    class SqliteServiceMock : ISqliteService
    {
        public Task<T> GetById<T>(int id) where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetAllById<T>(IEnumerable<int> ids) where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetAll<T>() where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteById<T>(int id) where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllById<T>(IEnumerable<int> ids) where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAll<T>() where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<int> Add<T>(T entity) where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<int> AddAll<T>(IEnumerable<T> entity) where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<int> Update<T>(T entity) where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAll<T>(IEnumerable<T> entity) where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetHighestId<T>() where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetByCondition<T>(Expression<Func<T, bool>> func, Expression<Func<T, object>> orderByProperty, bool @descending, int limit, int skip) where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<int> CountByCondition<T>(Expression<Func<T, bool>> func) where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteAsync<T>(string query, params object[] args) where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task<T> ExecuteScalarAsync<T>(string query, params object[] args) where T : BaseEntity, new()
        {
            throw new NotImplementedException();
        }
    }
}
