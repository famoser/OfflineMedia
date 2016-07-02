using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace OfflineMedia.Services
{
    public class StorageService
    {
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
    }
}
