using Pdf.Interfaces;
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
        private System.Drawing.Color selectedColor = System.Drawing.Color.Black;

        public event PropertyChangedEventHandler PropertyChanged;

        private MemoryStream pdfStream;

        //The PDF stream to be loaded into PdfViewer
        public MemoryStream PdfStream
        {
            get
            {
                return pdfStream;
            }
            set
            {
                pdfStream = value;
                OnPropertyChanged();
            }
        }


        public PdfViewerModel(string filePath)
        {
            pdfStream = DependencyService.Get<IAndroidFileHelper>().GetFileStream(filePath);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
