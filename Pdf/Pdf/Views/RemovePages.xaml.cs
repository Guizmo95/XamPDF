using Pdf.Helpers;
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
    public partial class RemovePages : ContentPage
    {
        private readonly FileInfo fileInfo;
        FileEndpoint fileEndpoint = new FileEndpoint();
        ItemsViewModel viewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);

            MessagingCenter.Subscribe<object, ThumbnailsModel>(this, ItemsViewModel.ScrollToPreviousLastItem, (sender, item) =>
            {
                CollectionViewThumbnails.ScrollTo(item, ScrollToPosition.Start);
            });
        }

        //TODO -- HANDLE IF SELECT ALL PAGES
        public RemovePages(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;
            BindingContext = viewModel = new ItemsViewModel(fileInfo);

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

            stkl.Children.Clear();

            ProgressBar progressBar = new ProgressBar();
            stkl.Children.Add(progressBar);

            Progress<UploadBytesProgress> progressReporterForUpload = new Progress<UploadBytesProgress>();
            progressReporterForUpload.ProgressChanged += (s, args) => UpdateProgress((double)(100 * args.PercentComplete) / 200, progressBar);

            Progress<DownloadBytesProgress> progressReporterForDownload = new Progress<DownloadBytesProgress>();
            progressReporterForDownload.ProgressChanged += (s, args) => UpdateProgress((progressBar.Progress) + ((double)(100 * args.PercentComplete) / 200), progressBar);

            await Task.Run(async () =>
            {
                string fileName = await fileEndpoint.UploadFilesForRemovePages(fileInfo, pagesNumbers, progressReporterForUpload);

                await Download(fileName, progressReporterForDownload);
            });
            

            //await Navigation.PushAsync(new GetDownload(fileName));
        }

        void UpdateProgress(double obj, ProgressBar progressBar)
        {
            progressBar.Progress = obj;
        }

        private async Task Download(string fileName, Progress<DownloadBytesProgress> progressReporter)
        {
            byte[] buffer = await DownloadHelper.CreateDownloadTask("http://10.0.2.2:44560/GetFile/" + fileName, progressReporter);
            DependencyService.Get<IAndroidFileHelper>().SaveFileInDownloads(fileName, buffer);
        }
    }
}