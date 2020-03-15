using Pdf.Interfaces;
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
    public partial class AddStamp : ContentPage
    {
        private readonly FileInfo fileInfo;
        FileEndpoint fileEndpoint = new FileEndpoint();
        public AddStamp(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;

            IPdfPickerAndroid pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();

            FilesList.ItemsSource = pdfPickerAndroid.GetPdfFilesInDocuments();
        }

        private async void StartTheConvertion(object sender, EventArgs e)
        {
            List<FileInfo> filesInfoWatermark = new List<FileInfo>();

            filesInfoWatermark.Add((FileInfo)FilesList.SelectedItem);
            filesInfoWatermark.Insert(0, fileInfo);

            string fileNameGenerated = await fileEndpoint.UploadFilesForStump(filesInfoWatermark);

            await Navigation.PushAsync(new GetDownload(fileNameGenerated));
        }
    }
}