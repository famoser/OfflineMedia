using System;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Interfaces;

#pragma warning disable 1998
namespace Famoser.OfflineMedia.UnitTests.Services.Mocks
{
    public class StorageServiceMock : IStorageService
    {
        public async Task<string> GetCachedTextFileAsync(string fileKey)
        {
            return "";
        }

        public async Task<bool> SetCachedTextFileAsync(string fileKey, string content)
        {
            return true;
        }

        public async Task<byte[]> GetCachedFileAsync(string fileKey)
        {
            return new byte[] { 1, 23 };
        }

        public async Task<bool> SetCachedFileAsync(string fileKey, byte[] content)
        {
            return true;
        }

        public Task<bool> DeleteCachedFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoamingTextFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetRoamingTextFileAsync(string filePath, string content)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetRoamingFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetRoamingFileAsync(string filePath, byte[] content)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRoamingFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetUserTextFileAsync(string fileKey)
        {
            return "";
        }

        public async Task<bool> SetUserTextFileAsync(string fileKey, string content)
        {
            return true;
        }

        public async Task<byte[]> GetUserFileAsync(string fileKey)
        {
            return new byte[] { 12, 31 };
        }

        public async Task<bool> SetUserFileAsync(string fileKey, byte[] content)
        {
            return true;
        }

        public async Task<string> GetAssetTextFileAsync(string path)
        {
            return "";
        }

        public async Task<byte[]> GetAssetFileAsync(string path)
        {
            return new byte[] { 122, 13 };
        }
    }
}
