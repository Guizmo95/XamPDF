using Pdf.Enumerations;
using Pdf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ToolsList : ContentPage
    {
        private List<ItemChoiceCustom> Items { get; set; }

        private int selectedItemId;

        public ToolsList()
        {
            InitializeComponent();

            Items = new List<ItemChoiceCustom>();
            Items.Add(new ItemChoiceCustom() { Id = 0, Libelle = "Concaténer de document PDF", Detail = "Permet de Fusionner de fichiers PDF en un fichier PDF" });
            Items.Add(new ItemChoiceCustom() { Id = 1, Libelle = "Concaténer des pages du PDF", Detail = "Permet de concaténer des pages du PDF choisi" });
            Items.Add(new ItemChoiceCustom() { Id = 2, Libelle = "Déconcatener des pages du PDF", Detail = "Permet de déconcatener toutes les pages du PDF choisi" });
            Items.Add(new ItemChoiceCustom() { Id = 3, Libelle = "Remove Pages from Pdf files", Detail = "Permet de supprimer des pages d'un PDF" });
            Items.Add(new ItemChoiceCustom() { Id = 4, Libelle = "Add a Watermark", Detail = "Add a watermark for a documents" });
            Items.Add(new ItemChoiceCustom() { Id = 5, Libelle = "Add stamp", Detail = "Add a stamp for a documents" });
            Items.Add(new ItemChoiceCustom() { Id = 6, Libelle = "Add summary", Detail = "Add a summary to your pdf doc" });
            Items.Add(new ItemChoiceCustom() { Id = 7, Libelle = "Uncompress", Detail = "Uncompress your pdf doc" });
            Items.Add(new ItemChoiceCustom() { Id = 8, Libelle = "Waiting", Detail = "waiting" });
            Items.Add(new ItemChoiceCustom() { Id = 9, Libelle = "Set a password", Detail = "Set a password to your doc" });

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
                    await Navigation.PushAsync(new ConcatePdfDocs());
                    break;
                case 1:
                    await Navigation.PushAsync(new FilesListForConvert(ProcessNames.ConcatePages));
                    break;
                case 2:
                    await Navigation.PushAsync(new FilesListForConvert(ProcessNames.DeconcatePages));
                    break;
                case 3:
                    await Navigation.PushAsync(new FilesListForConvert(ProcessNames.RemovePages));
                    break;
                case 4:
                    await Navigation.PushAsync(new FilesListForConvert(ProcessNames.AddWatermark));
                    break;
                case 5:
                    await Navigation.PushAsync(new FilesListForConvert(ProcessNames.AddStamp));
                    break;
                case 6:
                    await Navigation.PushAsync(new FilesListForConvert(ProcessNames.AddSummary));
                    break;
                case 7:
                    await Navigation.PushAsync(new UncompressDocs());
                    break;
                case 8:
                    break;
                case 9:
                    await Navigation.PushAsync(new FilesListForConvert(ProcessNames.SetPassword));
                    break;
                default:
                    break;
            }

        }
    }
}