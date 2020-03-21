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
    public partial class UncompressDocs : ContentPage
    {
        private FileEndpoint fileEndpoint = new FileEndpoint();
        public UncompressDocs()
        {
            InitializeComponent();

            IPdfPickerAndroid pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();
            FilesList.ItemsSource = pdfPickerAndroid.GetPdfFilesInDocuments();
        }

        private async void StartTheConvertion(object sender, EventArgs e)
        {
            var filesInfo = FilesList.SelectedItems.Cast<FileInfo>().ToList();

            List<string> filesNamesGenerated = await fileEndpoint.UploadFilesForUncompress(filesInfo);

            await Navigation.PushAsync(new GetDownload(filesNamesGenerated));
        }
    }
}