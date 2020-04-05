using Pdf.Interfaces;
using Syncfusion.SfPdfViewer.XForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pdf.Helpers
{
    public static class PdfToImage
    {
        //Convert Pdf to Thumbnails
        public static MemoryStream FileToImage(FileInfo fileInfo, int index)
        {
            //Get the stream
            var fileStream = new FileStream(fileInfo.FullName, FileMode.Open);

            //Initialize the SfPdfViewer
            SfPdfViewer sfPdfViewer = new SfPdfViewer();

            sfPdfViewer.LoadDocument(fileStream);

            int pageCount = DependencyService.Get<IGetThumbnails>().GetAllPages(fileInfo.FullName);
            Stream stream = sfPdfViewer.ExportAsImage(index);

            return stream as MemoryStream;
        }
    }
}
