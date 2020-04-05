using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Helpers
{
    public interface IMergePages
    {
        Task Merge(FileInfo fileInfo, List<int> pagesNumbers);
    }
}