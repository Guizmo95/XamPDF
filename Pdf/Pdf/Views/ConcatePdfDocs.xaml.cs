using Pdf.Api;
using Pdf.Helpers;
using Pdf.Interfaces;
using Pdf.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConcatePdfDocs : ContentPage
    {
        private readonly IConcateEndpoint concateEndpoint;
        public ConcatePdfDocs()
        {
            InitializeComponent();

            this.concateEndpoint = App.Container.Resolve<IConcateEndpoint>();

            IPdfPickerAndroid pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();

            FilesList.ItemsSource = pdfPickerAndroid.GetPdfFilesInDocuments(); 
        }

        
        //TODO -- ADD COUNTER FOR FILES CHOICE
        private async void StartTheConvertion(object sender, EventArgs e)
        {
            if(FilesList.SelectedItem == null)
                DependencyService.Get<IToastMessage>().LongAlert("Please select a document");
            else
            {
                var filesInfo = FilesList.SelectedItems.Cast<FileInfo>().ToList();

                stkl.Children.Clear();

                ProgressBar progressBar = new ProgressBar();
                stkl.Children.Add(progressBar);

                Progress<UploadBytesProgress> progressReporterForUpload = new Progress<UploadBytesProgress>();
                progressReporterForUpload.ProgressChanged += (s, args) => UpdateProgress((double)(args.PercentComplete), progressBar);

                Progress<DownloadBytesProgress> progressReporterForDownload = new Progress<DownloadBytesProgress>();
                progressReporterForDownload.ProgressChanged += (s, args) => UpdateProgress((double)(args.PercentComplete), progressBar);

                await Task.Run(async () =>
                {
                    string fileNameGenerated = await concateEndpoint.UploadFilesForConcateDocs(filesInfo, progressReporterForUpload);

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