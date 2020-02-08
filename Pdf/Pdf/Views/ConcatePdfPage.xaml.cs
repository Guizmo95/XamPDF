using Pdf.Views;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
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

        private FileData fileData1;
        private FileData fileData2;
        FileEndpoint fileEndpoint = new FileEndpoint();
        public ConcatePdfPage(FileData fileData1)
        {
            InitializeComponent();
            this.fileData1 = fileData1;
        }

        private async void PickFileButton(object sender, EventArgs e)
       {
            try
            {
                fileData2 = await CrossFilePicker.Current.PickFile(new string[] { "application/pdf" });
                if (fileData2 == null)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            string fileName = await fileEndpoint.UploadFile(fileData1, fileData2);

            await Navigation.PushAsync(new GetDownload(fileName));
        }

            //var httpClient = new HttpClient();

            //var uploadServiceBaseAdress = "http://10.0.2.2:51549/api/Files/Upload";

            //var httpResponseMessage = await httpClient.GetAsync(uploadServiceBaseAdress);

            //var status = await httpResponseMessage.Content.ReadAsStringAsync();

            //Console.WriteLine(status);
        
    }
}