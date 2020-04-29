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


        public Page1(string filePath)
        {
            
            InitializeComponent();

            this.filePath = filePath;


            ////Load the PDF document
            //PdfLoadedDocument loadedDocument = new PdfLoadedDocument(stream);

            ////Disable the incremental update
            //loadedDocument.FileStructure.IncrementalUpdate = false;

            ////Set the compression level
            //loadedDocument.Compression = PdfCompressionLevel.Best;

            ////Save and close the document

            //var download = DependencyService.Get<IAndroidFileHelper>().GetDownloadPath();

            //loadedDocument.Save(File.Create(Path.Combine(download, "test2.pdf")));

            //loadedDocument.Close(true);

            pdfViewerControl.CustomPdfRenderer = DependencyService.Get<ICustomPdfRendererService>().AlternatePdfRenderer;
            this.BindingContext = new PdfViewerModel(filePath);
        }

    }
}