using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using OfflineMedia.Common.Enums;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Services.Interfaces;

namespace OfflineMedia.Services
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
                LogHelper.Instance.Log(LogLevel.Error, this, "GetTextOfFileByKey failed", ex);
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
                LogHelper.Instance.Log(LogLevel.Error, this, "SaveFileByKey failed", ex);
            }
            return false;
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
                LogHelper.Instance.Log(LogLevel.Error, this, "GetContentsOfAssetFile failed", ex);
            }
            return null;
        }

        public Task<string> GetSettingsJson()
        {
            return GetContentsOfAssetFile("Settings");
        }

        public Task<string> GetSourceJson()
        {
            return GetContentsOfAssetFile("Source_min");
        }

        public Task<string> GetWeatherFontJson()
        {
            return GetContentsOfAssetFile("WeatherFontMapping");
        }

        public async Task<ulong> GetFileSizes()
        {
            ulong totalsize = 0;
            foreach (var fil in await ApplicationData.Current.LocalFolder.GetFilesAsync())
            {
                var props = await fil.GetBasicPropertiesAsync();
                totalsize += props.Size;
            }
            return totalsize;
        }

        public async Task<bool> ClearFiles()
        {
            try
            {
                //Message Box.
                MessageDialog msg = new MessageDialog("Die Anwendung wird zurückgesetzt und geschlossen. Alle Einstellungen gehen verloren.", "Anwendung zurücksetzten");

                //Commands
                msg.Commands.Add(new UICommand("abbrechen", CommandHandlers));
                msg.Commands.Add(new UICommand("zurücksetzten", CommandHandlers));

                await msg.ShowAsync();
                //end.
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ClearFiles failed", ex);
                return false;
            }
        }

        public async void CommandHandlers(IUICommand commandLabel)
        {
            var actions = commandLabel.Label;
            switch (actions)
            {
                //Okay Button.
                case "abbrechen":
                    break;
                //Quit Button.
                case "zurücksetzten":
                    await ApplicationData.Current.LocalFolder.CreateFileAsync("DELETEALL");
                    Application.Current.Exit();
                    break;
                    //end.
            }
        }

        private async Task<bool> DeleteAll()
        {
            try
            {
                return (await ApplicationData.Current.LocalFolder.GetFilesAsync()).Any(x => x.Name.Equals("DELETEALL"));
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "DeleteAll failed", ex);
                return false;
            }
        }

        public async Task<string> GetFilePathByKey(FileKeys fileKey)
        {
            try
            {
                //var deleteall = true;
                var deleteall = await DeleteAll();
                if (deleteall)
                    foreach (var fil in await ApplicationData.Current.LocalFolder.GetFilesAsync())
                    {
                        await fil.DeleteAsync();
                    }

                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileKey.ToString(), CreationCollisionOption.OpenIfExists);
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
