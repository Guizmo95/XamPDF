using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace PdfClient.Helpers
{
    public static class PdftkTools
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

        public static string ConcatePages(string fileName, List<int> pagesNumbers)
        {

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads");

            string outputName = Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + ".pdf";

            string argPagesNumbers = "";

            pagesNumbers.ForEach(delegate (int pageNumber)
            {
               argPagesNumbers += string.Format(pageNumber + " ");
            });
            startInfo.Arguments = "/C pdftk " + fileName + " cat " + argPagesNumbers + "output " + outputName;

            process.StartInfo = startInfo;
            process.Start();

            return outputName;
        }

        public static List<string> DeconcatePages(string fileName, List<int> pagesNumbers)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads");

            List<string> filesNames = new List<string>();

            pagesNumbers.ForEach(delegate (int pageNumber)
            {
                string outputName = Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + ".pdf";

                startInfo.Arguments = "/C pdftk " + fileName + " cat " + pageNumber + " output " + outputName;
                process.StartInfo = startInfo;
                process.Start();

                filesNames.Add(outputName);
            });
            return filesNames;
        }

        public static string AddWatermark(List<string> filesNames)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads");

            string outputName = Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + ".pdf";

            startInfo.Arguments = "/C pdftk " + filesNames[0] + " background " + filesNames[1] + " output " + outputName;

            process.StartInfo = startInfo;
            process.Start();

            return outputName;
        }

        public static string AddStump(List<string> filesNames)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads");

            string outputName = Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + ".pdf";

            startInfo.Arguments = "/C pdftk " + filesNames[0] + " stamp " + filesNames[1] + " output " + outputName;

            process.StartInfo = startInfo;
            process.Start();

            return outputName;
        }

        public static string RemovePages(string fileName, List<int> pagesNumbers)
        {

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads");

            string outputName = Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + ".pdf";

            string argPagesNumbers = "";

            pagesNumbers.ForEach(delegate (int pageNumber)
            {
                argPagesNumbers += string.Format(pageNumber + " ");
            });
            startInfo.Arguments = "/C pdftk " + fileName + " cat " + argPagesNumbers + "output " + outputName;

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