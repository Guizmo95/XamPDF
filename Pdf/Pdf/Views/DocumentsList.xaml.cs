using Acr.UserDialogs;
using Pdf.Helpers;
using Pdf.Interfaces;
using Pdf.Models;
using Syncfusion.DataSource;
using Syncfusion.XForms.PopupLayout;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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


        public DocumentsList()
        {
            InitializeComponent();

            SfBehavior = new SfBehavior();
            popupLayout = new SfPopupLayout();
            popupLayout.PopupView.HeightRequest = 124;
            popupLayout.PopupView.WidthRequest = 113;
            popupLayout.Padding = new Thickness(15, 10, 7, 15);
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
                sortName.HeightRequest = 41;
                sortName.WidthRequest = 113;
                sortName.Text = "Sort name";
                sortName.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                sortName.BackgroundColor = Color.White;
                sortName.FontSize = 11;
                sortName.Clicked += SortName_Clicked;

                Button sortDate;
                sortDate = new Button();
                sortDate.HeightRequest = 41;
                sortDate.WidthRequest = 113;
                sortDate.Text = "Sort date";
                sortDate.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                sortDate.BackgroundColor = Color.White;
                sortDate.FontSize = 11;
                sortDate.Clicked += SortDate_Clicked;

                Button sortSize;
                sortSize = new Button();
                sortSize.HeightRequest = 41;
                sortSize.WidthRequest = 113;
                sortSize.Text = "Sort size";
                sortSize.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                sortSize.BackgroundColor = Color.White;
                sortSize.FontSize = 11;
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

        }

        private void ResetSwipe()
        {
            
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

        private async void DocumentListView_SelectionChanged(object sender, Syncfusion.ListView.XForms.ItemSelectionChangedEventArgs e)
        {
            await Task.Delay(500);

            UserDialogs.Instance.ShowLoading("Loading ...", MaskType.Black);

            try
            {
                var file = (FileModel)DocumentListView.SelectedItem;

                if (file != null)
                {
                    await Navigation.PushAsync(new PdfViewer(file.FilePath));
                }
            }

            finally
            {
                DocumentListView.SelectedItem = null;

                UserDialogs.Instance.HideLoading();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

 
    }
}