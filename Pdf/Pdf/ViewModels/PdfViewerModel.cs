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

        public event PropertyChangedEventHandler PropertyChanged;

        private MemoryStream pdfStream;


        public PdfViewerModel(string filePath)
        {
           
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
