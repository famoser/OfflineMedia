using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using OfflineMediaV3.Common.Enums;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Framework.Services.Interfaces;
using OfflineMediaV3.Common.Framework.Singleton;
using OfflineMediaV3.Data;
using OfflineMediaV3.Data.Entities;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;

namespace OfflineMediaV3.Business.Framework
{
    public sealed class SqliteDataService : IDataService
    {
        private IStorageService _storageService;
        private ISQLitePlatform _sqLitePlatform;
        private SQLiteAsyncConnection _connection;

        private static SqliteDataService _instance;

        public static async Task<SqliteDataService> GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SqliteDataService(SimpleIoc.Default.GetInstance<IStorageService>(), SimpleIoc.Default.GetInstance<ISQLitePlatform>());
                await _instance.Init();
            }
            return _instance;
        }

        private SqliteDataService(IStorageService storageService, ISQLitePlatform sqlitePlatform)
        {
            _storageService = storageService;
            _sqLitePlatform = sqlitePlatform;
        }

        public async Task<bool> Init()
        {
            try
            {
                if (_connection == null)
                {
                    string databaseFile = await _storageService.GetFilePathByKey(FileKeys.Database);
                    _connection = new SQLiteAsyncConnection(() => new SQLiteConnectionWithLock(_sqLitePlatform, new SQLiteConnectionString(databaseFile, false)));
                    await _connection.CreateTableAsync<ArticleEntity>();
                    await _connection.CreateTableAsync<ContentEntity>();
                    await _connection.CreateTableAsync<GalleryEntity>();
                    await _connection.CreateTableAsync<ImageEntity>();
                    await _connection.CreateTableAsync<RelatedArticleRelations>();
                    await _connection.CreateTableAsync<RelatedThemeRelations>();
                    await _connection.CreateTableAsync<ThemeArticleRelations>();
                    await _connection.CreateTableAsync<ThemeEntity>();

                    await _connection.CreateTableAsync<SettingEntity>();

                    await _connection.ExecuteAsync("PRAGMA synchronous = OFF");

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.FatalError, this, "Datenbank konnte nicht initialisiert werden.", ex);
            }
            return false;
        }

        public async Task BeginTransaction(bool onlyread)
        {
            /*
            if (!onlyread)
                await _connection.ExecuteAsync("begin transaction");*/
        }

        public async Task CommitTransaction()
        {
            /*
            try
            {
                 await  _connection.ExecuteAsync("commit transaction");
            }
            catch (AggregateException ex)
            {

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "RollbackTransaction falied", ex);
            }*/
        }

        public void RollbackTransaction(bool onlyread)
        {
            /*
            try
            {
                if (!onlyread)
                    _connection.ExecuteAsync("rollback").Wait();
            }
            catch (AggregateException ex)
            {

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "RollbackTransaction falied", ex);
            }
             * */
        }

        public async Task<T> GetById<T>(int id) where T : class, new()
        {
            try
            {
                return await _connection.GetAsync<T>(id);
            }
            catch (InvalidOperationException ex)
            {
                //don't do shit 
                if (ex.Message == "Sequence contains no elements")
                {

                }
                else
                {
                    LogHelper.Instance.Log(LogLevel.Error, this, "GetById failed for " + typeof(T).Name + " with id " + id, ex);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "GetById failed for " + typeof(T).Name + " with id " + id, ex);
            }
            return null;
        }

        public async Task<bool> AddOrUpdate<T>(T obj) where T : class, new()
        {
            try
            {
                await _connection.InsertOrReplaceAsync(obj);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Update failed for " + obj.GetType().Name, ex);
            }
            return false;
        }

        public async Task<int> GetHighestId<T>() where T : EntityBase, new()
        {
            try
            {
                var  s = await _connection.Table<T>().OrderByDescending(c => c.Id).FirstAsync();
                return s.Id;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Update failed for " + typeof(T).Name, ex);
            }
            return -1;
        }

        public async Task<bool> DeleteById<T>(int id) where T : class, new()
        {
            try
            {
                await _connection.DeleteAsync<T>(id);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "DeleteById failed for " + typeof(T).Name + " with id " + id, ex);
            }
            return false;
        }

        public async Task<List<T>> GetByCondition<T>(Expression<Func<T, bool>> func, Expression<Func<T, object>> orderByProperty = null, bool descending = false, int limit = 0) where T : class, new()
        {
            try
            {
                if (orderByProperty != null)
                {
                    if (descending)
                    {
                        if (limit > 0)
                            return await _connection.Table<T>().Where(func).OrderByDescending(orderByProperty).Take(limit).ToListAsync();
                        return await _connection.Table<T>().Where(func).OrderByDescending(orderByProperty).ToListAsync();
                    }
                    if (limit > 0)
                        return await _connection.Table<T>().Where(func).OrderBy(orderByProperty).Take(limit).ToListAsync();
                    return await _connection.Table<T>().Where(func).OrderBy(orderByProperty).ToListAsync();
                }
                if (limit > 0)
                    return await _connection.Table<T>().Where(func).Take(limit).ToListAsync();
                return await _connection.Table<T>().Where(func).ToListAsync();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "GetByCondition failed", ex);
            }
            return new List<T>();
        }

        public async Task<int> CountByCondition<T>(Expression<Func<T, bool>> func) where T : class, new()
        {
            try
            {
                return await _connection.Table<T>().Where(func).CountAsync();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "CountByCondition failed for " + typeof(T).Name, ex);
            }
            return 0;
        }
    }
}
