﻿using Pdf.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdf.Api
{
    public interface ISummaryEndpoint
    {
        Task<string> UploadFilesForSummary(FileInfo fileInfo, List<SummaryModel> summaries);
    }
}