using System;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Settings;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using GalaSoft.MvvmLight.Views;

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

        private async Task<bool> CanDownloadAsync(DownloadContentType type)
        {
            if (_downloadBlocked)
                return false;

            var conntype = _platformCodeService.GetConnectionType();
            if (conntype == ConnectionType.None)
                return false;

            if (conntype == ConnectionType.Mobile)
            {
                if (!(bool)_platformCodeService.GetLocalSetting(AskedAtMobileConnectionKey, false))
                {
                    _platformCodeService.SetLocalSetting(AskedAtMobileConnectionKey, true);
                    await _dialogService.ShowMessage("Sie sind über das Mobilfunktnetz online, der Download wurde daher automatisch angehalten. Sie können diese Option in den Einstellungen wieder deaktivieren", "Datenverbindung");
                }

                if (!(bool)_platformCodeService.GetLocalSetting(GenerateSettingKey(conntype, DownloadContentType.Any), false))
                    return false;
                return (bool)_platformCodeService.GetLocalSetting(GenerateSettingKey(conntype, type), false);
            }
            if (conntype == ConnectionType.Wlan)
            {
                if (!(bool)_platformCodeService.GetLocalSetting(GenerateSettingKey(conntype, DownloadContentType.Any), true))
                    return false;
                return (bool)_platformCodeService.GetLocalSetting(GenerateSettingKey(conntype, type), true);
            }
            return false;
        }

        public void SetPermission(ConnectionType conntype, DownloadContentType type, bool val)
        {
            _platformCodeService.SetLocalSetting(GenerateSettingKey(conntype, type), val);
            PermissionsChanged?.Invoke(this, EventArgs.Empty);
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
