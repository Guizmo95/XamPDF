using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PdfClient.Helpers
{
    public static class ZipHelper
    {
        public static void CreateZipFromFiles(string zipName, List<string> filesNames)
        {
            using (var archive = ZipFile.Open(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/" + zipName)), ZipArchiveMode.Create))
            {
                filesNames.ForEach(delegate (string file)
                {
                    archive.CreateEntryFromFile(HttpContext.Current.Server.MapPath("~/Uploads/" + file), file);
                });
            }
        }
    }
}