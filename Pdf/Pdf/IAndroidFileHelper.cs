using System.IO;

namespace Pdf.Droid
{
    public interface IAndroidFileHelper
    {
            void SaveFile(string filename, byte[] file);
            byte[] ReadFully(Stream input);
            byte[] LoadLocalFile(string filePath);
    }
}