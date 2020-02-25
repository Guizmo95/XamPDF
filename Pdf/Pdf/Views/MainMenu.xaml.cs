using Pdf.Droid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pdf
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainMenu : Shell
    {
        public MainMenu()
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


            await Navigation.PushAsync(new ToolsList(fileInfo));

        }
    }
}
    

