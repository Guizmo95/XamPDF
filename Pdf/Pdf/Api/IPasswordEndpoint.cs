using System.IO;
using System.Threading.Tasks;

namespace Pdf.Api
{
    public interface IPasswordEndpoint
    {
        Task<string> UploadFilesForPassword(FileInfo fileInfo, string password);
    }
}