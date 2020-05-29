using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ICSharpCode.SharpZipLib.Zip;
using Java.IO;
using Pdf.Droid;
using Pdf.Droid.Helpers;
using Pdf.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidFileHelper))]
namespace Pdf.Droid.Helpers
{
    public class AndroidFileHelper : IAndroidFileHelper
    {
        public Dictionary<bool, string> Save(MemoryStream stream, string FilePath)
        {
            Dictionary<bool, string> saveStatus = new Dictionary<bool, string>();
            bool saved = false;

            try
            {
                Java.IO.File file = new Java.IO.File(FilePath);
                string filePath = file.Path;
                if (file.Exists()) file.Delete();
                Java.IO.FileOutputStream outs = new Java.IO.FileOutputStream(file);
                outs.Write(stream.ToArray());
                var ab = file.Path;
                outs.Flush();
                outs.Close();

                saveStatus.Add(true, null);
            }
            catch (Exception ex)
            {
                string message = $"Error when saving the PDF : " + ex.Message;

                saveStatus.Add(false, message);
            }

            return saveStatus;
        }

        public void SaveFileInDownloads(string filename, byte[] file)
        {
            string directoryDownload = (string)global::Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);

            var path = Path.Combine(directoryDownload, filename);

            System.IO.File.WriteAllBytes(path, file);
        }

        public MemoryStream GetFileStream(string filePath)
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.Path;

            return new MemoryStream(System.IO.File.ReadAllBytes(filePath));
        }

        public string GetDownloadPath()
        {
            string directoryDownload = (string)global::Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);

            return directoryDownload;
        }

        public async Task<List<string>> UnzipFileInDownload(string fileName)
        {
            string directoryDownload = (string)global::Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);

            var zipPath = Path.Combine(directoryDownload, fileName);

            List<string> filesNames = new List<string>();

            try
            {
                var entry = new ZipEntry(Path.GetFileNameWithoutExtension(fileName));
                var fileStreamIn = new FileStream(zipPath, FileMode.Open, FileAccess.Read);
                var zipInStream = new ZipInputStream(fileStreamIn);
                entry = zipInStream.GetNextEntry();
                while (entry != null && entry.CanDecompress)
                {
                    var outputFile = Path.Combine(directoryDownload, entry.Name);
                    var outputDirectory = Path.GetDirectoryName(outputFile);

                    if (entry.IsFile)
                    {
                        var fileStreamOut = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                        int size;
                        byte[] buffer = new byte[4096];
                        do
                        {
                            size = await zipInStream.ReadAsync(buffer, 0, buffer.Length);
                            await fileStreamOut.WriteAsync(buffer, 0, size);
                        } while (size > 0);
                        fileStreamOut.Close();
                    }
                    filesNames.Add(entry.Name);
                    entry = zipInStream.GetNextEntry();
                }
                zipInStream.Close();
                fileStreamIn.Close();

                System.IO.File.Delete(zipPath);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return filesNames;
        }

        public void SaveFileInDocFolder(string fileName, byte[] file)
        {
            string appFolder = (string)global::Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);

            var path = Path.Combine(appFolder, fileName);

            System.IO.File.WriteAllBytes(path, file);
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
            if (System.IO.File.Exists(filePath)) return System.IO.File.ReadAllBytes(filePath);
            return null;
        }

        public bool IsDirectoryEmpty(string directoryPath)
        {
            return !Directory.EnumerateFiles(directoryPath).Any();
        }
    }
    
}