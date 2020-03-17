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
    public partial class RemovePages : ContentPage
    {
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
        public RemovePages(FileInfo fileInfo)
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

        private async void StartProcessRemovePages(object sender, EventArgs e)
        {
            List<ThumbnailsModel> itemsSelected = CollectionViewThumbnails.SelectedItems.Cast<ThumbnailsModel>().ToList();
            List<ThumbnailsModel> allItems = CollectionViewThumbnails.ItemsSource.Cast<ThumbnailsModel>().ToList();

            if (itemsSelected == null)
            {
                return;
            }

            List<int> pagesNumbers = new List<int>();


            allItems.ForEach(delegate (ThumbnailsModel thumbnailsModel)
            {
                if(itemsSelected.Contains(thumbnailsModel) == false)
                {
                    pagesNumbers.Add(thumbnailsModel.PageNumber);
                }
            });

            string fileName= await fileEndpoint.UploadFilesForRemovePages(fileInfo, pagesNumbers);

            await Navigation.PushAsync(new GetDownload(fileName));
        }
    }
}