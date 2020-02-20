using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace PdfClient.Helpers
{
    public static class ConvertFileTools
    {
        public static string ConcateFiles(List<string> filesName) {

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads");

            string outputName = Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + ".pdf";

            string argFilesName =  "";

            filesName.ForEach(delegate (string fileName)
            {
                argFilesName += string.Format(fileName + " ");
            });
            startInfo.Arguments = "/C pdftk " + argFilesName + "cat output " + outputName;

            process.StartInfo = startInfo;
            process.Start();

            return outputName;
        }

        public static string CleanDate(string date) {
            StringBuilder sb = new StringBuilder(date);

            sb.Replace(" ", "");
            sb.Replace("/", "");
            sb.Replace(":", "");

            return sb.ToString();
        }
    }
}