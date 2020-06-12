using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThumbnailsPage : ContentPage
    {

        public SfListView ListView
        {
            get
            {
                return ListView;
            }
        }

        public ThumbnailsPage()
        {
            InitializeComponent();
        }
    }
}