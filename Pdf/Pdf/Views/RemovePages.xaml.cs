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
    public partial class RemovePages : ContentPage
    {
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

        public RemovePages(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;
            BindingContext = viewModel = new ItemsViewModel(fileInfo);

        }

        private async void StartProcessRemovePages(object sender, EventArgs e)
        {
            List<ThumbnailsModel> itemsSelected = CollectionViewThumbnails.SelectedItems.Cast<ThumbnailsModel>().ToList();
            List<ThumbnailsModel> allItems = CollectionViewThumbnails.ItemsSource.Cast<ThumbnailsModel>().ToList();

            if (itemsSelected == null)
            {
                return;
            }

            List<int> pagesNumbers = new List<int>();


            allItems.ForEach(delegate (ThumbnailsModel thumbnailsModel)
            {
                if(itemsSelected.Contains(thumbnailsModel) == false)
                {
                    pagesNumbers.Add(thumbnailsModel.PageNumber);
                }
            });

            string fileName= await fileEndpoint.UploadFilesForRemovePages(fileInfo, pagesNumbers);

            await Navigation.PushAsync(new GetDownload(fileName));
        }
    }
}