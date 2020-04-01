using Pdf.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Api
{
    public interface IConcateEndpoint
    {
        Task<string> UploadFilesForConcateDocs(List<FileInfo> filesInfo, IProgress<UploadBytesProgress> progessReporter);

        Task<string> UploadFilesForConcatePages(FileInfo fileInfo, List<int> pagesNumbers, IProgress<UploadBytesProgress> progessReporter);
    }
}