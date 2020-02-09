using System.IO;

namespace Pdf.Droid
{
    public interface IAndroidWritter
    {
            void SaveFile(string filename, byte[] file);
            byte[] ReadFully(Stream input);
    }
}