using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Helpers
{
    public interface IMergeDocuments
    {
        Task Merge(List<FileInfo> fileInfos);
    }
}