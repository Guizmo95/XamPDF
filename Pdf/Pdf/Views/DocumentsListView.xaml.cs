using Pdf.Enumerations;
using Pdf.ViewModels;
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
    public partial class DocumentsListView : ContentPage
    {
        private readonly DocumentViewModel documentViewModel;
        private readonly ProcessNames processNames;
        public DocumentsListView(ProcessNames processNames)
        {
            InitializeComponent();

            this.documentViewModel = new DocumentViewModel();
            DocumentListView.ItemsSource = documentViewModel.Documents;

            this.processNames = processNames;
        }

        private async void DocumentListView_SelectionChanging(object sender, Syncfusion.ListView.XForms.ItemSelectionChangingEventArgs e)
        {
            if (e.AddedItems == null)
            {
                return;
            }
            else
            {
                var fileInfo = (FileInfo)e.AddedItems[0];
                switch (processNames)
                {
                    case ProcessNames.View:
                        using (Stream stream = File.OpenRead(fileInfo.FullName))
                        {
                            await Navigation.PushAsync(new PdfViewer(stream));
                        }
                        break;
                    case ProcessNames.WorkingWithPages:
                        await Navigation.PushAsync(new WorkingWithPagesView(fileInfo));
                        break;
                    case ProcessNames.WorkingWithDocuments:
                        break;
                    default:
                        break;
                }
            }

            
        }
    }
}