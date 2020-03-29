using Pdf.Api;
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
using Unity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UncompressDocs : ContentPage
    {
        private readonly IUncompressEndpoint uncompressEndpoint;

        public UncompressDocs()
        {
            InitializeComponent();

            this.uncompressEndpoint = App.Container.Resolve<IUncompressEndpoint>();

            IPdfPickerAndroid pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();
            FilesList.ItemsSource = pdfPickerAndroid.GetPdfFilesInDocuments();
        }

        async void StartUploadHandler(object sender, System.EventArgs e)
        {

            var fileInfo = (FileInfo)FilesList.SelectedItem;

            Progress<UploadBytesProgress> progressReporterForUpload = new Progress<UploadBytesProgress>();
            progressReporterForUpload.ProgressChanged += (s, args) => UpdateProgress((double)(args.PercentComplete));

            Progress<DownloadBytesProgress> progressReporterForDownload = new Progress<DownloadBytesProgress>();
            progressReporterForDownload.ProgressChanged += (s, args) => UpdateProgress((double)(args.PercentComplete));

            await Task.Run(async () =>
            { 
                string fileName = await uncompressEndpoint.UploadFilesForUncompress(fileInfo, progressReporterForUpload);

                await Download(fileName, progressReporterForDownload);
            });
            
           
        }

        void UpdateProgress(double obj)
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