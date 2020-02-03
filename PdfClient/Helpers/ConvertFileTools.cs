using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PdfClient.Helpers
{
    public class ConvertFileTools
    {
        public static void ConcateFiles(string FileName1, string FileName2) {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //startInfo.FileName = "cmd.exe";
            // Set the datafile path relative to the application's path.
            //dataFile = HostingEnvironment.ApplicationPhysicalPath + "App_Data\\XMLData.xml";
            startInfo.Arguments = AppDomain.CurrentDomain.BaseDirectory + "pdftk A.pdf B.pdf cat output C.pdf";
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}