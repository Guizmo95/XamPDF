using System.IO;

namespace Pdf.Droid
{
    public interface IAndroidWritter
    {
            string SaveFile(string filename, Stream fileStream);
            byte[] ReadFully(Stream input);
    }
}