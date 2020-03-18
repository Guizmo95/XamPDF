using Pdf.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pdf.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class DocumentsList : ContentPage
    {
        public DocumentsList()
        {
            InitializeComponent();

            IPdfPickerAndroid pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();

            FilesList.ItemsSource = pdfPickerAndroid.GetPdfFilesInDocuments();
        }

        private async void FilesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem ==null)
            {
                return;
            }

            
            var fileInfo = (FileInfo)e.SelectedItem;

            using (Stream stream = File.OpenRead(fileInfo.FullName))
            {
                await Navigation.PushAsync(new PdfViewer(stream));
            }

            //WAIT UNTIL ITS FIX
            //Application.Current.MainPage = new NavigationPage(new ToolsList(fileInfo));

            //TODO -- PROGRAM FOR VIEWS PDF
        }
    }
}
    

