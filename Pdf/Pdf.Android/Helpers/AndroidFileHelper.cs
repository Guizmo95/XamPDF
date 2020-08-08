using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Pdf.Droid.Helpers;
using Pdf.Interfaces;


[assembly: Xamarin.Forms.Dependency(typeof(AndroidFileHelper))]
namespace Pdf.Droid.Helpers
{
    public class AndroidFileHelper : IAndroidFileHelper
    {
        public async Task<Dictionary<bool, string>> SaveAndReturnStatus(MemoryStream stream, string FilePath)
        {
            var saveStatus = new Dictionary<bool, string>();

            try
            {
                await SaveFile(stream, FilePath);

                saveStatus.Add(true, null);
            }
            catch (Exception ex)
            {
                var message = $"Error when saving the PDF : " + ex.Message;

                saveStatus.Add(false, message);
            }

            return saveStatus;
        }

        private static async Task SaveFile(MemoryStream stream, string FilePath)
        {
            var file = new Java.IO.File(FilePath);
            var filePath = file.Path;
            if (file.Exists()) file.Delete();
            var outs = new Java.IO.FileOutputStream(file);
            await outs.WriteAsync(stream.ToArray());
            var ab = file.Path;
            outs.Flush();
            outs.Close();
        }

        public List<FileInfo> GetPdfFiles()
        {
            var context = Android.App.Application.Context;

            var path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            var files = System.IO.Directory.GetFiles(path.ToString(), "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".pdf"))
                .ToList();
            files.Sort();

            var filesInfos = new List<FileInfo>();

            files.ForEach(delegate(string file)
            {
                filesInfos.Add(new FileInfo(file));
            });

            return filesInfos;
        }

        public MemoryStream GetFileStream(string filePath)
        {
            var path = Android.OS.Environment.ExternalStorageDirectory.Path;

            return new MemoryStream(System.IO.File.ReadAllBytes(filePath));
        }

    }
    
}