using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Enums.Settings;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using GalaSoft.MvvmLight.Views;
using Nito.AsyncEx;

namespace Famoser.OfflineMedia.WinUniversal.Platform
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

            if (conntype == ConnectionType.Wlan)
            {
                if (!(bool)_platformCodeService.GetLocalSetting(AskedAtMobileConnectionKey, false))
                {
                    await _dialogService.ShowMessage("Sie sind über das Mobilfunktnetz online, der Download wurde automatisch angehalten. Sie können diese Option in den Einstellungen wieder deaktivieren", "Datenverbindung");
                }

                return (bool)_platformCodeService.GetLocalSetting(GenerateSettingKey(conntype, type), false);

            }
            if (conntype == ConnectionType.Wlan)
                return (bool)_platformCodeService.GetLocalSetting(GenerateSettingKey(conntype, type), true);

            return false;
        }

        public void SetPermission(ConnectionType conntype, DownloadContentType type, bool val)
        {
            _platformCodeService.GetLocalSetting(GenerateSettingKey(conntype, type), val);
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
    }
}
