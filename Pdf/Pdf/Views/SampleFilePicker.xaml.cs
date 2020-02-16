using Pdf.Droid;
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
    public partial class SampleFilePicker : ContentPage
    {
        public SampleFilePicker()
        {
            InitializeComponent();

            IPdfPickerAndroid pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();
            
            FilesList.ItemsSource = pdfPickerAndroid.GetPdfFilesInDocuments();

        }

        private void FilesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}