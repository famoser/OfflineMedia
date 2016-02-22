using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Business.Services;

namespace OfflineMedia.Business.Helpers
{
    public class CompressionHelper
    {
        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer,0,buffer.Length);
            }
        }

        public static Task<byte[]> ResizeImage(byte[] image, IPlatformCodeService platformCodeService)
        {
            return platformCodeService.ResizeImage(image);
        }
    }
}
