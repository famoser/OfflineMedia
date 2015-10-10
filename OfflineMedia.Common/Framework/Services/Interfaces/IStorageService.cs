using System.Threading.Tasks;
using OfflineMedia.Common.Enums;

namespace OfflineMedia.Common.Framework.Services.Interfaces
{
    public interface IStorageService
    {
        Task<string> GetTextOfFileByKey(FileKeys key);
        Task<bool> SaveFileByKey(FileKeys key, string content);
        Task<string> GetSettingsJson();
        Task<string> GetSourceJson();
        Task<string> GetWeatherFontJson();

        Task<ulong> GetFileSizes();
        Task<bool> ClearFiles();

        Task<string> GetFilePathByKey(FileKeys fileKeys);
    }
}
