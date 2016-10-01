using System;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Settings;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using GalaSoft.MvvmLight.Views;
using Nito.AsyncEx;

namespace Famoser.OfflineMedia.View.Services
{
    public class PermissionService : IPermissionsService
    {
        private readonly IPlatformCodeService _platformCodeService;
        private readonly IDialogService _dialogService;
        public PermissionService(IPlatformCodeService platformCodeService, IDialogService dialogService)
        {
            _platformCodeService = platformCodeService;
            _dialogService = dialogService;
        }

        private const string AskedAtMobileConnectionKey = "AskedAtMobileConnection";

        private string GenerateSettingKey(ConnectionType connectionType, DownloadContentType downloadContentType)
        {
            var res = "";
            switch (connectionType)
            {
                case ConnectionType.Mobile:
                    res += "Mobile";
                    break;
                case ConnectionType.Wlan:
                    res += "Wlan";
                    break;
                case ConnectionType.None:
                    res += "None";
                    break;
            }
            switch (downloadContentType)
            {
                case DownloadContentType.Any:
                    res += "Any";
                    break;
                case DownloadContentType.Article:
                    res += "Article";
                    break;
                case DownloadContentType.Feed:
                    res += "Feed";
                    break;
                case DownloadContentType.Image:
                    res += "Image";
                    break;
            }
            return res;
        }

        private DateTime _lastTimeRequested;
        private bool _lastResult;
        private AsyncLock _asyncLock = new AsyncLock();
        private async Task<bool> CanDownloadAsync(DownloadContentType type)
        {
            if (_downloadBlocked)
                _lastResult = false;
            else if (_lastTimeRequested - DateTime.Now < TimeSpan.FromSeconds(10))
                return _lastResult;
            else
            {
                var conntype = _platformCodeService.GetConnectionType();
                if (conntype == ConnectionType.None)
                    _lastResult = false;
                else if (conntype == ConnectionType.Mobile)
                {
                    if (!(bool) _platformCodeService.GetLocalSetting(AskedAtMobileConnectionKey, false))
                    {
                        _platformCodeService.SetLocalSetting(AskedAtMobileConnectionKey, true);
                        await _dialogService.ShowMessage(
                            "Sie sind über das Mobilfunktnetz online, der Download wurde daher automatisch angehalten. Sie können diese Option in den Einstellungen wieder deaktivieren",
                            "Datenverbindung");
                    }

                    if (
                        !(bool)
                            _platformCodeService.GetLocalSetting(GenerateSettingKey(conntype, DownloadContentType.Any),
                                false))
                        _lastResult = false;
                    _lastResult = (bool) _platformCodeService.GetLocalSetting(GenerateSettingKey(conntype, type), false);
                }
                else if (conntype == ConnectionType.Wlan)
                {
                    if (
                        !(bool)
                            _platformCodeService.GetLocalSetting(GenerateSettingKey(conntype, DownloadContentType.Any),
                                true))
                        _lastResult = false;
                    _lastResult = (bool) _platformCodeService.GetLocalSetting(GenerateSettingKey(conntype, type), true);
                }
                else
                {
                    _lastResult = false;
                }
            }
            _lastTimeRequested = DateTime.Now;
            return _lastResult;
        }

        public void SetPermission(ConnectionType conntype, DownloadContentType type, bool val)
        {
            _platformCodeService.SetLocalSetting(GenerateSettingKey(conntype, type), val);
            PermissionsChanged?.Invoke(this, EventArgs.Empty);
            _lastTimeRequested = DateTime.MinValue;
        }

        public bool GetPermission(ConnectionType conntype, DownloadContentType type, bool fallback)
        {
            return (bool)_platformCodeService.GetLocalSetting(GenerateSettingKey(conntype, type), fallback);
        }

        public Task<bool> CanDownload()
        {
            return CanDownloadAsync(DownloadContentType.Any);
        }

        public Task<bool> CanDownloadFeeds()
        {
            return CanDownloadAsync(DownloadContentType.Feed);
        }

        public Task<bool> CanDownloadArticles()
        {
            return CanDownloadAsync(DownloadContentType.Feed);
        }

        public Task<bool> CanDownloadImages()
        {
            return CanDownloadAsync(DownloadContentType.Image);
        }

        private bool _downloadBlocked;
        public void BlockDownloadPermission(bool value)
        {
            _downloadBlocked = value;
        }

        public event EventHandler PermissionsChanged;
    }
}
