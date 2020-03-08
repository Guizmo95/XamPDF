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

            string directoryPath = getThumbnails.GetBitmaps(fileInfo.FullName);

            List<ThumbnailsModel> thumbnailsModels = new List<ThumbnailsModel>();

            int i = 1;
            Directory.GetFiles(directoryPath).ToList<string>().ForEach(delegate (string thumbnailsEmplacement)
            {
                thumbnailsModels.Add(new ThumbnailsModel(i, thumbnailsEmplacement));
                i++;
            });

            CollectionViewThumbnails.ItemsSource = thumbnailsModels;
        }

        private async void StartProcessConcatePages(object sender, EventArgs e)
        {
            List<ThumbnailsModel> items = CollectionViewThumbnails.SelectedItems.Cast<ThumbnailsModel>().ToList();

            if (items == null)
            {
                return;
            }

            List<int> pagesNumbers = new List<int>();
            
            items.ForEach(delegate (ThumbnailsModel thumbnailsModel)
            {
                pagesNumbers.Add(thumbnailsModel.PageNumber);
            });

            string fileNameGenerated = await fileEndpoint.UploadFilesForConcate(fileInfo, pagesNumbers);

            await Navigation.PushAsync(new GetDownload(fileNameGenerated));
        }
    }
}