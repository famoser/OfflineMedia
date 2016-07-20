using System;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Interfaces;

namespace Famoser.OfflineMedia.UnitTests.Services.Mocks
{
    class HistoryNavigationServiceMock : IHistoryNavigationService
    {
        public void GoBack()
        {
            
        }

        public void NavigateTo(string pageKey, INavigationBackNotifier navigationBackNotifier = null, object notifyObject = null)
        {
            
        }

        public void NavigateToAndForget(string pageKey)
        {
            
        }

        public void Configure(string key, Type pageType)
        {
            
        }

        public string RootPageKey { get; } = "ROOT";
        public string UnknownPageKey { get; } = "UNNKNOWN";
        public string CurrentPageKey { get; } = "CURRENT";
    }
}
