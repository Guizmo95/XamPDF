using Pdf.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Api
{
    public interface IOverlayEndpoint
    {
        Task<string> UploadFilesForWatermark(List<FileInfo> filesInfo, IProgress<UploadBytesProgress> progessReporter);

        Task<string> UploadFilesForStump(List<FileInfo> filesInfo, IProgress<UploadBytesProgress> progessReporter);
    }
}