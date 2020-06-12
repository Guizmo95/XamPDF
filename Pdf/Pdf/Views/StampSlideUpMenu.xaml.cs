using Pdf.Models;
using SlideOverKit;
using Syncfusion.ListView.XForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StampSlideUpMenu : SlideMenuView
    {
        public IList Images { get; set; }

        public SfListView StampListView
        {
            get
            {
                return stampListView;
            }
            private set
            {
                stampListView = value;
            }
        }

        public StampSlideUpMenu()
        {
            InitializeComponent();

            // You must set HeightRequest in this case
            this.HeightRequest = 120;
            // You must set IsFullScreen in this case, 
            // otherwise you need to set WidthRequest, 
            // just like the QuickInnerMenu sample
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

            Images = new List<StampModel>();

            Images.Add(new StampModel()
            {
                Image = ImageSource.FromResource("Pdf.Images.stamp.png"),
                Rotation = 44.75,
            });

            Images.Add(new StampModel()
            {
                Image = ImageSource.FromResource("Pdf.Images.do_not_copy.png"),
                Rotation = 45.3,
            });

            Images.Add(new StampModel()
            {
                Image = ImageSource.FromResource("Pdf.Images.draft.png"),
                Rotation = 44.7,
            });

            Images.Add(new StampModel()
            {
                Image = ImageSource.FromResource("Pdf.Images.payment.png"),
                Rotation = 45.4,
            });

            Images.Add(new StampModel()
            {
                Image = ImageSource.FromResource("Pdf.Images.received.png"),
                Rotation = 45.6,
            });

            Images.Add(new StampModel()
            {
                Image = ImageSource.FromResource("Pdf.Images.sent.png"),
                Rotation = 45.1,
            });

            stampListView.ItemsSource = Images;
        }
    }
}