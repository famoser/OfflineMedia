﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Nito.AsyncEx;
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
        private readonly IStorageService _storageService;
        private readonly ISQLitePlatform _sqLitePlatform;
        private SQLiteAsyncConnection _asyncConnection;
        private SQLiteConnection _connection;

        private static SqliteDataService _instance;

        public static async Task<SqliteDataService> GetInstance()
        {
            if (_instance == null)
            {
                if (_tsk == null)
                {
                    _initInstance = new SqliteDataService(SimpleIoc.Default.GetInstance<IStorageService>(), SimpleIoc.Default.GetInstance<ISQLitePlatform>());
                    _tsk = _initInstance.Init();
                    await _tsk;
                    await _initInstance.PrepareDatabase();
                    _tsk = null;
                }
                else
                {
                    await _tsk;
                }

                _instance = _initInstance;
            }
            return _instance;
        }

        private static Task _tsk;
        private static SqliteDataService _initInstance;
        private static readonly AsyncLock DatabaseLock = new AsyncLock();

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
                using (await DatabaseLock.LockAsync())
                {
                    var res = await _asyncConnection.GetAsync<T>(id);
                    return res;
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message != "Sequence contains no elements")
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

        public async Task<List<T>> GetAllById<T>(IEnumerable<int> ids) where T : EntityIdBase, new()
        {
            try
            {
                using (await DatabaseLock.LockAsync())
                {
                    var res = await _asyncConnection.Table<T>().Where(a => ids.Any(d => d == a.Id)).ToListAsync();
                    return res;
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message != "Sequence contains no elements")
                {
                    LogHelper.Instance.Log(LogLevel.Error, this, "GetAllById failed for " + typeof(T).Name, ex);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "GetAllById failed for " + typeof(T).Name, ex);
            }

            return null;
        }

        public async Task<List<T>> GetAll<T>() where T : EntityIdBase, new()
        {
            try
            {
                using (await DatabaseLock.LockAsync())
                {
                    var res = await _asyncConnection.Table<T>().ToListAsync();
                    return res;
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message != "Sequence contains no elements")
                {
                    LogHelper.Instance.Log(LogLevel.Error, this, "GetAll failed for " + typeof(T).Name, ex);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "GetAll failed for " + typeof(T).Name, ex);
            }
            
            return null;
        }

        public async Task<bool> DeleteAllById<T>(IEnumerable<int> ids) where T : EntityIdBase, new()
        {
            try
            {
                var args = string.Join(",", ids);
                using (await DatabaseLock.LockAsync())
                {
                    await _asyncConnection.ExecuteAsync("DELETE FROM " + typeof(T).Name + " WHERE id IN (" + args + ");");
                }
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
                using (await DatabaseLock.LockAsync())
                {
                    await _asyncConnection.InsertAsync(obj);
                }

                return obj.Id;
            }
            catch (Exception ex)
            {
                //try again
                int res = await Add(obj);
                if (res == -1)
                    LogHelper.Instance.Log(LogLevel.Error, this, "Update failed for " + obj.GetType().Name, ex);
                else
                {
                    return res;
                }
            }

            return -1;
        }

        public async Task<List<int>> AddAll<T>(IEnumerable<T> obj) where T : EntityIdBase, new()
        {
            try
            {
                using (await DatabaseLock.LockAsync())
                {
                    await _asyncConnection.RunInTransactionAsync(conn =>
                    {
                        conn.InsertAll(obj);
                    });
                }

                return obj.Select(d => d.Id).ToList();
            }
            catch (Exception ex)
            {
                //try again
                var res = await AddAll(obj);
                if (!res.Any())
                    LogHelper.Instance.Log(LogLevel.Error, this, "Update failed for " + obj.GetType().Name, ex);
                else
                {
                    return res;
                }
            }

            return new List<int>();
        }

        public async Task<bool> Update<T>(T obj) where T : EntityIdBase, new()
        {
            try
            {
                using (await DatabaseLock.LockAsync())
                {
                    await _asyncConnection.UpdateAsync(obj);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                //try again
                if (!await Update(obj))
                    LogHelper.Instance.Log(LogLevel.Error, this, "Update failed for " + obj.GetType().Name, ex);
                else
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateAll<T>(IEnumerable<T> obj) where T : EntityIdBase, new()
        {
            try
            {
                using (await DatabaseLock.LockAsync())
                {
                    await _asyncConnection.RunInTransactionAsync(conn =>
                    {
                        conn.UpdateAll(obj);
                    });
                }
                
                return true;
            }
            catch (Exception ex)
            {
                //try again
                if (!await UpdateAll(obj))
                    LogHelper.Instance.Log(LogLevel.Error, this, "Update failed for " + obj.GetType().Name, ex);
                else
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<int> GetHighestId<T>() where T : EntityIdBase, new()
        {
            try
            {
                using (await DatabaseLock.LockAsync())
                {
                    var s = await _asyncConnection.Table<T>().OrderByDescending(c => c.Id).FirstAsync();
                    return s.Id;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Sequence contains no elements")
                {
                    return 0;
                }
                //try again
                int res = await GetHighestId<T>();
                if (res == -1)
                    LogHelper.Instance.Log(LogLevel.Error, this, "Update failed for " + typeof(T).Name, ex);
                else
                {
                    return res;
                }
            }

            return -1;
        }

        public async Task<bool> DeleteById<T>(int id) where T : class, new()
        {
            try
            {
                using (await DatabaseLock.LockAsync())
                {
                    await _asyncConnection.DeleteAsync<T>(id);
                }
                return true;
            }
            catch (Exception ex)
            {
                

                if (!await DeleteById<T>(id))
                    LogHelper.Instance.Log(LogLevel.Error, this,
                        "DeleteById failed for " + typeof(T).Name + " with id " + id, ex);
                else
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<List<T>> GetByCondition<T>(Expression<Func<T, bool>> func, Expression<Func<T, object>> orderByProperty, bool descending, int limit, int skip) where T : class, new()
        {
            try
            {
                List<T> res;
                using (await DatabaseLock.LockAsync())
                {
                    if (orderByProperty != null)
                    {
                        if (descending)
                        {
                            if (limit > 0)
                                res = await
                                        _asyncConnection.Table<T>()
                                            .Where(func)
                                            .OrderByDescending(orderByProperty)
                                            .Skip(skip)
                                            .Take(limit)
                                            .ToListAsync();
                            else
                                res =
                                    await
                                        _asyncConnection.Table<T>()
                                            .Where(func)
                                            .OrderByDescending(orderByProperty)
                                            .Skip(skip)
                                            .ToListAsync();
                        }
                        else
                        {

                            if (limit > 0)
                                res =
                                    await
                                        _asyncConnection.Table<T>()
                                            .Where(func)
                                            .OrderBy(orderByProperty)
                                            .Skip(skip)
                                            .Take(limit)
                                            .ToListAsync();
                            else
                                res =
                                    await
                                        _asyncConnection.Table<T>()
                                            .Where(func)
                                            .OrderBy(orderByProperty)
                                            .Skip(skip)
                                            .ToListAsync();
                        }
                    }
                    else
                    {
                        if (limit > 0)
                            res = await _asyncConnection.Table<T>().Where(func).Take(limit).Skip(skip).ToListAsync();
                        else
                            res = await _asyncConnection.Table<T>().Where(func).Skip(skip).ToListAsync();
                    }
                }

                
                return res;
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
                using (await DatabaseLock.LockAsync())
                {
                    var res = await _asyncConnection.Table<T>().Where(func).CountAsync();
                    return res;
                }
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
                using (await DatabaseLock.LockAsync())
                {
                    var res = (await _asyncConnection.Table<ArticleEntity>().Where(
                        a => a.WordDump.Contains(keyword)).ToListAsync()).Select(d => d.Id).ToList();
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "GetByKeyword failed for " + keyword, ex);
            }

            
            return new List<int>();
        }

        public async Task<bool> DeleteArticlesById(IEnumerable<int> articleIds)
        {
            await DeleteAllById<ArticleEntity>(articleIds);
            try
            {
                using (await DatabaseLock.LockAsync())
                {
                    await _asyncConnection.ExecuteAsync("DELETE FROM ContentEntity WHERE ArticleId NOT IN (SELECT Id FROM ArticleEntity)");
                    await _asyncConnection.ExecuteAsync("DELETE FROM GalleryEntity WHERE Id NOT IN (SELECT GalleryId FROM ContentEntity)");
                    await _asyncConnection.ExecuteAsync("DELETE FROM ImageEntity WHERE GalleryId NOT IN (SELECT Id FROM GalleryEntity) AND GalleryId > 0");
                    await _asyncConnection.ExecuteAsync("DELETE FROM ImageEntity WHERE Id NOT IN (SELECT LeadImageId FROM ArticleEntity) AND GalleryId = 0");
                    await _asyncConnection.ExecuteAsync("DELETE FROM RelatedArticleRelations WHERE Article1Id NOT IN (SELECT Id FROM ArticleEntity) OR Article2Id NOT IN (SELECT Id FROM ArticleEntity)");
                    await _asyncConnection.ExecuteAsync("DELETE FROM ThemeArticleRelations WHERE ArticleId NOT IN (SELECT Id FROM ArticleEntity)");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "DeleteArticlesById failed", ex);
            }

            
            return false;
        }

        public async Task<bool> SetArticleState(int articleId, int newState)
        {
            try
            {
                using (await DatabaseLock.LockAsync())
                {
                    await _asyncConnection.ExecuteAsync("UPDATE ArticleEntity SET State = " + newState + " WHERE Id = " + articleId);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "DeleteArticlesById failed", ex);
            }

            
            return false;
        }

        public async Task<bool> SetArticleFavorite(int articleId, bool isFavorite)
        {
            try
            {
                using (await DatabaseLock.LockAsync())
                {
                    var str = isFavorite ? "1" : "0";
                    await _asyncConnection.ExecuteAsync("UPDATE ArticleEntity SET IsFavorite = " + str + " WHERE Id = " + articleId);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "DeleteArticlesById failed", ex);
            }

            
            return false;
        }

        private async Task<bool> PrepareDatabase()
        {
            try
            {
                using (await DatabaseLock.LockAsync())
                {
                    await _asyncConnection.RunInTransactionAsync(conn =>
                    {
                        conn.Execute("UPDATE ArticleEntity SET State=0 WHERE State=1");
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ResetLoadingEnum", ex);
            }

            return false;
        }

        /*
        private Queue<Guid> _process = new Queue<Guid>();
        private string _activeLock;
        private async Task LockDatabase(string activeLock)
        {
            var guid = Guid.NewGuid();
            _process.Enqueue(guid);
            while (await IsLocked(guid)) { }
            Lock(activeLock);
        }

#pragma warning disable 1998
        private async Task UnlockDatabase()
#pragma warning restore 1998
        {
            UnLock();
        }

        private bool _isLocked = false;
        private async Task<bool> IsLocked(Guid guid)
        {
            if (!_isLocked)
            {
                if (_process.Peek() == guid)
                {
                    _process.Dequeue();
                    return false;
                }
            }
            await Task.Delay(10);
            return true;
        }

        private bool Lock(string activeLock)
        {
            _activeLock = activeLock;
            _isLocked = true;
            return true;
        }

        private bool UnLock()
        {
            _isLocked = false;
            return true;
        }
        */
    }
}
