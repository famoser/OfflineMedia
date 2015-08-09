using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Framework.Generic;
using OfflineMediaV3.Business.Models;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Framework.Services.Interfaces;
using OfflineMediaV3.Data;
using OfflineMediaV3.Data.Entities;
using SQLite.Net.Async;

namespace OfflineMediaV3.Business.Framework.Repositories
{
    public class GenericRepository<TBusiness, TEntity>
        where TBusiness : BaseModel, new()
        where TEntity : EntityBase, new()
    {
        private readonly EntityModelConverter _entityBusinessConverter;
        private readonly IDataService _dataService;

        public GenericRepository(IDataService dataService)
        {
            _entityBusinessConverter = new EntityModelConverter();
            _dataService = dataService;
        }

        public async Task<TBusiness> GetById(int id)
        {
            try
            {
                var entity = await _dataService.GetById<TEntity>(id);

                if (entity != null)
                {
                    var business = new TBusiness();
                    business = _entityBusinessConverter.ConvertToBusiness(entity, business);

                    return business;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = String.Format("Exception Occured while trying to Get Generic Type from Database. Id: '{0}', Entity Type: '{1}', Business Type: '{2}'", id, typeof(TEntity), typeof(TBusiness));
                LogHelper.Instance.Log(LogLevel.Error, this, errorMsg, ex);
            }
            return null;
        }

        public async Task<bool> Delete(TBusiness business)
        {
            try
            {
                var id = _entityBusinessConverter.GetPrimaryKeyFromBusiness(business);

                return await _dataService.DeleteById<TEntity>(id);
            }
            catch (Exception ex)
            {
                string errorMsg = String.Format("Exception Occured while trying to Delete Generic Type from Database. Entity Type: '{0}', Business Type: '{1}'", typeof(TEntity), typeof(TBusiness));
                LogHelper.Instance.Log(LogLevel.Error, this, errorMsg, ex);
            }
            return false;
        }

        public async Task<List<TBusiness>> GetByCondition(Expression<Func<TEntity, bool>> func, Expression<Func<TEntity, object>> orderByProperty = null, bool descending = false, int limit = 0)
        {
            try
            {
                var entityList = await _dataService.GetByCondition(func, orderByProperty, descending, limit);

                if (entityList.Any())
                {
                    var list = new List<TBusiness>();
                    foreach (var entity in entityList)
                    {
                        var business = new TBusiness();
                        business = _entityBusinessConverter.ConvertToBusiness(entity, business);
                        list.Add(business);
                    }

                    return list;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = String.Format("Exception Occured while trying to Get Generic Type from Database. Entity Type: '{0}', Business Type: '{1}'", typeof(TEntity), typeof(TBusiness));
                LogHelper.Instance.Log(LogLevel.Error, this, errorMsg, ex);
            }
            return new List<TBusiness>();
        }

        public async Task<int> CountByCondition(Expression<Func<TEntity, bool>> func)
        {
            try
            {
                return await _dataService.CountByCondition(func);
            }
            catch (Exception ex)
            {
                string errorMsg = String.Format("Exception Occured while trying to CountByCondition from Database. Entity Type: '{0}', Business Type: '{1}'", typeof(TEntity), typeof(TBusiness));
                LogHelper.Instance.Log(LogLevel.Error, this, errorMsg, ex);
            }
            return 0;
        }

        public async Task<int> AddOrUpdate(TBusiness business)
        {
            try
            {
                var id = _entityBusinessConverter.GetPrimaryKeyFromBusiness(business);
                TEntity entity = null;
                if (id != 0)
                    entity = await _dataService.GetById<TEntity>(id);

                if (entity != null)
                {
                    entity = _entityBusinessConverter.ConvertToEntity(business, entity, true, false);
                    return await _dataService.AddOrUpdate(entity) ? entity.Id : -1;
                }

                int newId = (await _dataService.GetHighestId<TEntity>()) + 1;
                _entityBusinessConverter.SetPrimaryKeyToBusiness(business, newId);
                entity = _entityBusinessConverter.ConvertToEntity(business, new TEntity(), true, true);
                return await _dataService.AddOrUpdate(entity) ? newId : -1;
            }
            catch (Exception ex)
            {
                string errorMsg = String.Format("Exception Occured while trying to AddOrUpdate Generic Type from Database. Entity Type: '{0}', Business Type: '{1}'", typeof(TEntity), typeof(TBusiness));
                LogHelper.Instance.Log(LogLevel.Error, this, errorMsg, ex);
            }
            return -1;
        }
    }
}
