using Pdf.Interfaces;
using Pdf.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace Pdf.ViewModels
{
    public class DocumentViewModel:BaseViewModel
    {
        public IList<FileInfo> Documents { get; set; }
        private readonly IPdfPickerAndroid pdfPickerAndroid;

        public DocumentViewModel()
        {
            pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();
            Documents = pdfPickerAndroid.GetPdfFilesInDocuments();
        }
    }
}
