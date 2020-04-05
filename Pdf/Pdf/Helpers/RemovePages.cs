using Pdf.Interfaces;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdf.Helpers
{
    public class RemovePages : IRemovePages
    {
        public async Task RemovePageInAnExistingPdf(FileInfo fileInfo, List<int> pagesNumbers)
        {
            Stream docStream = new FileStream(fileInfo.FullName, FileMode.Open);

            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);

            for (int i = 0; i < pagesNumbers.Count; i++)
            {
                //Imports the pages from the loaded document

                loadedDocument.Pages.RemoveAt(pagesNumbers[i] - 1);

                //Save the PDF document to stream

                MemoryStream stream = new MemoryStream();

                loadedDocument.Save(stream);

                //Close the document

                loadedDocument.Close(true);

                string outputFileName = System.IO.Path.GetRandomFileName() + ".pdf";

                await Xamarin.Forms.DependencyService.Get<IAndroidFileHelper>().SaveAndView(outputFileName, "application/pdf", stream);
            }
        }
    }
}
