using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Helpers
{
    public interface ISplitPdf
    {
        Task SplitPdfPagesInAnExistingDocument(FileInfo fileInfo, List<int> pagesNumber);
    }
}