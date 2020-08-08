using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfPropertyPopup : ContentView
    {
        public SfListView PdfPropertyListView
        {
            get => PListView;
            protected set => PListView = value;
        }

        public PdfPropertyPopup()
        {
            InitializeComponent();
        }
    }
}