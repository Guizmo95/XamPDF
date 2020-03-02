using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics.Pdf;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Pdf.Droid;
using Pdf.Droid.Helpers;
using Pdf.Interfaces;
using static Android.Graphics.Pdf.PdfRenderer;

[assembly:Xamarin.Forms.Dependency(typeof(GetThumbnails))]
namespace Pdf.Droid.Helpers
{
    public class GetThumbnails : IGetThumbnails
    {
        public string GetBitmaps(string filePath) {


            PdfRenderer pdfRenderer = new PdfRenderer(GetSeekableFileDescriptor(filePath));

            var appDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string directoryPath = System.IO.Path.Combine(appDirectory, "thumbnailsTemp", System.IO.Path.GetFileNameWithoutExtension(fileName));

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);

                int pageCount = pdfRenderer.PageCount;

                for (int i = 0; i < pageCount; i++)
                {
                    Page page = pdfRenderer.OpenPage(i);
                    Android.Graphics.Bitmap bmp = Android.Graphics.Bitmap.CreateBitmap(page.Width, page.Height, Android.Graphics.Bitmap.Config.Argb8888);
                    page.Render(bmp, null, null, PdfRenderMode.ForDisplay);

                    try
                    {
                        using (FileStream output = new FileStream(System.IO.Path.Combine(directoryPath, fileName + "Thumbnails" + i + ".png"), FileMode.Create))
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

            return directoryPath;
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

        ParcelFileDescriptor IGetThumbnails.GetSeekableFileDescriptor(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}