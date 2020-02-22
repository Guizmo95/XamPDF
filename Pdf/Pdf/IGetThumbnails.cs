using Android.OS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf
{
    public interface IGetThumbnails
    {
        string GetBitmaps(string filePath);

        ParcelFileDescriptor GetSeekableFileDescriptor(string filePath);
    }
}
