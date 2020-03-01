using Android.Graphics.Pdf;
using Pdf.Enumerations;
using Pdf.Interfaces;
using Pdf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConcatePageThumbnails : ContentPage
    {
        private readonly FileInfo fileInfo;
        FileEndpoint fileEndpoint = new FileEndpoint();

        //TODO -- Gerer le retour 
        public ConcatePageThumbnails(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;

            IGetThumbnails getThumbnails = DependencyService.Get<IGetThumbnails>();

            //TODO -- CREATE BITMAP MODEL

            string directoryPath = getThumbnails.GetBitmaps(fileInfo.FullName);
            
            CollectionViewThumbnails.ItemsSource = Directory.GetFiles(directoryPath);
        }

        private void StartProcessConcatePages(object sender, EventArgs e)
        {
            List<string> items = CollectionViewThumbnails.SelectedItems.Cast<string>().ToList();

            if (items == null)
            {
                return;
            }

            List<FileInfo> fileInfos = new List<FileInfo>();

            //TODO -- TRANSFORM PDF PAGES TO STREAM

            items.ForEach(delegate (string item) {
                fileInfos.Add(new FileInfo(item));
            });

            PdfRenderer.Page page = 

            //fileEndpoint.UploadFiles(fileInfos, ProcessNames.ConcatePages);
             
        }
    }
}