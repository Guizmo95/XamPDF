using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Pdf.Droid;
using Pdf.Models;

[assembly: Xamarin.Forms.Dependency(typeof(PdfPickerAndroid))]
namespace Pdf.Droid
{
    
    public class PdfPickerAndroid:IPdfPickerAndroid 
    {
        public void GetPdfFilesInDocuments()
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.Path;

            List<string> files = (List<string>)System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".pdf"));

            List<FileInfo> filesInfos = new List<FileInfo>();

            files.ForEach(delegate(String file)
            {
                filesInfos.Add(new FileInfo(file));
            });

            //return fileInfos



            Console.WriteLine(files);

            //return files;
        }

    }
}