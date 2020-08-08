using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Interfaces
{
    public interface IAndroidFileHelper
    {
            Task<Dictionary<bool, string>> SaveAndReturnStatus(MemoryStream stream, string FilePath);
            List<FileInfo> GetPdfFiles();
            MemoryStream GetFileStream(string filePath);
    }
}