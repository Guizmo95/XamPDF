using Pdf.Models;
using Pdf.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace Pdf
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ToolsList : ContentPage
    {
        private FileInfo fileInfo;
        private List<ItemChoiceCustom> Items { get; set; }

        private int selectedItemId;

        public ToolsList(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;

            Items = new List<ItemChoiceCustom>();
            Items.Add(new ItemChoiceCustom() { Id = 0, Libelle = "Concaténer de document PDF", Detail = "Permet de Fusionner de fichiers PDF en un fichier PDF" });
            Items.Add(new ItemChoiceCustom() { Id = 1, Libelle = "Concaténer des pages du PDF", Detail = "Permet de concaténer des pages du PDF choisi" });

            MyListView.ItemsSource = Items;
        }

        private void SelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (ItemChoiceCustom)e.SelectedItem;
            this.selectedItemId = item.Id;
        }

        private async void StartProcessButton(object sender, EventArgs e)
        {
            
            if (MyListView.SelectedItem == null)
            {
                return;
            }

            switch (this.selectedItemId)
            {
                case 0:
                    await Navigation.PushAsync(new ConcatePdfPage(fileInfo));
                    break;
                case 1:
                    await Navigation.PushAsync(new ConcatePageThumbnails(fileInfo));
                    break;
                default:
                    break;
            }

        }
    }
}