using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using OfflineMediaV3.Common.Enums;
using OfflineMediaV3.Common.Framework.Services.Interfaces;

namespace OfflineMediaV3.DroidNative.Services
{
    public class StorageService : IStorageService
    {
        public async Task<string> GetTextOfFileByKey(FileKeys key)
        {
            try
            {
                var res = await Task.Run(() =>
                {

                    using (var i = new StreamReader(RuntimeObjects.Context.OpenFileInput(key.ToString())))
                    {
                        return i.ReadToEnd();
                    }
                });
                return res;
            }
            catch
            {
                return "";
            }
        }

        public async Task<bool> SaveFileByKey(FileKeys key, string content)
        {
            await Task.Run(() =>
            {
                using (var o = new StreamWriter(RuntimeObjects.Context.OpenFileOutput(key.ToString(), FileCreationMode.Private)))
                {
                    o.Write(content);
                }
            });
            return true;
        }

        public async Task<string> GetSettingsJson()
        {
            var res = await Task.Run(() =>
            {
                string content;
                using (StreamReader sr = new StreamReader(RuntimeObjects.Context.Assets.Open("Settings.json")))
                {
                    content = sr.ReadToEnd();
                }
                return content;
            });
            return res;
        }

        public async Task<string> GetSourceJson()
        {
            var res = await Task.Run(() =>
            {
                string content;
                using (StreamReader sr = new StreamReader(RuntimeObjects.Context.Assets.Open("Source.json")))
                {
                    content = sr.ReadToEnd();
                }
                return content;
            });
            return res;
        }

        public async Task<ulong> GetFileSizes()
        {
            ulong res = 0;
            var files = await RuntimeObjects.Context.GetExternalFilesDir(null).ListFilesAsync();
            if (files != null)
                foreach (var file in files)
                {
                    res += Convert.ToUInt64(file.Length());
                }

            return res;
        }

#pragma warning disable 1998
        public async Task<bool> ClearFiles()
#pragma warning restore 1998
        {

            AlertDialog.Builder builder = new AlertDialog.Builder(RuntimeObjects.Context);
            builder.SetMessage("Die Anwendung wird zurückgesetzt und geschlossen. Alle Einstellungen gehen verloren.");
            builder.SetTitle("Anwendung zurücksetzten");
            builder.SetPositiveButton("zurücksetzten", ResetApplication);
            builder.SetNegativeButton("abbrechen", DoNothing);
            // Create the AlertDialog object and return it
            builder.Create();


            return true;
        }

        private void DoNothing(object sender, DialogClickEventArgs e)
        {
            //doing nothing
        }

        private async void ResetApplication(object sender, DialogClickEventArgs e)
        {
            await Task.Run(() =>
            {
                using (var o = new StreamWriter(RuntimeObjects.Context.OpenFileOutput("DELETEALL", FileCreationMode.Private)))
                {
                    o.Write("yes");
                }
            });
        }
        
#pragma warning disable 1998
        public async Task<string> GetFilePathByKey(FileKeys fileKeys)
#pragma warning restore 1998
        {
            return RuntimeObjects.Context.GetExternalFilesDir(null).AbsolutePath + "database.db";
        }
    }
}