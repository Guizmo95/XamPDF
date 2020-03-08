using Android.OS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdf.Interfaces
{
    public interface IGetThumbnails
    {
        string GetBitmaps(string filePath);

        ParcelFileDescriptor GetSeekableFileDescriptor(string filePath);
    }
}
