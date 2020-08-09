using System.Collections;
using System.Collections.Generic;
using Pdf.Models;
using SlideOverKit;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StampSlideUpMenu : SlideMenuView
    {
        public IList Images { get; set; }

        public SfListView StampListView
        {
            get => StampSfListView;
            set => StampSfListView = value;
        }

        public StampSlideUpMenu()
        {
            InitializeComponent();

            this.HeightRequest = 200;
            this.IsFullScreen = true;
            this.MenuOrientations = MenuOrientation.BottomToTop;

            // You must set BackgroundColor, 
            // and you cannot put another layout with background color cover the whole View
            // otherwise, it cannot be dragged on Android
            this.BackgroundColor = Color.FromHex("fafafa");
            this.BackgroundViewColor = Color.Transparent;

            // In some small screen size devices, the menu cannot be full size layout.
            // In this case we need to set different size for Android.
            if (Device.RuntimePlatform == Device.Android)
                this.HeightRequest += 50;

            Images = new List<StampModel>
            {
                new StampModel() {Image = ImageSource.FromResource("Pdf.Images.approved_green.png"),},
                new StampModel() {Image = ImageSource.FromResource("Pdf.Images.approved_red.png"),},
                new StampModel() {Image = ImageSource.FromResource("Pdf.Images.completed_green.png"),},
                new StampModel() {Image = ImageSource.FromResource("Pdf.Images.completed.png"),},
                new StampModel() {Image = ImageSource.FromResource("Pdf.Images.draft_green.png"),},
                new StampModel() {Image = ImageSource.FromResource("Pdf.Images.draft_red.png"),},
                new StampModel() {Image = ImageSource.FromResource("Pdf.Images.expired_green.png"),},
                new StampModel() {Image = ImageSource.FromResource("Pdf.Images.expired_red.png"),},
                new StampModel() {Image = ImageSource.FromResource("Pdf.Images.rejected_green.png"),},
                new StampModel() {Image = ImageSource.FromResource("Pdf.Images.rejected_red.png"),}
            };

            StampSfListView.ItemsSource = Images;
        }
    }
}