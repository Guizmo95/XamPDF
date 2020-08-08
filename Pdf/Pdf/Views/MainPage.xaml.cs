using Acr.UserDialogs;
using Pdf.Helpers;
using Pdf.Models;
using Pdf.ViewModels;
using Syncfusion.DataSource;
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

        public SfBehaviorDocumentsPage SfBehaviorDocumentsPage { get; set; }

        public MainPage()
        {
            InitializeComponent();

            //    SfBehaviorDocumentsPage = new SfBehaviorDocumentsPage();
            //    popupLayout = new SfPopupLayout();
            //    popupLayout.PopupView.HeightRequest = 92;
            //    popupLayout.PopupView.WidthRequest = 80;
            //    popupLayout.PopupView.ShowHeader = false;
            //    popupLayout.PopupView.ShowFooter = false;
            //    popupLayout.PopupView.ShowCloseButton = false;
            //    popupLayout.PopupView.AnimationMode = AnimationMode.SlideOnRight;
            //    popupLayout.PopupView.AnimationEasing = AnimationEasing.SinOut;
            //    popupLayout.PopupView.AnimationDuration = 150;

            //    DataTemplate templateView = new DataTemplate(() =>
            //    {
            //        StackLayout stackLayout = new StackLayout();
            //        stackLayout.Orientation = StackOrientation.Vertical;
            //        stackLayout.VerticalOptions = LayoutOptions.Center;
            //        stackLayout.Spacing = 0;

            //        Button sortName;
            //        sortName = new Button();
            //        sortName.HeightRequest = 30g;
            //        sortName.WidthRequest = 80;
            //        sortName.Text = "Sort name";
            //        sortName.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
            //        sortName.BackgroundColor = Color.White;
            //        sortName.FontSize = 8;
            //        sortName.Clicked += SortName_Clicked;

            //        Button sortDate;
            //        sortDate = new Button();
            //        sortDate.HeightRequest = 30;
            //        sortDate.WidthRequest = 80;
            //        sortDate.Text = "Sort date";
            //        sortDate.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
            //        sortDate.BackgroundColor = Color.White;
            //        sortDate.FontSize = 8;
            //        sortDate.Clicked += SortDate_Clicked;

            //        Button sortSize;
            //        sortSize = new Button();
            //        sortSize.HeightRequest = 30;
            //        sortSize.WidthRequest = 80;
            //        sortSize.Text = "Sort size";
            //        sortSize.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
            //        sortSize.BackgroundColor = Color.White;
            //        sortSize.FontSize = 8;
            //        sortSize.Clicked += SortSize_Clicked;

            //        stackLayout.Children.Add(sortName);
            //        stackLayout.Children.Add(sortDate);
            //        stackLayout.Children.Add(sortSize);

            //        return stackLayout;
            //    });

            //    popupLayout.PopupView.ContentTemplate = templateView;

            //    behavior.BindingContext = SfBehaviorDocumentsPage;

            //    IList<ToolsCustomItem> list = new List<ToolsCustomItem>();
            //    list.Add(new ToolsCustomItem(0, "baseline_picture_as_pdf_24_drawerMenu.xml", "My documents"));
            //    list.Add(new ToolsCustomItem(1, "baseline_stars_24.xml", "Rate my App"));
            //    list.Add(new ToolsCustomItem(2, "baseline_visibility_24.xml", "Favorites"));
            //    list.Add(new ToolsCustomItem(3, "baseline_feedback_24.xml", "Feedback"));
            //    list.Add(new ToolsCustomItem(4, "baseline_account_circle_24.xml", "About"));
            //    ToolsListView.ItemsSource = list;

            //    SortName_Clicked(null, null);

            //    sortButton.RotateTo(180);
            //}

            //private void sortdate_clicked(object sender, eventargs e)
            //{
            //    documentlistview.datasource.sortdescriptors.clear();
            //    documentlistview.datasource.sortdescriptors.add(new sortdescriptor()
            //    {
            //        propertyname = "creationtime",
            //        direction = listsortdirection.ascending,
            //    });
            //    documentlistview.refreshview();
            //}

            //private void sortname_clicked(object sender, eventargs e)
            //{
            //    documentlistview.datasource.sortdescriptors.clear();
            //    documentlistview.datasource.sortdescriptors.add(new sortdescriptor()
            //    {
            //        propertyname = "filename",
            //        direction = listsortdirection.ascending,
            //    });
            //    documentlistview.refreshview();
            //}

            //private void sortsize_clicked(object sender, eventargs e)
            //{
            //    documentlistview.datasource.sortdescriptors.clear();
            //    documentlistview.datasource.sortdescriptors.add(new sortdescriptor()
            //    {
            //        propertyname = "filelenght",
            //        direction = listsortdirection.ascending,
            //    });
            //    documentlistview.refreshview();
            //}

            //void hamburgerbutton_clicked(object sender, eventargs e)
            //{
            //    navigationdrawer.toggledrawer();
            //}

            //private async void documentlistview_selectionchanging(object sender, syncfusion.listview.xforms.itemselectionchangingeventargs e)
            //{
            //    if (e.addeditems == null)
            //    {
            //        return;
            //    }

            //    var file = (filemodel)e.addeditems[0];

            //    using (stream stream = file.openread(file.filepath))
            //    {
            //        await navigation.pushasync(new pdfviewer(stream));
            //    }
            //}

            //private void sortbutton_clicked(object sender, eventargs e)
            //{
            //    popuplayout.show(sortbutton.x, sortbutton.y);
            //}

            //private void searchbutton_clicked(object sender, eventargs e)
            //{
            //    headerlabel.isvisible = false;
            //    hamburgerbutton.isvisible = false;
            //    searchbutton.isvisible = false;

            //    sortbutton.horizontaloptions = layoutoptions.endandexpand;

            //    filterdocument.isvisible = true;
            //    clearsearchbar.isvisible = true;
            //}

            //private void clearsearchbar_clicked(object sender, eventargs e)
            //{
            //    headerlabel.isvisible = true;
            //    hamburgerbutton.isvisible = true;
            //    searchbutton.isvisible = true;

            //    sortbutton.horizontaloptions = layoutoptions.end;

            //    filterdocument.isvisible = false;
            //    clearsearchbar.isvisible = false;
            //}
            }
        }
    }