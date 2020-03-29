using Pdf.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Api
{
    public interface IRemovePagesEndpoint
    {
        Task<string> UploadFilesForRemovePages(FileInfo fileInfo, List<int> pagesNumbers, IProgress<UploadBytesProgress> progessReporter);
    }
}