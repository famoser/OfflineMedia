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
        Task<bool> AddOrUpdate<T>(T entity) where T : class, new();
        Task<int> GetHighestId<T>() where T : EntityBase, new();

        Task<List<TEntity>> GetByCondition<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> func, System.Linq.Expressions.Expression<Func<TEntity, object>> orderByProperty, bool descending, int limit) where TEntity : class, new();

        Task<int> CountByCondition<T>(Expression<Func<T, bool>> func) where T : class, new();
    }
}
