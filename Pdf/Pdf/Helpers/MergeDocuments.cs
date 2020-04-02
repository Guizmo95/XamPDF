using Pdf.Interfaces;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdf.Helpers
{
    public class MergeDocuments
    {
        //Merge PDF documents selected by users
        public async Task Merge(List<FileInfo> fileInfos)
        {
            //Create Pdf Streams
            Stream[] source = new Stream[fileInfos.Count];

            int i = 0;
            fileInfos.ForEach(delegate (FileInfo fileInfo)
            {
                source[i] = new FileStream(fileInfo.FullName, FileMode.Open);
            });

            //Create a new PDF document
            PdfDocument document = new PdfDocument();

            PdfMergeOptions mergeOptions = new PdfMergeOptions();
            //Enable Optimize Resources
            mergeOptions.OptimizeResources = true;

            //Merge the documents

            PdfDocumentBase.Merge(document, mergeOptions, source);

            //Save the PDF document to stream

            MemoryStream stream = new MemoryStream();

            document.Save(stream);

            //Close the documents

            document.Close(true);

            await Xamarin.Forms.DependencyService.Get<IAndroidFileHelper>().SaveAndView("Sample.pdf", "application/pdf", stream);
        }
    }
}
