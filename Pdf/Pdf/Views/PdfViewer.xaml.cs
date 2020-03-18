using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfViewer : ContentPage
    {
        Stream fileStream;
        public PdfViewer(Stream fileStream)
        {
            InitializeComponent();
            this.fileStream = fileStream;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Load the PDF
            pdfViewerControl.LoadDocument(fileStream);
        }
    }
}