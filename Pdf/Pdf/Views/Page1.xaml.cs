using Pdf.Interfaces;
using Pdf.ViewModels;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.SfPdfViewer.XForms;
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
    public partial class Page1 : ContentPage
    {
        private string filePath;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Task.Run(() =>
            {
                var pdfStream = DependencyService.Get<IAndroidFileHelper>().GetFileStream(filePath);
                pdfViewerControl.CustomPdfRenderer = DependencyService.Get<ICustomPdfRendererService>().AlternatePdfRenderer;

                Device.BeginInvokeOnMainThread(() =>
                {
                    pdfViewerControl.LoadDocument(pdfStream);
                });
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }

        public Page1(string filePath)
        {
            
            InitializeComponent();

            this.filePath = filePath;
            pdfViewerControl.Toolbar.Enabled = true;

            ////Load the PDF document
            //PdfLoadedDocument loadedDocument = new PdfLoadedDocument(stream);

            ////Disable the incremental update
            //loadedDocument.FileStructure.IncrementalUpdate = false;

            ////Set the compression level
            //loadedDocument.Compression = PdfCompressionLevel.Best;

            ////SaveAndReturnStatus and close the document

            //var download = DependencyService.Get<IAndroidFileHelper>().GetDownloadPath();

            //loadedDocument.SaveAndReturnStatus(File.Create(Path.Combine(download, "test2.pdf")));

            //loadedDocument.Close(true);

            
        }

    }
}