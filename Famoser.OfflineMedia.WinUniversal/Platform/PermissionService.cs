using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Popups;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using GalaSoft.MvvmLight.Views;
using Nito.AsyncEx;

namespace Famoser.OfflineMedia.WinUniversal.Platform
{
    public class PermissionService : IPermissionsService
    {
        private static ApplicationDataContainer _settings;
        private static readonly AsyncLock AsyncLock = new AsyncLock();
        private static bool _isInitialized;

        private const int NoCommandId = 1;
        private const int YesCommandId = 2;

        private static async Task InitializeSettings(bool force = false)
        {
            ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile == null)
            {
                _canDownloadImages = false;
                _canDownloadArticles = false;
                _canDownloadFeeds = false;
                _canDownload = false;
                return;
            }
            if (!profile.IsWwanConnectionProfile)
            {
                _canDownloadImages = true;
                _canDownloadArticles = true;
                _canDownloadFeeds = true;
                _canDownload = true;

                if (force)
                {
                    if (_settings.Values.ContainsKey("PermissionService.Asked") &&
                        (string)_settings.Values["PermissionService.Asked"] == "1.0")
                        return;

                    //todo: rename, this is not clear at all
                    _settings.Values["PermissionService.CanDownloadImages"] = false;
                    _settings.Values["PermissionService.CanDownloadArticles"] = false;
                    _settings.Values["PermissionService.CanDownloadFeeds"] = false;
                    _settings.Values["PermissionService.CanDownload"] = false;
                }
            }
            else
            {
                _settings = ApplicationData.Current.LocalSettings;

                if (!_isInitialized)
                {
                    using (await AsyncLock.LockAsync())
                    {
                        _isInitialized = true;

                        if (_settings.Values.ContainsKey("PermissionService.Asked") &&
                            (string)_settings.Values["PermissionService.Asked"] == "1.0")
                            return;
                        _settings.Values["PermissionService.Asked"] = "1.0";

                        var dialog =
                            new MessageDialog(
                                "Sie sind nicht über WLAN verbunden. Möchten Sie die Nachrichten trotzdem aktualisieren? Sie können dies und mehr Datensparoptionen in den Einstellung ändern",
                                "keine WLAN-Verbindung")
                            {
                                DefaultCommandIndex = 0,
                                CancelCommandIndex = 0
                            };
                        dialog.Commands.Add(new UICommand()
                        {
                            Id = NoCommandId,
                            Invoked = Invoked,
                            Label = "nein"
                        });
                        dialog.Commands.Add(new UICommand()
                        {
                            Id = YesCommandId,
                            Invoked = Invoked,
                            Label = "ja"
                        });

                        await dialog.ShowAsync();
                    }
                }
                _canDownloadImages = (bool)_settings.Values["PermissionService.CanDownloadImages"];
                _canDownloadArticles = (bool)_settings.Values["PermissionService.CanDownloadArticles"];
                _canDownloadFeeds = (bool)_settings.Values["PermissionService.CanDownloadFeeds"];
                _canDownload = (bool)_settings.Values["PermissionService.CanDownload"];
            }
        }

        public static async Task SetArticleDownloadOnMobileConnection(bool state)
        {
            await InitializeSettings(true);

            ApplicationData.Current.LocalSettings.Values["PermissionService.CanDownloadArticles"] = state;
            ApplicationData.Current.LocalSettings.Values["PermissionService.CanDownloadFeeds"] = state;
        }

        public static async Task<bool> GetArticleDownloadOnMobileConnection()
        {
            await InitializeSettings(true);

            return (bool) ApplicationData.Current.LocalSettings.Values["PermissionService.CanDownloadArticles"];
        }

        public static async Task SetImageDownloadOnMobileConnection(bool state)
        {
            await InitializeSettings(true);

            ApplicationData.Current.LocalSettings.Values["PermissionService.CanDownloadImages"] = state;
        }

        public static async Task<bool> GetImageDownloadOnMobileConnection()
        {
            await InitializeSettings(true);

            return (bool)ApplicationData.Current.LocalSettings.Values["PermissionService.CanDownloadImages"];
        }

        private static void Invoked(IUICommand command)
        {
            switch ((int)command.Id)
            {
                case YesCommandId:
                    {
                        _settings.Values["PermissionService.CanDownloadImages"] = true;
                        _settings.Values["PermissionService.CanDownloadArticles"] = true;
                        _settings.Values["PermissionService.CanDownloadFeeds"] = true;
                        _settings.Values["PermissionService.CanDownload"] = true;
                        break;
                    }
                case NoCommandId:
                    {
                        _settings.Values["PermissionService.CanDownloadImages"] = false;
                        _settings.Values["PermissionService.CanDownloadArticles"] = false;
                        _settings.Values["PermissionService.CanDownloadFeeds"] = false;
                        _settings.Values["PermissionService.CanDownload"] = false;
                        break;
                    }
            }
        }

        private static bool _blockDownload;
        private static bool _canDownload;
        public async Task<bool> CanDownload()
        {
            await InitializeSettings();

            return _canDownload && !_blockDownload;
        }

        private static bool _canDownloadFeeds;
        public async Task<bool> CanDownloadFeeds()
        {
            await InitializeSettings();

            return _canDownloadFeeds && !_blockDownload;
        }

        private static bool _canDownloadArticles;
        public async Task<bool> CanDownloadArticles()
        {
            await InitializeSettings();

            return _canDownloadArticles && !_blockDownload;
        }

        private static bool _canDownloadImages;
        public async Task<bool> CanDownloadImages()
        {
            await InitializeSettings();

            return _canDownloadImages && !_blockDownload;
        }

        public void OverrideDownloadPermission(bool value)
        {
            _blockDownload = value;
        }
    }
}
