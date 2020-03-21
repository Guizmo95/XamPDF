using Acr.UserDialogs;
using Pdf.Interfaces;
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
    public partial class DeconcatePageThumbnails : ContentPage
    {
        //TODO - REDIMENSIONNEMENT DYNAMIQUE
        private readonly FileInfo fileInfo;
        FileEndpoint fileEndpoint = new FileEndpoint();
        ItemsViewModel viewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);

            MessagingCenter.Subscribe<object, ThumbnailsModel>(this, ItemsViewModel.ScrollToPreviousLastItem, (sender, item) =>
            {
                CollectionViewThumbnails.ScrollTo(item, ScrollToPosition.Start);
            });
        }

        public DeconcatePageThumbnails(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;
            BindingContext = viewModel = new ItemsViewModel(fileInfo);
        }

        private async void StartProcessDeconcatePages(object sender, EventArgs e)
        {
            List<ThumbnailsModel> items = CollectionViewThumbnails.SelectedItems.Cast<ThumbnailsModel>().ToList();

            if (items == null)
            {
                return;
            }

            List<int> pagesNumbers = new List<int>();

            items.ForEach(delegate (ThumbnailsModel thumbnailsModel)
            {
                pagesNumbers.Add(thumbnailsModel.PageNumber);
            });

            List<string> filesNamesGenerated = await fileEndpoint.UploadFilesForDeconcate(fileInfo, pagesNumbers);

            await Navigation.PushAsync(new GetDownload(filesNamesGenerated));
        }

        private void SelectAllItems(object sender, EventArgs e)
        {
            CollectionViewThumbnails.SelectedItems = CollectionViewThumbnails.ItemsSource.Cast<object>().ToList();
        }
    }
}