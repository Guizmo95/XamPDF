using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pdf.ViewModels
{
    public class PdfViewerModel
    {
        private Stream m_pdfDocumentStream;

        public Stream PdfDocumentStream
        {
            get
            {
                return m_pdfDocumentStream;
            }
            set
            {
                m_pdfDocumentStream = value;
            }
        }

        public PdfViewerModel(Stream stream)
        {
            m_pdfDocumentStream = stream;
        }
    }
}
