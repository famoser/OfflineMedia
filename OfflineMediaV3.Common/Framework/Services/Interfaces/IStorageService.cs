using System.Threading.Tasks;
using OfflineMediaV3.Common.Enums;

namespace OfflineMediaV3.Common.Framework.Services.Interfaces
{
    public interface IStorageService
    {
        Task<string> GetTextOfFileByKey(FileKeys key);
        Task<bool> SaveFileByKey(FileKeys key, string content);
        Task<string> GetSettingsJson();
        Task<string> GetSourceJson();

        Task<ulong> GetFileSizes();
        Task<bool> ClearFiles();

        Task<string> GetFilePathByKey(FileKeys fileKeys);
    }
}
