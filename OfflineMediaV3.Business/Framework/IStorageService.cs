using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Enums;

namespace OfflineMediaV3.Business.Framework
{
    public interface IStorageService
    {
        Task<string> GetTextOfFileByKey(FileKeys key);
        Task<bool> SaveFileByKey(FileKeys key, string content);
        Task<string> GetSettingsJson();
        Task<string> GetSourceJson();

        Task<string> GetFilePathByKey(FileKeys fileKeys);
    }
}
