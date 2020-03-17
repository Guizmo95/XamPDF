using Android.OS;
using Pdf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdf.Interfaces
{
    public interface IGetThumbnails
    {
        bool InProccess { get; set; }
        List<ThumbnailsModel> Items { get; set; }
        Task<List<ThumbnailsModel>> GetBitmaps(string filePath, int lastIndex = 0, int numberOfItemsPerPage = 5);
        Task<IEnumerable<ThumbnailsModel>> GetItemsAsync(string filePath, bool forceRefresh = false, int lastIndex = 0);
        Dictionary<bool, int> IsUnder15Pages(string filePath);
        ParcelFileDescriptor GetSeekableFileDescriptor(string filePath);
    }
}
