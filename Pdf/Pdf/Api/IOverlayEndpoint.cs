using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Api
{
    public interface IOverlayEndpoint
    {
        Task<string> UploadFilesForWatermark(List<FileInfo> filesInfo);

        Task<string> UploadFilesForStump(List<FileInfo> filesInfo);
    }
}