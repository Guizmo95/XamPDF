using Pdf.Models;
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
    public partial class MainPage : ContentPage
    {
        private readonly DocumentViewModel documentViewModel;
        public MainPage()
        {
            InitializeComponent();

            this.documentViewModel = new DocumentViewModel();
            DocumentListView.ItemsSource = documentViewModel.Documents;

            List<string> list = new List<string>();
            list.Add("My documents");
            list.Add("Tools");
            list.Add("Converted files");
            list.Add("Help");
            list.Add("Feedback");
            list.Add("About");
            listView.ItemsSource = list;
        }

        void hamburgerButton_Clicked(object sender, EventArgs e)
        {
            navigationDrawer.ToggleDrawer();
        }

        private async void DocumentListView_SelectionChanging(object sender, Syncfusion.ListView.XForms.ItemSelectionChangingEventArgs e)
        {
            if (e.AddedItems == null)
            {
                return;
            }

            var fileInfo = (FileInfo)e.AddedItems[0];

            using (Stream stream = File.OpenRead(fileInfo.FullName))
            {
                await Navigation.PushAsync(new PdfViewer(stream));
            }
        }
        
    }
}