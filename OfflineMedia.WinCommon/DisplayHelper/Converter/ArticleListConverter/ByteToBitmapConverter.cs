using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Threading;

namespace OfflineMedia.DisplayHelper.Converter.ArticleListConverter
{
    public class ByteToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var bytes = value as byte[];
            if (bytes != null)
            {
                //var img = new BitmapImage();
                //MemoryStream stream = new MemoryStream(bytes);
                //img.SetSource(stream.AsRandomAccessStream());
                //return img;

                var task = Task.Run(async () =>
                {
                    using (InMemoryRandomAccessStream raStream = new InMemoryRandomAccessStream())
                    {
                        using (DataWriter writer = new DataWriter(raStream))
                        {
                            // Write the bytes to the stream
                            writer.WriteBytes(bytes);

                            // Store the bytes to the MemoryStream
                            await writer.StoreAsync();

                            // Not necessary, but do it anyway
                            await writer.FlushAsync();

                            // Detach from the Memory stream so we don't close it
                            writer.DetachStream();
                        }

                        raStream.Seek(0);

                        BitmapImage bitMapImage = new BitmapImage();
                        bitMapImage.SetSource(raStream);
                        return bitMapImage;
                    }
                });
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
