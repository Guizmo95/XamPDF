namespace Pdf.Droid
{
    public interface IAndroidWritter
    {
            string CreateFile(string filename, byte[] bytes);
    }
}