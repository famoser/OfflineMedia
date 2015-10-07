using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using OfflineMediaV3.Data.Entities;

namespace OfflineMediaV3.Data
{
    public interface IDataService
    {
        Task<T> GetById<T>(int id) where T : class, new();
        Task<bool> DeleteById<T>(int id) where T : class, new();
        Task<bool> DeleteAllById<T>(List<int> ids) where T : EntityIdBase, new();
        Task<int> Add<T>(T entity) where T : EntityIdBase, new();
        Task<List<int>> AddAll<T>(List<T> entity) where T : EntityIdBase, new();
        Task<bool> UpdateAll<T>(List<T> entity) where T : EntityIdBase, new();
        Task<bool> Update<T>(T entity) where T : EntityIdBase, new();
        Task<int> GetHighestId<T>() where T : EntityIdBase, new();

        Task<List<T>> GetByCondition<T>(Expression<Func<T, bool>> func, Expression<Func<T, object>> orderByProperty, bool descending, int limit, int skip) where T : class, new();

        Task<int> CountByCondition<T>(Expression<Func<T, bool>> func) where T : class, new();

        Task<List<int>> GetByKeyword(string keyword);
        Task<bool> DeleteArticlesById(IEnumerable<int> enumerable);
    }
}
