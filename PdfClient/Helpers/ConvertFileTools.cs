using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace PdfClient.Helpers
{
    public static class ConvertFileTools
    {
        public static string ConcateFiles(string fileName1, string fileName2) {

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads");

            string outputName = Path.GetRandomFileName();
            startInfo.Arguments = "/C pdftk "+ fileName1 + fileName2 + "cat output " + outputName;

            process.StartInfo = startInfo;
            process.Start();

            return outputName;
        }
    }
}