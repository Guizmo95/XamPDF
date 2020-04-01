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
    public partial class RemovePages : ContentPage
    {
        private readonly FileInfo fileInfo;
        private readonly IRemovePagesEndpoint removePagesEndpoint;
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

        //TODO -- HANDLE IF SELECT ALL PAGES
        public RemovePages(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;
            this.removePagesEndpoint = App.Container.Resolve<IRemovePagesEndpoint>();
            BindingContext = viewModel = new ItemsViewModel(fileInfo);

        }

        private async void StartProcessRemovePages(object sender, EventArgs e)
        {
            if(CollectionViewThumbnails.SelectedItems.Count == 0)
                DependencyService.Get<IToastMessage>().LongAlert("Please select a page");
            else
            {
                List<ThumbnailsModel> itemsSelected = CollectionViewThumbnails.SelectedItems.Cast<ThumbnailsModel>().ToList();

                int pages = DependencyService.Get<IGetThumbnails>().GetAllPages(fileInfo.FullName);
                List<int> NumberOfPagesNotRemoved = new List<int>();

                for (int i = 1; i <= pages; i++)
                {
                    if (itemsSelected.Any(p => p.PageNumber == i))
                    {
                        return;
                    }
                    else
                    {
                        NumberOfPagesNotRemoved.Add(i);
                    }

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
                    string fileName = await removePagesEndpoint.UploadFilesForRemovePages(fileInfo, NumberOfPagesNotRemoved, progressReporterForUpload);

                    await Download(fileName, progressReporterForDownload);
                });

                DependencyService.Get<IToastMessage>().ShortAlert("File downloaded");

                await Task.Run(() =>
                {
                    DependencyService.Get<IGetThumbnails>().DeleteThumbnailsRepository(fileInfo.FullName);
                });
            }
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