﻿using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Networking.Connectivity;
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

#pragma warning disable 1998
namespace Famoser.OfflineMedia.WinUniversal.Platform
{
    public class PlatformCodeService : IPlatformCodeService
    {
        public Task<byte[]> ResizeImageAsync(Stream imageStream, double height, double width)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var decoder = await BitmapDecoder.CreateAsync(imageStream.AsRandomAccessStream());
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

                    imageStream.Seek(0, SeekOrigin.Begin);
                    using (var memoryStream = new MemoryStream())
                    {
                        imageStream.CopyTo(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Warning, "Download.cs", "ResizeImageAsync failed", ex);
                }
                return null;
            });
        }

        private readonly HttpClient _httpClient = new HttpClient();
        public Task<byte[]> DownloadResizeImage(Uri url, double height, double width)
        {
            return Task.Run(async () =>
            {
                if (url != null)
                {
                    try
                    {
                        using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                        {
                            IBuffer streamToReadFrom = await response.Content.ReadAsBufferAsync();
                            return await ResizeImageAsync(streamToReadFrom.AsStream(), height, width);
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

        public async Task<bool> Share(Uri articleUri, string title, string subTitle)
        {
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += (dtm, drea) => DoShare(dtm, drea, articleUri, title, subTitle);

            DataTransferManager.ShowShareUI();
            return true;
        }

        private void DoShare(DataTransferManager sender, DataRequestedEventArgs args, Uri articleUri, string title, string description)
        {
            DataRequest request = args.Request;

            request.Data.Properties.Title = "Share";
            request.Data.Properties.Description = "Teile den Artikel";
            request.Data.SetText(title + "\n" + description);
            request.Data.SetWebLink(articleUri);
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
            ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();

            if (profile == null)
                return ConnectionType.None;

            return profile.IsWwanConnectionProfile ? ConnectionType.Mobile : ConnectionType.Wlan;
        }

        public object GetLocalSetting(string settingKey, object fallback)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(settingKey))
                ApplicationData.Current.LocalSettings.Values[settingKey] = fallback;

            return ApplicationData.Current.LocalSettings.Values[settingKey];
        }

        public void SetLocalSetting(string settingKey, object value)
        {
            ApplicationData.Current.LocalSettings.Values[settingKey] = value;
        }

        public void ClearLocalSettings()
        {
            ApplicationData.Current.LocalSettings.Values.Clear();
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
