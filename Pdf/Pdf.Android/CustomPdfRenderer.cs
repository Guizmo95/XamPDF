using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Shockwave.Pdfium;
using Pdf.Droid;
using Syncfusion.SfPdfViewer.XForms;
using Syncfusion.SfPdfViewer.XForms.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(CustomPdfRenderer))]
namespace Pdf.Droid
{
    internal class CustomPdfRenderer : ICustomPdfRenderer, ICustomPdfRendererService
    {
        internal PdfiumCore m_pdfiumCore;
        internal PdfDocument m_pdfDocument;
        internal int m_pageCount;
        internal Bitmap.Config m_bitmapConfig;

        /// <summary>
        /// Gets or sets the total page count of the PDF document
        /// </summary>
        public int PageCount
        {
            get
            {
                return m_pageCount;
            }

            set
            {
                m_pageCount = value;
            }
        }
        /// <summary>
        /// Gets and sets Bitmap.Config to render the bitmap from PDF document
        /// </summary>
        public Bitmap.Config BitmapConfig
        {
            get
            {
                return m_bitmapConfig;
            }

            set
            {
                m_bitmapConfig = value;
            }
        }

        /// <summary>
        /// Gets the dependency object of ICustomPdfRenderer 
        /// </summary>
        public object AlternatePdfRenderer
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Closes the initialized object to release memory
        /// </summary>
        public void Close()
        {
            //Close the created PdfDocument instance
            m_pdfiumCore.CloseDocument(m_pdfDocument);
            //Dispose PdfiumCore instance
            m_pdfiumCore.Dispose();
        }

        /// <summary>
        /// Get the Page size of the PDF document with given page index
        /// </summary>
        /// <param name="pageIndex">Page index to the get page size</param>
        /// <returns>Size of the page of PDF document </returns>
        public Android.Util.Size GetPageSize(int pageIndex)
        {
            if (m_pdfiumCore == null)
            {
                throw new System.NullReferenceException("object reference is not set to an instance: m_pdfiumCore");
            }
            else if (m_pdfDocument == null)
            {
                throw new System.NullReferenceException("object reference is not set to an instance: m_pdfDocument");
            }
            else if (pageIndex < 0 && pageIndex > m_pageCount - 1)
            {
                throw new System.ArgumentOutOfRangeException("pageIndex", "Index was out of range. Must be non-negative and less than the size of the PageCount.");
            }
            else
            {
                //open the PDF page with specified index to get the Size of a Page
                m_pdfiumCore.OpenPage(m_pdfDocument, pageIndex);
                int pageHeight = m_pdfiumCore.GetPageHeightPoint(m_pdfDocument, pageIndex);
                int pageWidth = m_pdfiumCore.GetPageWidthPoint(m_pdfDocument, pageIndex);
                return new Android.Util.Size(pageWidth, pageHeight);
            }
        }

        /// <summary>
        /// Render the PDF page as bitmap with specified page index
        /// </summary>
        /// <param name="bitmap">Bitmap to draw the content of the PDF page </param>
        /// <param name="pageIndex">Render the bitmap of PDF page with the page index</param>
        /// <param name="pageWidth">Width of the page to draw on to the bitmap</param>
        /// <param name="pageHeight">Height of the page to draw on to the bitmap</param>
        public void Render(Bitmap bitmap, int pageIndex, int pageWidth, int pageHeight)
        {
            if (bitmap == null)
            {
                throw new System.NullReferenceException("object reference is not set to an instance: bitmap");
            }
            if (m_pdfiumCore == null)
            {
                throw new System.NullReferenceException("object reference is not set to an instance: m_pdfiumCore");
            }
            else if (m_pdfDocument == null)
            {
                throw new System.NullReferenceException("object reference is not set to an instance: m_pdfDocument");
            }
            else if (pageIndex < 0 && pageIndex > m_pageCount - 1)
            {
                throw new System.ArgumentOutOfRangeException("pageIndex", "Index was out of range. Must be non-negative and less than the size of the PageCount.");
            }
            else
            {

                //Set the config of Bitmap format we required to render the PDF pages
                bitmap.SetConfig(m_bitmapConfig);
                //Open the PDF page with the specified page index to render the page as bitmap
                m_pdfiumCore.OpenPage(m_pdfDocument, pageIndex);
                m_pdfiumCore.RenderPageBitmap(m_pdfDocument, bitmap, pageIndex, 0, 0, pageWidth, pageHeight);
            }

        }

        /// <summary>
        /// Initialize the required object
        /// </summary>
        /// <param name="context">Context of the application</param>
        /// <param name="inputStream">PDF document stream</param>
        public void Initialize(Context context, Stream inputStream)
        {
            if (inputStream == null)
            {
                throw new System.NullReferenceException("object reference is not set to an instance: inputStream");
            }
            //Initialize PdfiumCore instance
            m_pdfiumCore = new PdfiumCore(context);
            byte[] byteArray = ReadBytes(inputStream);
            if (m_pdfiumCore == null)
            {
                throw new System.NullReferenceException("object reference is not set to an instance: m_pdfiumCore");
            }
            //Create the PdfDocument instance from the PDF byte array
            m_pdfDocument = m_pdfiumCore.NewDocument(byteArray);
            if (m_pdfDocument == null)
            {
                throw new System.NullReferenceException("object reference is not set to an instance: m_pdfDocument");
            }
            if (m_bitmapConfig == null)
            {
                m_bitmapConfig = Bitmap.Config.Rgb565;
            }
            //Get the total number of pages from the PDF document
            m_pageCount = m_pdfiumCore.GetPageCount(m_pdfDocument);
        }

        /// <summary>
        /// Convert stream to byte array to render the PDF document using Pdfium renderer
        /// </summary>
        /// <param name="inputStream">PDF document stream to convert into byte array</param>
        /// <returns>byte array of PDF document</returns>
        private static byte[] ReadBytes(Stream input)
        {
            if (input.CanSeek)
            {
                input.Position = 0;
            }
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    
    }
}