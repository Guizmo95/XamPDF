using System.IO;

namespace Pdf.Interfaces
{
    public interface IAndroidFileHelper
    {
            void SaveFileInDownloads(string filename, byte[] file);
            void SaveFileInDocFolder(string fileName, byte[] file);
            byte[] ReadFully(Stream input);
            byte[] LoadLocalFile(string filePath);
    }
}