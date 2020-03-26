using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Interfaces
{
    public interface IAndroidDownloadManager
    {
        void Download(string uri, string fileName);
    }
}
