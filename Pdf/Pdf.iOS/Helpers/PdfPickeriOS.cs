using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Pdf.Droid;
using Pdf.Droid.Helpers;
using Pdf.Interfaces;
using Pdf.Models;

[assembly: Xamarin.Forms.Dependency(typeof(PdfPickeriOS))]
namespace Pdf.Droid.Helpers
{
    
    public class PdfPickeriOS: IPdfPicker
    {
        public List<FileInfo> GetPdfFiles()
        {
            //TODO -- A REFAIRE TEST SUR REELE APPAREIL
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            List<string> files = System.IO.Directory.GetFiles(documents, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".pdf"))
                .ToList();
            files.Sort();

            List<FileInfo> filesInfos = new List<FileInfo>();

            files.ForEach(delegate(String file)
            {
                filesInfos.Add(new FileInfo(file));
            });

            return filesInfos;
        }

    }
}