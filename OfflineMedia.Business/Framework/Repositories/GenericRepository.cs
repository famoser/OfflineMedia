using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using OfflineMedia.Business.Framework.Generic;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Data;
using OfflineMedia.Data.Entities;

namespace OfflineMedia.Business.Framework.Repositories
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

        public async Task<List<TBusiness>> GetByCondition(Expression<Func<TEntity, bool>> func, Expression<Func<TEntity, object>> orderByProperty = null, bool descending = false, int limit = 0, int skip = 0)
        {
            try
            {
                var entityList = await _dataService.GetByCondition(func, orderByProperty, descending, limit, skip);

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

        public async Task<bool> Add(TBusiness business)
        {
            try
            {
                var entity = _entityBusinessConverter.ConvertToEntity(business, new TEntity(), true);
                int id = await _dataService.Add(entity);
                if (id != -1)
                {
                    business.Id = id;
                    return true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = String.Format("Exception Occured while trying to AddOrUpdate Generic Type from Database. Entity Type: '{0}', Business Type: '{1}'", typeof(TEntity), typeof(TBusiness));
                LogHelper.Instance.Log(LogLevel.Error, this, errorMsg, ex);
            }
            return false;
        }

        public async Task<bool> AddAll(List<TBusiness> business)
        {
            try
            {
                List<TEntity> list = new List<TEntity>();
                foreach (var business1 in business)
                {
                    list.Add(_entityBusinessConverter.ConvertToEntity(business1, new TEntity(), true));
                }
                var res = await _dataService.AddAll(list);
                if (res.Count == business.Count)
                {
                    for (int index = 0; index < business.Count; index++)
                    {
                        business[index].Id = res[index];
                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
                string errorMsg = String.Format("Exception Occured while trying to AddOrUpdate Generic Type from Database. Entity Type: '{0}', Business Type: '{1}'", typeof(TEntity), typeof(TBusiness));
                LogHelper.Instance.Log(LogLevel.Error, this, errorMsg, ex);
            }
            return false;
        }

        public async Task<bool> Update(TBusiness business)
        {
            try
            {
                var entity = _entityBusinessConverter.ConvertToEntity(business, new TEntity(), true);
                return await _dataService.Update(entity);
            }
            catch (Exception ex)
            {
                string errorMsg = String.Format("Exception Occured while trying to AddOrUpdate Generic Type from Database. Entity Type: '{0}', Business Type: '{1}'", typeof(TEntity), typeof(TBusiness));
                LogHelper.Instance.Log(LogLevel.Error, this, errorMsg, ex);
            }
            return false;
        }

        public async Task<bool> UpdateAll(List<TBusiness> business)
        {
            try
            {
                List<TEntity> list = new List<TEntity>();
                foreach (var business1 in business)
                {
                    list.Add(_entityBusinessConverter.ConvertToEntity(business1, new TEntity(), true));
                }
                return await _dataService.UpdateAll(list);
            }
            catch (Exception ex)
            {
                string errorMsg = String.Format("Exception Occured while trying to AddOrUpdate Generic Type from Database. Entity Type: '{0}', Business Type: '{1}'", typeof(TEntity), typeof(TBusiness));
                LogHelper.Instance.Log(LogLevel.Error, this, errorMsg, ex);
            }
            return false;
        }



        public async Task<bool> DeleteAll(List<TBusiness> business)
        {
            try
            {
                List<int> list = new List<int>();
                foreach (var business1 in business)
                {
                    list.Add(_entityBusinessConverter.GetPrimaryKeyFromBusiness(business1));
                }
                return await _dataService.DeleteAllById<TEntity>(list);
            }
            catch (Exception ex)
            {
                string errorMsg = String.Format("Exception Occured while trying to AddOrUpdate Generic Type from Database. Entity Type: '{0}', Business Type: '{1}'", typeof(TEntity), typeof(TBusiness));
                LogHelper.Instance.Log(LogLevel.Error, this, errorMsg, ex);
            }
            return false;
        }
    }
}
