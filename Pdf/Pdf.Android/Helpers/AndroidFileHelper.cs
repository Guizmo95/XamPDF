using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Pdf.Droid;
using Pdf.Droid.Helpers;
using Pdf.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidFileHelper))]
namespace Pdf.Droid.Helpers
{
    public class AndroidFileHelper : IAndroidFileHelper
    {
        public void SaveFileInDownloads(string filename, byte[] file)
        {
            string directoryDownload = (string)global::Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);

            var path = Path.Combine(directoryDownload, filename);

            File.WriteAllBytes(path, file);
        }

        public void SaveFileInDocFolder(string fileName, byte[] file)
        {
            string appFolder = (string)global::Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);

            var path = Path.Combine(appFolder, fileName);

            File.WriteAllBytes(path, file);
        }

        public byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public byte[] LoadLocalFile(string filePath)
        {
            string directoryDownload = (string)global::Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            if (File.Exists(filePath)) return File.ReadAllBytes(filePath);
            return null;
        }

        public bool IsDirectoryEmpty(string directoryPath)
        {
            return !Directory.EnumerateFiles(directoryPath).Any();
        }
    }
    
}