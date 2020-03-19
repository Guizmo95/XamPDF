using Android.Arch.Lifecycle;
using Android.Widget;
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
    public partial class AddSummary : ContentPage
    {
        List<SummaryModel> summaries = new List<SummaryModel>();
        readonly FileInfo fileInfo;
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
        public AddSummary(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;
            BindingContext = viewModel = new ItemsViewModel(fileInfo);
        }

        //TODO -- HANDLE MAX LENGTH
        //TODO FINISH ADD SUMMARY PROCCESS
        private async void CollectionViewThumbnails_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            if(CollectionViewThumbnails.SelectedItem == null)
            {
                return;
            }
            else
            {
                ThumbnailsModel item = (ThumbnailsModel)CollectionViewThumbnails.SelectedItem;

                await Navigation.PushModalAsync(new AddSummaryModal(summaries, item));

                //string title = await DisplayPromptAsync("Title", "Select a title for this page", initialValue: string.Empty);
                //int pageNumber = item.PageNumber;

                //if (string.IsNullOrEmpty(title) == false)
                //{
                //    summaries.Add(new SummaryModel(title, pageNumber));

                //}

                //CollectionViewThumbnails.SelectedItem = null;
            }
        }

        //private async void StartProccessAddSummary(object sender, EventArgs e)
        //{ 
        //    if(summaries.Count == 0)
        //    {
        //        DependencyService.Get<IToast>().ShortAlert("No summary added");
        //    }
        //    else
        //    {

        //    }

        //    string fileNameGenerated = await fileEndpoint.UploadFilesForConcate(fileInfo, pagesNumbers);

        //    await Navigation.PushAsync(new GetDownload(fileNameGenerated));
        //}


    }
}