using Pdf.Helpers;
using Pdf.Models;
using Pdf.ViewModels;
using Syncfusion.DataSource;
using Syncfusion.XForms.Backdrop;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private SfPopupLayout popupLayout;

        public SfBehavior SfBehavior { get; set; }

        public MainPage()
        {
            InitializeComponent();



            SfBehavior = new SfBehavior();
            popupLayout = new SfPopupLayout();
            popupLayout.PopupView.HeightRequest = 92;
            popupLayout.PopupView.WidthRequest = 80;
            popupLayout.PopupView.ShowHeader = false;
            popupLayout.PopupView.ShowFooter = false;
            popupLayout.PopupView.ShowCloseButton = false;
            popupLayout.PopupView.AnimationMode = AnimationMode.SlideOnRight;
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
                sortName.HeightRequest = 30;
                sortName.WidthRequest = 80;
                sortName.Text = "Sort name";
                sortName.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                sortName.BackgroundColor = Color.White;
                sortName.FontSize = 8;
                sortName.Clicked += SortName_Clicked;

                Button sortDate;
                sortDate = new Button();
                sortDate.HeightRequest = 30;
                sortDate.WidthRequest = 80;
                sortDate.Text = "Sort date";
                sortDate.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                sortDate.BackgroundColor = Color.White;
                sortDate.FontSize = 8;
                sortDate.Clicked += SortDate_Clicked;

                Button sortSize;
                sortSize = new Button();
                sortSize.HeightRequest = 30;
                sortSize.WidthRequest = 80;
                sortSize.Text = "Sort size";
                sortSize.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                sortSize.BackgroundColor = Color.White;
                sortSize.FontSize = 8;
                sortSize.Clicked += SortSize_Clicked;

                stackLayout.Children.Add(sortName);
                stackLayout.Children.Add(sortDate);
                stackLayout.Children.Add(sortSize);

                return stackLayout;
            });

            popupLayout.PopupView.ContentTemplate = templateView;

            behavior.BindingContext = SfBehavior;

            IList<ToolsCustomItem> list = new List<ToolsCustomItem>();
            list.Add(new ToolsCustomItem(0, "baseline_picture_as_pdf_24_drawerMenu.xml", "My documents"));
            list.Add(new ToolsCustomItem(1, "baseline_stars_24.xml", "Rate my App"));
            list.Add(new ToolsCustomItem(2, "baseline_visibility_24.xml", "Favorites"));
            list.Add(new ToolsCustomItem(3, "baseline_feedback_24.xml", "Feedback"));
            list.Add(new ToolsCustomItem(4, "baseline_account_circle_24.xml", "About"));
            ToolsListView.ItemsSource = list;

            sortButton.RotateTo(180);
        }

        private void SortDate_Clicked(object sender, EventArgs e)
        {
            DocumentListView.DataSource.SortDescriptors.Clear();
            DocumentListView.DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = "CreationTime",
                Direction = ListSortDirection.Ascending,
            });
            DocumentListView.RefreshView();
        }

        private void SortName_Clicked(object sender, EventArgs e)
        {
            DocumentListView.DataSource.SortDescriptors.Clear();
            DocumentListView.DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = "FileName",
                Direction = ListSortDirection.Ascending,
            });
            DocumentListView.RefreshView();
        }

        private void SortSize_Clicked(object sender, EventArgs e)
        {
            DocumentListView.DataSource.SortDescriptors.Clear();
            DocumentListView.DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = "FileLenght",
                Direction = ListSortDirection.Ascending,
            });
            DocumentListView.RefreshView();
        }

        void HamburgerButton_Clicked(object sender, EventArgs e)
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

        private void SortButton_Clicked(object sender, EventArgs e)
        {
            popupLayout.Show(sortButton.X, sortButton.Y);
        }
    }
}