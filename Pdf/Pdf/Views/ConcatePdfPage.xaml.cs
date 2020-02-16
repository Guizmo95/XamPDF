using Pdf.Droid;
using Pdf.Views;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConcatePdfPage : ContentPage
    {

        private FileInfo fileInfo1;
        private FileEndpoint fileEndpoint = new FileEndpoint();
        public ConcatePdfPage(FileInfo fileInfo)
        {
            InitializeComponent();
            this.fileInfo1 = fileInfo;

            IPdfPickerAndroid pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();

            FilesList.ItemsSource = pdfPickerAndroid.GetPdfFilesInDocuments(); 
        }

        

        private async void StartTheConvertion(object sender, EventArgs e)
        {
            var filesInfo = FilesList.SelectedItems.Cast<FileInfo>().ToList();
            filesInfo.Insert(0, fileInfo1);

            await fileEndpoint.UploadFiles(filesInfo);
           
            
        }



        //var httpClient = new HttpClient();

        //var uploadServiceBaseAdress = "http://10.0.2.2:51549/api/Files/Upload";

        //var httpResponseMessage = await httpClient.GetAsync(uploadServiceBaseAdress);

        //var status = await httpResponseMessage.Content.ReadAsStringAsync();

        //Console.WriteLine(status);

    }
}