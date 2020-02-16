using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Pdf.Droid
{
    public interface IPdfPickerAndroid
    {
        List<FileInfo> GetPdfFilesInDocuments();
    }
}