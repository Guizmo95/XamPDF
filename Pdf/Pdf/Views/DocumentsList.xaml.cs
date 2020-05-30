using Pdf.Helpers;
using Pdf.Models;
using Syncfusion.DataSource;
using Syncfusion.XForms.PopupLayout;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentsList : ContentPage, INotifyPropertyChanged
    {
        private SfPopupLayout popupLayout;
        private FileModel selectedDoc;
        public SfBehavior SfBehavior { get; set; }

        public FileModel SelectedDoc
        {
            get
            {
                return selectedDoc;
            }

            set
            {
                selectedDoc = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DocumentsList()
        {
            InitializeComponent();

            SfBehavior = new SfBehavior();
            popupLayout = new SfPopupLayout();
            popupLayout.PopupView.HeightRequest = 104;
            popupLayout.PopupView.WidthRequest = 93;
            popupLayout.PopupView.ShowHeader = false;
            popupLayout.PopupView.ShowFooter = false;
            popupLayout.PopupView.ShowCloseButton = false;
            popupLayout.PopupView.AnimationMode = AnimationMode.Fade;
            popupLayout.PopupView.AnimationEasing = AnimationEasing.SinOut;
            popupLayout.PopupView.AnimationDuration = 150;

            DataTemplate templateView = new DataTemplate(() =>
            {
                StackLayout stackLayout = new StackLayout();
                stackLayout.Orientation = StackOrientation.Vertical;
                stackLayout.VerticalOptions = LayoutOptions.Center;
                stackLayout.Spacing = 0;

                Button sortName;
                sortName = new Button();
                sortName.HeightRequest = 34;
                sortName.WidthRequest = 93;
                sortName.Text = "Sort name";
                sortName.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                sortName.BackgroundColor = Color.White;
                sortName.FontSize = 8.8;
                sortName.Clicked += SortName_Clicked;

                Button sortDate;
                sortDate = new Button();
                sortDate.HeightRequest = 34;
                sortDate.WidthRequest = 93;
                sortDate.Text = "Sort date";
                sortDate.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                sortDate.BackgroundColor = Color.White;
                sortDate.FontSize = 8.8;
                sortDate.Clicked += SortDate_Clicked;

                Button sortSize;
                sortSize = new Button();
                sortSize.HeightRequest = 34;
                sortSize.WidthRequest = 93;
                sortSize.Text = "Sort size";
                sortSize.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                sortSize.BackgroundColor = Color.White;
                sortSize.FontSize = 8.8;
                sortSize.Clicked += SortSize_Clicked;

                stackLayout.Children.Add(sortName);
                stackLayout.Children.Add(sortDate);
                stackLayout.Children.Add(sortSize);

                return stackLayout;
            });

            popupLayout.PopupView.ContentTemplate = templateView;

            behavior.BindingContext = SfBehavior;

            SortName_Clicked(null, null);

            sortButton.RotateTo(180);

            MessagingCenter.Subscribe<DocumentListViewBehavior>(this, "PushAsyncPage", (sender) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    activityIndicator.IsRunning = true;
                    filter.IsVisible = true;

                    try
                    {
                        var file = (FileModel)DocumentListView.SelectedItem;

                        if(file != null)
                        {
                            await Navigation.PushAsync(new PdfViewer(file.FilePath));
                        }
                    }

                    finally
                    {
                        DocumentListView.SelectedItem = null;
                        activityIndicator.IsRunning = false;
                        filter.IsVisible = false;
                    }
                });
            });
        }

        private void SortDate_Clicked(object sender, EventArgs e)
        {
            DocumentListView.DataSource.SortDescriptors.Clear();
            DocumentListView.DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = "creationtime",
                Direction = Syncfusion.DataSource.ListSortDirection.Ascending,
            });
            DocumentListView.RefreshView();
        }

        private void SortName_Clicked(object sender, EventArgs e)
        {
            DocumentListView.DataSource.SortDescriptors.Clear();
            DocumentListView.DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = "filename",
                Direction = Syncfusion.DataSource.ListSortDirection.Ascending,
            });
            DocumentListView.RefreshView();
        }

        private void SortSize_Clicked(object sender, EventArgs e)
        {
            DocumentListView.DataSource.SortDescriptors.Clear();
            DocumentListView.DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = "FileLenght",
                Direction = Syncfusion.DataSource.ListSortDirection.Ascending,
            });
            DocumentListView.RefreshView();
        }

        private void SortButton_Clicked(object sender, EventArgs e)
        {
            // Shows SfPopupLayout at the top of the button.
            popupLayout.ShowRelativeToView(sortButton, RelativePosition.AlignTopRight, 0, 0);
        }

        private void SearchButton_Clicked(object sender, EventArgs e)
        {
            headerLabel.IsVisible = false;
            searchButton.IsVisible = false;
            sortButton.IsVisible = false;

            sortButton.HorizontalOptions = LayoutOptions.EndAndExpand;

            filterDocument.IsVisible = true;
            clearSearchBar.IsVisible = true;
        }

        private void ClearSearchBar_Clicked(object sender, EventArgs e)
        {
            headerLabel.IsVisible = true;
            searchButton.IsVisible = true;
            sortButton.IsVisible = true;

            sortButton.HorizontalOptions = LayoutOptions.End;

            filterDocument.IsVisible = false;
            clearSearchBar.IsVisible = false;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}