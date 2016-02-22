using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMedia.Business.Services
{
    public interface IPlatformCodeService
    {
        Task<byte[]> ResizeImage(byte[] image);
    }
}
