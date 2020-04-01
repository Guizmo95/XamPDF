using Android.Arch.Lifecycle;
using Pdf.Api;
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
    public partial class AddSummary : ContentPage
    {
        private List<SummaryModel> summaries = new List<SummaryModel>();
        private readonly ISummaryEndpoint summaryEndpoint;
        private readonly FileInfo fileInfo;
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

        protected override void OnDisappearing()
        {
            Task.Run(() =>
            {
                DependencyService.Get<IGetThumbnails>().DeleteThumbnailsRepository(fileInfo.FullName);
            });

            base.OnDisappearing();
        }
        public AddSummary(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;
            this.summaryEndpoint = App.Container.Resolve<ISummaryEndpoint>();

            BindingContext = viewModel = new ItemsViewModel(fileInfo);
        }

        //TODO -- HANDLE MAX LENGTH
        //TODO FINISH ADD SUMMARY PROCCESS
        private async void CollectionViewThumbnails_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            if(CollectionViewThumbnails.SelectedItem == null)
            {
                return;
            }
            else
            {
                ThumbnailsModel item = (ThumbnailsModel)CollectionViewThumbnails.SelectedItem;

                await Navigation.PushModalAsync(new AddSummaryModal(summaries, item));

            }
        }

        private async void StartProccessAddSummary(object sender, EventArgs e)
        {
            
            if (summaries.Count == 0)
            {
                DependencyService.Get<IToastMessage>().ShortAlert("No summary added");
            }

            stkl.Children.Clear();

            ProgressBar progressBar = new ProgressBar();
            stkl.Children.Add(progressBar);

            Progress<UploadBytesProgress> progressReporterForUpload = new Progress<UploadBytesProgress>();
            progressReporterForUpload.ProgressChanged += (s, args) => UpdateProgress((double)(args.PercentComplete), progressBar);

            Progress<DownloadBytesProgress> progressReporterForDownload = new Progress<DownloadBytesProgress>();
            progressReporterForDownload.ProgressChanged += (s, args) => UpdateProgress((double)(args.PercentComplete), progressBar);

            await Task.Run(async () =>
            {
                string fileNameGenerated = await summaryEndpoint.UploadFilesForSummary(fileInfo, summaries, progressReporterForUpload);

                await Download(fileNameGenerated, progressReporterForDownload);
            });

            DependencyService.Get<IToastMessage>().ShortAlert("File downloaded");

            await Task.Run(() =>
            {
                DependencyService.Get<IGetThumbnails>().DeleteThumbnailsRepository(fileInfo.FullName);
            });
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