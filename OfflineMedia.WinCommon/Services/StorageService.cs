using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Famoser.FrameworkEssentials.Logging;
using Famoser.OfflineMedia.Business.Enums;

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
                LogHelper.Instance.Log(LogLevel.Error, "GetTextOfFileByKey failed",this, ex);
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
                LogHelper.Instance.Log(LogLevel.Error, "SaveFileByKey failed", this, ex);
            }
            return false;
        }

        private async Task<string> GetContentsOfAssetFile(string fileName)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/SettingsUserConfiguration/" + fileName + ".json"));
                return await FileIO.ReadTextAsync(file);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "GetContentsOfAssetFile failed", this, ex);
            }
            return null;
        }

        private static IBuffer _fileTemp;
        public static async Task<IBuffer> GetImageFile(string fileName)
        {
            try
            {
                if (_fileTemp == null)
                {
                    StorageFile file =
                        (await
                            StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Images/" + fileName))
                                .AsTask()
                                .ConfigureAwait(false));
                    _fileTemp = await FileIO.ReadBufferAsync(file).AsTask().ConfigureAwait(false);
                }
                return _fileTemp;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "GetContentsOfAssetFile failed", "SS", ex);
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
                LogHelper.Instance.Log(LogLevel.Error, "ClearFiles failed", this, ex);
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
                LogHelper.Instance.Log(LogLevel.Error, "DeleteAll failed",this, ex);
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
                LogHelper.Instance.Log(LogLevel.FatalError, "GetFilePathByKey failed",this, ex);
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
