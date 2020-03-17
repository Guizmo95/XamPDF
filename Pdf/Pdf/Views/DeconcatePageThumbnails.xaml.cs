using Acr.UserDialogs;
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
    public partial class DeconcatePageThumbnails : ContentPage
    {
        //TODO - REDIMENSIONNEMENT DYNAMIQUE
        private readonly FileInfo fileInfo;
        FileEndpoint fileEndpoint = new FileEndpoint();

        //protected async override void OnAppearing()
        //{
        //    IGetThumbnails getThumbnails = DependencyService.Get<IGetThumbnails>();
        //    string directoryPath = "";

        //    Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading...", MaskType.Black));

        //    await Task.Run(() =>
        //    {
        //        directoryPath = getThumbnails.GetBitmaps(fileInfo.FullName).Result;

        //    }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
        //    {
        //        List<ThumbnailsModel> thumbnailsModels = new List<ThumbnailsModel>();

        //        int i = 1;
        //        Directory.GetFiles(directoryPath).ToList<string>().ForEach(delegate (string thumbnailsEmplacement)
        //        {
        //            thumbnailsModels.Add(new ThumbnailsModel(i, thumbnailsEmplacement));
        //            i++;
        //        });
        //        CollectionViewThumbnails.ItemsSource = thumbnailsModels;
        //        UserDialogs.Instance.HideLoading();
        //    }
        //)
        //);
        //}

        public DeconcatePageThumbnails(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;

            //IGetThumbnails getThumbnails = DependencyService.Get<IGetThumbnails>();

            //string directoryPath = getThumbnails.GetBitmaps(fileInfo.FullName).Result;

            //List<ThumbnailsModel> thumbnailsModels = new List<ThumbnailsModel>();

            //int i = 1;
            //Directory.GetFiles(directoryPath).ToList<string>().ForEach(delegate (string thumbnailsEmplacement)
            //{
            //    thumbnailsModels.Add(new ThumbnailsModel(i, thumbnailsEmplacement));
            //    i++;
            //});

            //CollectionViewThumbnails.ItemsSource = thumbnailsModels;
        }

        private async void StartProcessDeconcatePages(object sender, EventArgs e)
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

            List<string> filesNamesGenerated = await fileEndpoint.UploadFilesForDeconcate(fileInfo, pagesNumbers);

            await Navigation.PushAsync(new GetDownload(filesNamesGenerated));
        }

        private void SelectAllItems(object sender, EventArgs e)
        {
            CollectionViewThumbnails.SelectedItems = CollectionViewThumbnails.ItemsSource.Cast<object>().ToList();
        }
    }
}