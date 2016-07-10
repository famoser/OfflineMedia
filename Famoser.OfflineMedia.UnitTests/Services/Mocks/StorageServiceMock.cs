﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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