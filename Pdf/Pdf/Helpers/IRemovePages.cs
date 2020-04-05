using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Helpers
{
    public interface IRemovePages
    {
        Task RemovePageInAnExistingPdf(FileInfo fileInfo, List<int> pagesNumbers);
    }
}