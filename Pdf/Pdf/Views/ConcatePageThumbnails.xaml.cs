using Acr.UserDialogs;
using Android.Arch.Lifecycle;
using Android.Graphics.Pdf;
using Pdf.Enumerations;
using Pdf.Interfaces;
using Pdf.Models;
using Pdf.ViewModels;
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
        FileEndpoint fileEndpoint = new FileEndpoint();
        FileInfo fileInfo;
        ItemsViewModel viewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);

           // Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading...", MaskType.Black));

           // await Task.Run(() =>
           //{
               

           //}).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
           //{
               
           //    UserDialogs.Instance.HideLoading();
           //})
           // );
            

            MessagingCenter.Subscribe<object, ThumbnailsModel>(this, ItemsViewModel.ScrollToPreviousLastItem, (sender, item) =>
            {
                CollectionViewThumbnails.ScrollTo(item, ScrollToPosition.End);
            });
        }

        //protected override void OnAppearing()
        //{

        //    Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading...", MaskType.Black));

        //    await Task.Run(() =>
        //   {


        //   }).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
        //   {

        //       UserDialogs.Instance.HideLoading();
        //   })
        //    );
        //}

        //TODO -- Gerer le retour 
        public ConcatePageThumbnails(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;
            BindingContext = viewModel = new ItemsViewModel(fileInfo);

            //IGetThumbnails getThumbnails = DependencyService.Get<IGetThumbnails>();

            //Task.Run(() => await getThumbnails.GetBitmaps(fileInfo.FullName).Result);

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