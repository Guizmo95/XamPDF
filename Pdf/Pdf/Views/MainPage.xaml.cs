using Pdf.Helpers;
using Pdf.Models;
using Pdf.ViewModels;
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
            popupLayout.PopupView.HeightRequest = 200;
            popupLayout.PopupView.WidthRequest = 150;
            popupLayout.PopupView.AcceptButtonText = "Sort";

            DataTemplate headerTemplateView = new DataTemplate(() =>
            {
                Label headerContent;

                headerContent = new Label();
                headerContent.Text = "Sort files";
                headerContent.TextColor = Color.FromHex("#e6e6e6");
                headerContent.FontFamily = "GothamBold.ttf#GothamBold";
                headerContent.FontSize = 14;
                headerContent.HorizontalTextAlignment = TextAlignment.Center;
                headerContent.VerticalTextAlignment = TextAlignment.Center;
                return headerContent;
            });

            DataTemplate templateView = new DataTemplate(() =>
            {
                StackLayout stackLayout = new StackLayout();
                stackLayout.Orientation = StackOrientation.Vertical;


                Label sortName;
                sortName = new Label();
                sortName.Text = "Sort name";
                sortName.VerticalTextAlignment = TextAlignment.Center;

                Label sortDate;
                sortDate = new Label();
                sortDate.Text = "Sort name";
                sortDate.VerticalTextAlignment = TextAlignment.Center;
                
                









            });

            popupLayout.PopupView.ShowCloseButton = false;
            popupLayout.PopupView.HeaderTemplate = headerTemplateView;

            Main.BindingContext = this;

            IList<ToolsCustomItem> list = new List<ToolsCustomItem>();
            list.Add(new ToolsCustomItem(0, "baseline_picture_as_pdf_24_drawerMenu.xml", "My documents"));
            list.Add(new ToolsCustomItem(1, "baseline_stars_24.xml", "Rate my App"));
            list.Add(new ToolsCustomItem(2, "baseline_visibility_24.xml", "Recent"));
            list.Add(new ToolsCustomItem(3, "baseline_feedback_24.xml", "Feedback"));
            list.Add(new ToolsCustomItem(4, "baseline_account_circle_24.xml", "About"));
            ToolsListView.ItemsSource = list;

            sortButton.RotateTo(180);
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