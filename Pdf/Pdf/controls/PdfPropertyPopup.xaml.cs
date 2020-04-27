using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfPropertyPopup : ContentView
    {
        public SfListView PdfPropertyListView
        {
            get
            {
                return pdfPropertyListView;
            }
            protected set
            {
                pdfPropertyListView = value;
            }
        }


        public PdfPropertyPopup()
        {
            InitializeComponent();
        }
    }
}