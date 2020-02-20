using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Graphics.Pdf.PdfRenderer;

namespace Pdf.Droid
{
    public class GetThumbnails
    {
        public void GetBitmaps(string filePath) {

            PdfRenderer pdfRenderer = new PdfRenderer(GetSeekableFileDescriptor(filePath));

            int pageCount = pdfRenderer.PageCount;

            for (int i = 0; i<pageCount; i++)
            {
                Page page = pdfRenderer.OpenPage(i);
                Bitmap bmp = Bitmap.CreateBitmap(page.Width, page.Height, Bitmap.Config.Argb8888);
                page.Render(bmp, null, null, PdfRenderMode.ForDisplay);
                
            }
        }

        private ParcelFileDescriptor GetSeekableFileDescriptor(string filePath)
        {
            ParcelFileDescriptor fileDescriptor = null;

            try
            {
                fileDescriptor = ParcelFileDescriptor.Open(new Java.IO.File(filePath), ParcelFileMode.ReadOnly);
            }
            catch (Exception ex)
            {
                //TODO Handle exeption
                throw;
            }

            return fileDescriptor;
        }
    }
}