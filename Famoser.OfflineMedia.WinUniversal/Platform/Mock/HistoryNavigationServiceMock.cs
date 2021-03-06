﻿using System;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Interfaces;

namespace Famoser.OfflineMedia.WinUniversal.Platform.Mock
{
    public class HistoryNavigationServiceMock : IHistoryNavigationService
    {
        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(string pageKey, INavigationBackNotifier navigationBackNotifier = null, object notifyObject = null)
        {
            throw new NotImplementedException();
        }

        public void NavigateToAndForget(string pageKey)
        {
            throw new NotImplementedException();
        }

        public void Configure(string key, Type pageType)
        {
            throw new NotImplementedException();
        }

        public string RootPageKey { get; }
        public string UnknownPageKey { get; }
        public string CurrentPageKey { get; }
    }
}
