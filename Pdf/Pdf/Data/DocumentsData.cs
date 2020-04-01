using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pdf.Data
{
    public class DocumentsData:IDocumentsData
    {
        public IList<FileInfo> Documents { get; set; }
    }
}
