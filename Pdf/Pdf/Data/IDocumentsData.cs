using System.Collections.Generic;
using System.IO;

namespace Pdf.Data
{
    public interface IDocumentsData
    {
        IList<FileInfo> Documents { get; set; }
    }
}