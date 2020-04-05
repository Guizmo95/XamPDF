using Pdf.Helpers;
using Pdf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pdf.ViewModels
{
    public class ThumbnailsViewModel
    {
        public IList<ThumbnailsModel> Thumbnails { get; set; }
        private readonly FileInfo fileInfo;

        public ThumbnailsViewModel(FileInfo fileInfo)
        {
            Thumbnails = new List<ThumbnailsModel>();
            this.fileInfo = fileInfo;
            //PdfToImage pdfToImage = new PdfToImage(fileInfo);

            var appDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string directoryPath = System.IO.Path.Combine(appDirectory, "thumbnailsTemp");

            string[] files =  Directory.GetFiles(directoryPath);

            int i = 0;
            foreach(string file in files)
            {
                ThumbnailsModel thumbnailsModel = new ThumbnailsModel(i, file);
                Thumbnails.Add(thumbnailsModel);
                i++;
            }
        }
    }
}
