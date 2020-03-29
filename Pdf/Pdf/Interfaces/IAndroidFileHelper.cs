using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Interfaces
{
    public interface IAndroidFileHelper
    {
            void SaveFileInDownloads(string filename, byte[] file);
            void SaveFileInDocFolder(string fileName, byte[] file);
            Task<List<string>> UnzipFileInDownload(string fileName);
            byte[] ReadFully(Stream input);
            byte[] LoadLocalFile(string filePath);
    }
}