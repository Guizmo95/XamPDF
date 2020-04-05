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
    public class MergePages : IMergePages
    {
        //Merge Pages into a PDF
        public async Task Merge(FileInfo fileInfo, List<int> pagesNumbers)
        {
            //Load the file as stream
            Stream stream = new FileStream(fileInfo.FullName, FileMode.Open);

            PdfLoadedDocument lDoc = new PdfLoadedDocument(stream);

            //Create a new document
            PdfDocument document = new PdfDocument();

            //ImportsPages

            pagesNumbers.ForEach(delegate (int pageNumber)
            {
                document.ImportPage(lDoc, pageNumber);
            });

            //Save The Pdf document to stream 

            MemoryStream ms = new MemoryStream();

            document.Save(ms);

            //Close the documents

            document.Close();

            lDoc.Close();

            string outputFileName = System.IO.Path.GetRandomFileName() + ".pdf";

            await Xamarin.Forms.DependencyService.Get<IAndroidFileHelper>().SaveAndView(outputFileName, "application/pdf", ms);
        } 
    }
}
