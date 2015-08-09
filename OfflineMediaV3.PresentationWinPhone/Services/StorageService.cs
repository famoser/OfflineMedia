using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Framework;
using OfflineMediaV3.Business.Framework.Logs;

namespace OfflineMediaV3.Services
{
    public class StorageService : IStorageService
    {
        public async Task<string> GetTextOfFileByKey(FileKeys key)
        {
            try
            {
                StorageFile localFile = await ApplicationData.Current.LocalFolder.GetFileAsync(key.ToString());
                if (localFile != null)
                {
                    return await FileIO.ReadTextAsync(localFile);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public async Task<bool> SaveFileByKey(FileKeys key, string content)
        {
            try
            {
                StorageFile localFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(key.ToString(), CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(localFile, content);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<string> GetContentsOfAssetFile(string fileName)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Configuration/" + fileName + ".json"));
                return await FileIO.ReadTextAsync(file);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task<string> GetSettingsJson()
        {
            return GetContentsOfAssetFile("Settings");
        }

        public Task<string> GetSourceJson()
        {
            return GetContentsOfAssetFile("Source");
        }

        public async Task<string> GetFilePathByKey(FileKeys fileKey)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileKey + "2", CreationCollisionOption.OpenIfExists);
                return file.Path;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.FatalError, this, "GetFilePathByKey failed", ex);
            }
            return null;
        }

        private List<string> _messagesToLog = new List<string>();
        internal void LogThis(string message)
        {
            _messagesToLog.Add(message);
            StartLogging();
        }

        private bool _isLogging;
        internal async void StartLogging()
        {
            try
            {
                if (!_isLogging)
                {
                    _isLogging = true;
                    while (_messagesToLog.Count > 0)
                    {
                        StorageFile localFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("messages.log",
                                    CreationCollisionOption.ReplaceExisting);
                        await FileIO.AppendTextAsync(localFile, _messagesToLog[0]);
                        _messagesToLog.RemoveAt(0);
                    }
                    _isLogging = false;
                }
            }
            catch (Exception)
            {
                //sometimes throws an access exception when logging too fast
            }
        }
    }
}
