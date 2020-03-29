using Pdf.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Api
{
    public interface IDeconcateEndpoint
    {
        Task<string> UploadFilesForDeconcate(FileInfo fileInfo, List<int> pagesNumbers, IProgress<UploadBytesProgress> progessReporter);
    }
}