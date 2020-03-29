using System.Threading.Tasks;

namespace Pdf.Api
{
    public interface IGetFilesEndpoint
    {
        Task<byte[]> GetFileConcated(string fileName);
    }
}