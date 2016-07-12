using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.Web.Http;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.UniversalWindows.Helpers;
using Famoser.OfflineMedia.Business.Services;
using GalaSoft.MvvmLight.Threading;

namespace Famoser.OfflineMedia.WinUniversal.Services
{
    public class PlatformCodeService : IPlatformCodeService
    {
        public async Task<byte[]> DownloadResizeImage(Uri url)
        {
            if (url != null)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                        {
                            IBuffer streamToReadFrom = await response.Content.ReadAsBufferAsync();

                            var decoder = await BitmapDecoder.CreateAsync(streamToReadFrom.AsStream().AsRandomAccessStream());
                            if (decoder.OrientedPixelHeight > ResolutionHelper.HeightOfDevice ||
                                decoder.OrientedPixelWidth > ResolutionHelper.WidthOfDevice)
                            {
                                var resizedStream = new InMemoryRandomAccessStream();
                                BitmapEncoder encoder =
                                    await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                                double widthRatio = ResolutionHelper.WidthOfDevice /
                                                    decoder.OrientedPixelWidth;
                                double heightRatio = ResolutionHelper.HeightOfDevice /
                                                     decoder.OrientedPixelHeight;

                                // Use whichever ratio had to be sized down the most to make sure the image fits within our constraints.
                                double scaleRatio = Math.Min(widthRatio, heightRatio);
                                uint aspectHeight = (uint)Math.Floor(decoder.OrientedPixelHeight * scaleRatio);
                                uint aspectWidth = (uint)Math.Floor(decoder.OrientedPixelWidth * scaleRatio);

                                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
                                encoder.BitmapTransform.ScaledHeight = aspectHeight;
                                encoder.BitmapTransform.ScaledWidth = aspectWidth;

                                // write out to the stream
                                await encoder.FlushAsync();

                                // Reset the stream location.
                                resizedStream.Seek(0);

                                // Writes the image byte array in an InMemoryRandomAccessStream
                                // that is needed to set the source of BitmapImage.
                                using (DataReader reader = new DataReader(resizedStream.GetInputStreamAt(0)))
                                {
                                    var bytes = new byte[resizedStream.Size];

                                    await reader.LoadAsync((uint)resizedStream.Size);
                                    reader.ReadBytes(bytes);


                                    return bytes;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Warning, "Download.cs", "DownloadImageAsync failed: " + url.AbsoluteUri, ex);
                }
            }
            return null;
        }

        public void CheckBeginInvokeOnUi(Action action)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(action);
        }

        public async Task<bool> OpenInBrowser(Uri url)
        {
            return await Launcher.LaunchUriAsync(url);
        }

        public void ExitApplication()
        {
            Application.Current.Exit();
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
