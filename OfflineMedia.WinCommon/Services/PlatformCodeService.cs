using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.Web.Http;
using OfflineMedia.Business.Services;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.DisplayHelper;

namespace OfflineMedia.Services
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
                            if (decoder.OrientedPixelHeight > ResolutionHelper.Instance.HeightOfDevice ||
                                decoder.OrientedPixelWidth > ResolutionHelper.Instance.WidthOfDevice)
                            {
                                var resizedStream = new InMemoryRandomAccessStream();
                                BitmapEncoder encoder =
                                    await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                                double widthRatio = ResolutionHelper.Instance.WidthOfDevice /
                                                    decoder.OrientedPixelWidth;
                                double heightRatio = ResolutionHelper.Instance.HeightOfDevice /
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
    }
}
