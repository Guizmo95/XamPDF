using System.IO;

namespace Pdf.Interfaces
{
    public interface IAndroidFileHelper
    {
            void SaveFile(string filename, byte[] file);
            byte[] ReadFully(Stream input);
            byte[] LoadLocalFile(string filePath);
    }
}