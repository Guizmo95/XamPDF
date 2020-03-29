using Acr.UserDialogs;
using Pdf.Api;
using Pdf.Helpers;
using Pdf.Interfaces;
using Pdf.Models;
using Pdf.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    //TODO -- CHECK IF THIS CLASS WORK
    public partial class DeconcatePageThumbnails : ContentPage
    {
        //TODO - REDIMENSIONNEMENT DYNAMIQUE
        private readonly FileInfo fileInfo;
        private readonly IDeconcateEndpoint deconcateEndpoint;
        private ItemsViewModel viewModel;

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

        public DeconcatePageThumbnails(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;
            this.deconcateEndpoint = App.Container.Resolve<IDeconcateEndpoint>();

            BindingContext = viewModel = new ItemsViewModel(fileInfo);
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

            stkl.Children.Clear();

            ProgressBar progressBar = new ProgressBar();
            stkl.Children.Add(progressBar);

            Progress<UploadBytesProgress> progressReporterForUpload = new Progress<UploadBytesProgress>();
            progressReporterForUpload.ProgressChanged += (s, args) => UpdateProgress((double)(args.PercentComplete), progressBar);

            Progress<DownloadBytesProgress> progressReporterForDownload = new Progress<DownloadBytesProgress>();
            progressReporterForDownload.ProgressChanged += (s, args) => UpdateProgress((double)(args.PercentComplete), progressBar);

            await Task.Run(async () =>
            {
                var HttpResponse = await deconcateEndpoint.UploadFilesForDeconcate(fileInfo, pagesNumbers, progressReporterForUpload);
                string fileName = HttpResponse;

                await Download(fileName, progressReporterForDownload);
            });

            DependencyService.Get<IToastMessage>().ShortAlert("File downloaded");
        }

        private void SelectAllItems(object sender, EventArgs e)
        {
            CollectionViewThumbnails.SelectedItems = CollectionViewThumbnails.ItemsSource.Cast<object>().ToList();
        }

        void UpdateProgress(double obj, ProgressBar progressBar)
        {
            if(progressBar.Progress < 0.5)
            {
                progressBar.Progress = obj;
            }
            else
            {
                if(progressBar.Progress >= 0.5)
                {
                    progressBar.Progress += obj;
                }
            }
            
        }

        //TODO -- NEED TO RETURN LIST OF FILES NAMES
        private async Task Download(string fileName, Progress<DownloadBytesProgress> progressReporter)
        {
            await DownloadHelper.CreateDownloadTask("http://10.0.2.2:44560/GetFile/", fileName, progressReporter);

            await DependencyService.Get<IAndroidFileHelper>().UnzipFileInDownload(fileName);
        }
    }
}