using Pdf.Enumerations;
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
    public partial class FilesListForConvert : ContentPage
    {
        ProcessNames processNames;
        public FilesListForConvert(ProcessNames processNames)
        {
            InitializeComponent();

            this.processNames = processNames;

            IPdfPickerAndroid pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();

            FilesList.ItemsSource = pdfPickerAndroid.GetPdfFilesInDocuments();
        }

        private async void FilesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }


            var fileInfo = (FileInfo)e.SelectedItem;

            switch (processNames)
            {
                case ProcessNames.ConcatePages:
                    await Navigation.PushAsync(new ConcatePageThumbnails(fileInfo));
                    break;
                case ProcessNames.DeconcatePages:
                    await Navigation.PushAsync(new DeconcatePageThumbnails(fileInfo));
                    break;
                case ProcessNames.AddWatermark:
                    await Navigation.PushAsync(new AddWatermark(fileInfo));
                    break;
                case ProcessNames.AddStamp:
                    await Navigation.PushAsync(new AddStamp(fileInfo));
                    break;
                case ProcessNames.RemovePages:
                    await Navigation.PushAsync(new RemovePages(fileInfo));
                    break;
                case ProcessNames.AddSummary:
                    await Navigation.PushAsync(new AddSummary(fileInfo));
                    break;
            }

            

            //TODO -- PROGRAM FOR VIEWS PDF
        }


    }
}