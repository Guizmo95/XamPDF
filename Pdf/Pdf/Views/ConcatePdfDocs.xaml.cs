using Pdf.Interfaces;
using Pdf.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConcatePdfDocs : ContentPage
    {

        private FileEndpoint fileEndpoint = new FileEndpoint();
        public ConcatePdfDocs()
        {
            InitializeComponent();

            IPdfPickerAndroid pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();

            FilesList.ItemsSource = pdfPickerAndroid.GetPdfFilesInDocuments(); 
        }

        
        //TODO -- ADD COUNTER FOR FILES CHOICE
        private async void StartTheConvertion(object sender, EventArgs e)
        {
            var filesInfo = FilesList.SelectedItems.Cast<FileInfo>().ToList();

            string fileNameGenerated = await fileEndpoint.UploadFilesForConcate(filesInfo);

            await Navigation.PushAsync(new GetDownload(fileNameGenerated));
        }

    }
}