using Pdf.Api;
using Pdf.Helpers;
using Pdf.Interfaces;
using Pdf.Models;
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
    public partial class AddWatermark : ContentPage
    {
        private readonly FileInfo fileInfo;
        private readonly IOverlayEndpoint overlayEndpoint;
        public AddWatermark(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;
            this.overlayEndpoint = App.Container.Resolve<IOverlayEndpoint>();

            IPdfPickerAndroid pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();

            FilesList.ItemsSource = pdfPickerAndroid.GetPdfFilesInDocuments();
        }

        private async void StartTheConvertion(object sender, EventArgs e)
        {
            if(FilesList.SelectedItem == null)
                DependencyService.Get<IToastMessage>().ShortAlert("Please select a file");
            else
            {
                List<FileInfo> filesInfoWatermark = new List<FileInfo>();

                filesInfoWatermark.Add((FileInfo)FilesList.SelectedItem);
                filesInfoWatermark.Insert(0, fileInfo);

                stkl.Children.Clear();

                ProgressBar progressBar = new ProgressBar();
                stkl.Children.Add(progressBar);

                Progress<UploadBytesProgress> progressReporterForUpload = new Progress<UploadBytesProgress>();
                progressReporterForUpload.ProgressChanged += (s, args) => UpdateProgress((double)(args.PercentComplete), progressBar);

                Progress<DownloadBytesProgress> progressReporterForDownload = new Progress<DownloadBytesProgress>();
                progressReporterForDownload.ProgressChanged += (s, args) => UpdateProgress((double)(args.PercentComplete), progressBar);

                await Task.Run(async () =>
                {
                    string fileNameGenerated = await overlayEndpoint.UploadFilesForWatermark(filesInfoWatermark, progressReporterForUpload);

                    await Download(fileNameGenerated, progressReporterForDownload);
                });

                DependencyService.Get<IToastMessage>().ShortAlert("File downloaded");
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