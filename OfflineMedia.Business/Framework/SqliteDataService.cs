using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using OfflineMedia.Common.Enums;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Services.Interfaces;
using OfflineMedia.Data;
using OfflineMedia.Data.Entities;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;

namespace OfflineMedia.Business.Framework
{
    public sealed class SqliteDataService : IDataService
    {
        private IStorageService _storageService;
        private ISQLitePlatform _sqLitePlatform;
        private SQLiteAsyncConnection _asyncConnection;
        private SQLiteConnection _connection;

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
                if (_asyncConnection == null)
                {
                    string databaseFile = await _storageService.GetFilePathByKey(FileKeys.Database);
                    _asyncConnection = new SQLiteAsyncConnection(() => new SQLiteConnectionWithLock(_sqLitePlatform, new SQLiteConnectionString(databaseFile, false)));
                    await _asyncConnection.CreateTableAsync<ArticleEntity>();
                    await _asyncConnection.CreateTableAsync<ContentEntity>();
                    await _asyncConnection.CreateTableAsync<GalleryEntity>();
                    await _asyncConnection.CreateTableAsync<ImageEntity>();
                    await _asyncConnection.CreateTableAsync<RelatedArticleRelations>();
                    await _asyncConnection.CreateTableAsync<RelatedThemeRelations>();
                    await _asyncConnection.CreateTableAsync<ThemeArticleRelations>();
                    await _asyncConnection.CreateTableAsync<ThemeEntity>();

                    await _asyncConnection.CreateTableAsync<SettingEntity>();

                    await _asyncConnection.ExecuteAsync("PRAGMA synchronous = OFF");

                    return true;
                }
                if (_connection == null)
                {
                    string databaseFile = await _storageService.GetFilePathByKey(FileKeys.Database);
                    _connection = new SQLiteConnection(_sqLitePlatform, databaseFile);
                    _connection.CreateTable<ArticleEntity>();
                    _connection.CreateTable<ContentEntity>();
                    _connection.CreateTable<GalleryEntity>();
                    _connection.CreateTable<ImageEntity>();
                    _connection.CreateTable<RelatedArticleRelations>();
                    _connection.CreateTable<RelatedThemeRelations>();
                    _connection.CreateTable<ThemeArticleRelations>();
                    _connection.CreateTable<ThemeEntity>();

                    _connection.CreateTable<SettingEntity>();

                    //_connection.ExecuteAsync("PRAGMA synchronous = OFF");

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.FatalError, this, "Datenbank konnte nicht initialisiert werden.", ex);
            }
            return false;
        }

#pragma warning disable 1998
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
#pragma warning restore 1998

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
                return await _asyncConnection.GetAsync<T>(id);
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

        public async Task<bool> DeleteAllById<T>(List<int> ids) where T : EntityIdBase, new()
        {
            try
            {
            
                await _asyncConnection.RunInTransactionAsync(conn =>
                {
                    foreach (var id in ids)
                    {
                        conn.Delete<T>(id);
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "DeleteAllById failed for " + typeof(T).Name, ex);
            }
            return false;
        }

        public async Task<int> Add<T>(T obj) where T : EntityIdBase, new()
        {
            try
            {
                await _asyncConnection.InsertAsync(obj);
                return obj.Id;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Update failed for " + obj.GetType().Name, ex);
            }
            return -1;
        }

        public async Task<List<int>> AddAll<T>(List<T> obj) where T : EntityIdBase, new()
        {
            try
            {
                await _asyncConnection.RunInTransactionAsync(conn =>
                {
                    conn.InsertAll(obj);
                });
                return obj.Select(d => d.Id).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Update failed for " + obj.GetType().Name, ex);
            }
            return new List<int>();
        }

        public async Task<bool> Update<T>(T obj) where T : EntityIdBase, new()
        {
            try
            {
                await _asyncConnection.UpdateAsync(obj);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Update failed for " + obj.GetType().Name, ex);
            }
            return false;
        }

        public async Task<bool> UpdateAll<T>(List<T> obj) where T : EntityIdBase, new()
        {
            try
            {
                await _asyncConnection.RunInTransactionAsync(conn =>
                {
                    conn.UpdateAll(obj);
                });
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Update failed for " + obj.GetType().Name, ex);
            }
            return false;
        }

        public async Task<int> GetHighestId<T>() where T : EntityIdBase, new()
        {
            try
            {
                var s = await _asyncConnection.Table<T>().OrderByDescending(c => c.Id).FirstAsync();
                return s.Id;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Sequence contains no elements")
                    return 0;

                LogHelper.Instance.Log(LogLevel.Error, this, "Update failed for " + typeof(T).Name, ex);
            }
            return -1;
        }

        public async Task<bool> DeleteById<T>(int id) where T : class, new()
        {
            try
            {
                await _asyncConnection.DeleteAsync<T>(id);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "DeleteById failed for " + typeof(T).Name + " with id " + id, ex);
            }
            return false;
        }

        public async Task<List<T>> GetByCondition<T>(Expression<Func<T, bool>> func, Expression<Func<T, object>> orderByProperty, bool descending, int limit, int skip) where T : class, new()
        {
            try
            {

                if (orderByProperty != null)
                {
                    if (descending)
                    {
                        if (limit > 0)
                            return await _asyncConnection.Table<T>().Where(func).OrderByDescending(orderByProperty).Skip(skip).Take(limit).ToListAsync();
                        return await _asyncConnection.Table<T>().Where(func).OrderByDescending(orderByProperty).Skip(skip).ToListAsync();
                    }
                    if (limit > 0)
                        return await _asyncConnection.Table<T>().Where(func).OrderBy(orderByProperty).Skip(skip).Take(limit).ToListAsync();
                    return await _asyncConnection.Table<T>().Where(func).OrderBy(orderByProperty).Skip(skip).ToListAsync();
                }
                if (limit > 0)
                    return await _asyncConnection.Table<T>().Where(func).Take(limit).Skip(skip).ToListAsync();
                return await _asyncConnection.Table<T>().Where(func).Skip(skip).ToListAsync();
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
                return await _asyncConnection.Table<T>().Where(func).CountAsync();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "CountByCondition failed for " + typeof(T).Name, ex);
            }
            return 0;
        }

        public async Task<List<int>> GetByKeyword(string keyword)
        {
            try
            {
                return (await _asyncConnection.Table<ArticleEntity>().Where(
                    a => a.WordDump.Contains(keyword)).ToListAsync()).Select(d => d.Id).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "GetByKeyword failed for " + keyword, ex);
            }
            return new List<int>();
        }

        public async Task<bool> DeleteArticlesById(IEnumerable<int> articleIds)
        {
            try
            {
                await _asyncConnection.RunInTransactionAsync(conn =>
                {
                    foreach (var articleId in articleIds)
                    {
                        conn.Delete<ArticleEntity>(articleId);
                    }

                    //clear tables from invalid values
                    conn.Execute("DELETE FROM ContentEntity WHERE ArticleId NOT IN (SELECT Id FROM ArticleEntity)");
                    conn.Execute("DELETE FROM GalleryEntity WHERE Id NOT IN (SELECT GalleryId FROM ContentEntity)");
                    conn.Execute("DELETE FROM ImageEntity WHERE GalleryId NOT IN (SELECT Id FROM GalleryEntity) AND GalleryId > 0");
                    conn.Execute("DELETE FROM ImageEntity WHERE Id NOT IN (SELECT LeadImageId FROM ArticleEntity) AND GalleryId = 0");
                    conn.Execute("DELETE FROM RelatedArticleRelations WHERE Article1Id NOT IN (SELECT Id FROM ArticleEntity) OR Article2Id NOT IN (SELECT Id FROM ArticleEntity)");
                    conn.Execute("DELETE FROM ThemeArticleRelations WHERE ArticleId NOT IN (SELECT Id FROM ArticleEntity)");
                });
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "DeleteArticlesById failed", ex);
            }
            return false;
        }
    }
}
