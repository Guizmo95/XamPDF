using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using Java.IO;
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
                Android.Graphics.Bitmap bmp = Android.Graphics.Bitmap.CreateBitmap(page.Width, page.Height, Android.Graphics.Bitmap.Config.Argb8888);
                page.Render(bmp, null, null, PdfRenderMode.ForDisplay);

                string fileName = System.IO.Path.GetFileName(filePath);

                var appDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                Directory.CreateDirectory(fileName);

                try
                {
                    using (FileStream output = new FileStream(appDirectory + "/" + fileName + "Thumbnails" + i, FileMode.OpenOrCreate))
                    {
                        bmp.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, output);
                    }

                    page.Close();
                }
                catch (Exception ex)
                {
                    //TODO -- GERER CETTE EXPEXPTION
                    throw new Exception();
                }

                
                
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