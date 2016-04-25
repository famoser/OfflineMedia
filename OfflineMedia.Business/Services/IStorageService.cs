using System.Threading.Tasks;
using OfflineMedia.Business.Enums;

namespace OfflineMedia.Business.Services
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
