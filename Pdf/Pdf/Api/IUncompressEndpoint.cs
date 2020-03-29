using Pdf.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Api
{
    public interface IUncompressEndpoint
    {
        Task<string> UploadFilesForUncompress(FileInfo fileInfo, IProgress<UploadBytesProgress> progessReporter);
    }
}