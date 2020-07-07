using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Pdf.Interfaces
{
    public interface IPdfPicker
    {
        List<FileInfo> GetPdfFilesInDocuments();
    }
}