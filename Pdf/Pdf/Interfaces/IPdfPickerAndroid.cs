using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Pdf.Interfaces
{
    public interface IPdfPickerAndroid
    {
        List<FileInfo> GetPdfFilesInDocuments();
    }
}