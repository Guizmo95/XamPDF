using Pdf.Interfaces;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdf.Helpers
{
    public class SplitPdf :ISplitPdf
    {
        public async Task SplitPdfPagesInAnExistingDocument(FileInfo fileInfo, List<int> pagesNumbers)
        {
            //Load the file as stream

            Stream docStream = new FileStream(fileInfo.FullName, FileMode.Open);

            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);

            for (int i = 0; i < pagesNumbers.Count; i++)
            {
                //Creates a new document

                PdfDocument document = new PdfDocument();

                //Imports the pages from the loaded document

                document.ImportPage(loadedDocument, pagesNumbers[i] - 1);

                //Save the PDF document to stream

                MemoryStream stream = new MemoryStream();

                document.Save(stream);

                //Close the document

                document.Close(true);

                string outputFileName = System.IO.Path.GetRandomFileName() + ".pdf";

                //await Xamarin.Forms.DependencyService.Get<IAndroidFileHelper>().SaveAndView(outputFileName, "application/pdf", stream);
            }
        }
    }
}
