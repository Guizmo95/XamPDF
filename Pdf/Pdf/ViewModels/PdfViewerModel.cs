using Pdf.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace Pdf.ViewModels
{
    public class PdfViewerModel : INotifyPropertyChanged
    {
        private Stream m_pdfDocumentStream;
        private System.Drawing.Color selectedColor = System.Drawing.Color.Black;

        public event PropertyChangedEventHandler PropertyChanged;

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

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
