using Acr.UserDialogs;
using Android.Arch.Lifecycle;
using Android.Graphics.Pdf;
using Pdf.Api;
using Pdf.Enumerations;
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
using Unity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConcatePageThumbnails : ContentPage
    {
        private readonly IConcateEndpoint concateEndpoint;
        private readonly FileInfo fileInfo;
        private ItemsViewModel viewModel;

        //TODO -- DELETE SELECTION

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

        //TODO -- Gerer le retour 
        //TODO -- HANDLE IF SELECT ALL
        public ConcatePageThumbnails(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;
            this.concateEndpoint = App.Container.Resolve<IConcateEndpoint>();

            BindingContext = viewModel = new ItemsViewModel(fileInfo);
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

            stkl.Children.Clear();

            ProgressBar progressBar = new ProgressBar();
            stkl.Children.Add(progressBar);

            Progress<UploadBytesProgress> progressReporterForUpload = new Progress<UploadBytesProgress>();
            progressReporterForUpload.ProgressChanged += (s, args) => UpdateProgress((double)(args.PercentComplete), progressBar);

            Progress<DownloadBytesProgress> progressReporterForDownload = new Progress<DownloadBytesProgress>();
            progressReporterForDownload.ProgressChanged += (s, args) => UpdateProgress((double)(args.PercentComplete), progressBar);

            await Task.Run(async () =>
            {
                string fileNameGenerated = await concateEndpoint.UploadFilesForConcate(fileInfo, pagesNumbers, progressReporterForUpload);

                await Download(fileNameGenerated, progressReporterForDownload);
            });

            DependencyService.Get<IToastMessage>().ShortAlert("File downloaded");
        }

        void UpdateProgress(double obj, ProgressBar progressBar)
        {
            if (progressBar.Progress < 0.5)
            {
                progressBar.Progress = obj;
            }
            else
            {
                if (progressBar.Progress >= 0.5)
                {
                    progressBar.Progress += obj;
                }
            }
        }

        private async Task Download(string fileName, Progress<DownloadBytesProgress> progressReporter)
        {
            await DownloadHelper.CreateDownloadTask("http://10.0.2.2:44560/GetFile/", fileName, progressReporter);
        }
    }
}