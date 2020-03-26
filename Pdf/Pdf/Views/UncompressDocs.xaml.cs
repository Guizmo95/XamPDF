using Pdf.Helpers;
using Pdf.Interfaces;
using Pdf.Models;
using Pdf.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UncompressDocs : ContentPage
    {
        private FileEndpoint fileEndpoint = new FileEndpoint();

        public UncompressDocs()
        {
            InitializeComponent();

            IPdfPickerAndroid pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();
            FilesList.ItemsSource = pdfPickerAndroid.GetPdfFilesInDocuments();
        }

        async void StartUploadHandler(object sender, System.EventArgs e)
        {

            var fileInfo = (FileInfo)FilesList.SelectedItem;

            Progress<UploadBytesProgress> progressReporterForUpload = new Progress<UploadBytesProgress>();
            progressReporterForUpload.ProgressChanged += (s, args) => UpdateProgress((double)(100 * args.PercentComplete)/200);

            Progress<DownloadBytesProgress> progressReporterForDownload = new Progress<DownloadBytesProgress>();
            progressReporterForDownload.ProgressChanged += (s, args) => UpdateProgress((styledProgressBar.Progress) + ((double)(100 * args.PercentComplete)/200));

            await Task.Run(async () =>
            { 
                string fileName = await fileEndpoint.UploadFilesForUncompress(fileInfo, progressReporterForUpload);

                await Download(fileName, progressReporterForDownload);
            });

            
            DependencyService.Get<IToastMessage>().ShortAlert("Download start");
        }

        void UpdateProgress(double obj)
        {
            styledProgressBar.Progress = obj;
        }

        private async Task Download(string fileName, Progress<DownloadBytesProgress> progressReporter)
        {
            byte[] buffer = await DownloadHelper.CreateDownloadTask("http://10.0.2.2:44560/GetFile/" + fileName, progressReporter);
            DependencyService.Get<IAndroidFileHelper>().SaveFileInDownloads(fileName, buffer);
        }


    }



}