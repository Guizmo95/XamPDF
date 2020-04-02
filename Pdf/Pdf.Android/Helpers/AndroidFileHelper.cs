using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ICSharpCode.SharpZipLib.Zip;
using Java.IO;
using Pdf.Droid;
using Pdf.Droid.Helpers;
using Pdf.Interfaces;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidFileHelper))]
namespace Pdf.Droid.Helpers
{
    public class AndroidFileHelper : IAndroidFileHelper
    {

        public async Task SaveAndView(string fileName, String contentType, MemoryStream stream)
        {
            string directoryDownload = null;
            //Get the root path in android device.

            directoryDownload = (string)global::Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);

            //Create directory and file 
            Java.IO.File myDir = new Java.IO.File(directoryDownload);
            myDir.Mkdir();

            Java.IO.File file = new Java.IO.File(myDir, fileName);

            //Remove if the file exists
            if (file.Exists()) file.Delete();

            //Write the stream into the file
            FileOutputStream outs = new FileOutputStream(file);
            outs.Write(stream.ToArray());

            outs.Flush();
            outs.Close();

            //Invoke the created file for viewing
            //if (file.Exists())
            //{
            //    Android.Net.Uri path = Android.Net.Uri.FromFile(file);
            //    string extension = Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
            //    string mimeType = Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
            //    Intent intent = new Intent(Intent.ActionView);
            //    intent.SetDataAndType(path, mimeType);
            //    Forms.Context.StartActivity(Intent.CreateChooser(intent, "Choose App"));
            //}
        }
        public void SaveFileInDownloads(string filename, byte[] file)
        {
            string directoryDownload = (string)global::Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);

            var path = Path.Combine(directoryDownload, filename);

            System.IO.File.WriteAllBytes(path, file);
        }

        public string Convert()
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