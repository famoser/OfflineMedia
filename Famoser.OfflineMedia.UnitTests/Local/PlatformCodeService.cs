using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.Web.Http;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.UniversalWindows.Helpers;
using Famoser.OfflineMedia.Business.Enums.Settings;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using GalaSoft.MvvmLight.Threading;

namespace Famoser.OfflineMedia.UnitTests.Local
{
    public class PlatformCodeService : IPlatformCodeService
    {
        public Task<byte[]> ResizeImageAsync(Stream imageStream, double height, double width)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var file = await StorageFile.GetFileFromPathAsync(Guid.NewGuid().ToString());
                    using (var stream = await file.OpenStreamForWriteAsync())
                    {
                        await imageStream.CopyToAsync(stream);
                    }

                    //save stream to file
                    using (var fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                    {
                        var decoder = await BitmapDecoder.CreateAsync(fileStream);
                        InMemoryRandomAccessStream resizedStream = new InMemoryRandomAccessStream();
                        if (decoder.OrientedPixelHeight > height || decoder.OrientedPixelWidth > width)
                        {
                            BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                            double widthRatio = width / decoder.OrientedPixelWidth;
                            double heightRatio = height / decoder.OrientedPixelHeight;

                            // Use whichever ratio had to be sized down the most to make sure the image fits within our constraints.
                            double scaleRatio = Math.Min(widthRatio, heightRatio);
                            uint aspectHeight = (uint)Math.Floor(decoder.OrientedPixelHeight * scaleRatio);
                            uint aspectWidth = (uint)Math.Floor(decoder.OrientedPixelWidth * scaleRatio);

                            encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
                            encoder.BitmapTransform.ScaledHeight = aspectHeight;
                            encoder.BitmapTransform.ScaledWidth = aspectWidth;

                            try
                            {
                                // write out to the stream
                                // might fail cause https://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.imaging.bitmapencoder.bitmaptransform.aspx
                                await encoder.FlushAsync();
                            }
                            catch (Exception ex)
                            {
                                //from http://stackoverflow.com/questions/38617761/bitmapencoder-flush-throws-argument-exception/38633258#38633258
                                if (ex.HResult.ToString() == "WINCODEC_ERR_INVALIDPARAMETER" || ex.HResult == -2147024809)
                                {
                                    resizedStream = new InMemoryRandomAccessStream();
                                    BitmapEncoder pixelencoder =
                                        await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                                    BitmapTransform transform = new BitmapTransform
                                    {
                                        InterpolationMode = BitmapInterpolationMode.Fant,
                                        ScaledHeight = aspectHeight,
                                        ScaledWidth = aspectWidth
                                    };
                                    var provider = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                                        BitmapAlphaMode.Ignore,
                                        transform,
                                        ExifOrientationMode.RespectExifOrientation,
                                        ColorManagementMode.DoNotColorManage);
                                    var pixels = provider.DetachPixelData();
                                    pixelencoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                        aspectWidth,
                                        aspectHeight, decoder.DpiX, decoder.DpiY, pixels);
                                    try
                                    {
                                        await pixelencoder.FlushAsync();
                                    }
                                    catch (Exception ex2)
                                    {
                                        LogHelper.Instance.LogException(ex2);
                                        return null;
                                    }
                                }
                                else
                                {
                                    LogHelper.Instance.LogException(ex);
                                    return null;
                                }
                            }

                            // Reset the stream location.
                            resizedStream.Seek(0);


                            // Writes the image byte array in an InMemoryRandomAccessStream
                            using (DataReader reader = new DataReader(resizedStream.GetInputStreamAt(0)))
                            {
                                var bytes = new byte[resizedStream.Size];

                                await reader.LoadAsync((uint)resizedStream.Size);
                                reader.ReadBytes(bytes);

                                return bytes;
                            }
                        }

                        using (var reader = new DataReader(fileStream.GetInputStreamAt(0)))
                        {
                            var bytes = new byte[fileStream.Size];
                            await reader.LoadAsync((uint)fileStream.Size);
                            reader.ReadBytes(bytes);
                            return bytes;
                        }
                    }

                   
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Warning, "Download.cs", "ResizeImageAsync failed", ex);
                }
                return null;
            });
        }

        public Task<byte[]> DownloadResizeImage(Uri url, double height, double width)
        {
            return Task.Run(async () =>
            {
                if (url != null)
                {
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            using (
                                HttpResponseMessage response =
                                    await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                            {
                                IBuffer streamToReadFrom = await response.Content.ReadAsBufferAsync();
                                return await ResizeImageAsync(streamToReadFrom.AsStream(), height, width);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.Log(LogLevel.Warning, "Download.cs",
                            "DownloadImageAsync failed: " + url.AbsoluteUri, ex);
                    }
                }
                return null;
            });
        }

        public async void CheckBeginInvokeOnUi(Action action, Func<Task> after = null)
        {
            if (action == null)
                return;
            if (DispatcherHelper.UIDispatcher.HasThreadAccess)
                action();
            else
            {
                await DispatcherHelper.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
            }
            if (after != null)
                await after().ConfigureAwait(false);
        }

        public async Task<bool> OpenInBrowser(Uri url)
        {
            return await Launcher.LaunchUriAsync(url);
        }

        public Task<bool> Share(Uri articleUri, string title, string subTitle)
        {
            throw new NotImplementedException();
        }

        public int DeviceWidth()
        {
            return (int)ResolutionHelper.WidthOfDevice;
        }

        public int DeviceHeight()
        {
            return (int)ResolutionHelper.HeightOfDevice;
        }

        public async Task<bool> DeleteDatabaseFile()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("database.sqlite3", CreationCollisionOption.OpenIfExists);
                await file.DeleteAsync(StorageDeleteOption.Default);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        public void ExitApplication()
        {
            Application.Current.Exit();
        }

        public ConnectionType GetConnectionType()
        {
            throw new NotImplementedException();
        }

        public object GetLocalSetting(string settingKey, object fallback)
        {
            throw new NotImplementedException();
        }

        public void SetLocalSetting(string settingKey, object value)
        {
            throw new NotImplementedException();
        }

        public void ClearLocalSettings()
        {
            throw new NotImplementedException();
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
