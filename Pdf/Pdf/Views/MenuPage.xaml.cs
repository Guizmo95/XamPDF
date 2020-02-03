﻿using Pdf.Models;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace Pdf
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        private FileData fileData;
        private List<ItemChoiceCustom> Items { get; set; }

        private int selectedItemId;

        public MenuPage(FileData fileData)
        {
            InitializeComponent();

            this.fileData = fileData;

            Items = new List<ItemChoiceCustom>();
            Items.Add(new ItemChoiceCustom() { Id = 0, Libelle = "Concaténer de document PDF", Detail = "Permet de Fusionner de fichiers PDF en un fichier PDF" });
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
                    await Navigation.PushAsync(new ConcatePdfPage(fileData));
                    break;
                default:
                    break;
            }

        }
    }
}