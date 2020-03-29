using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PdfClient.Models
{
    public class MyFile
    {
        public byte[] data;
        public string fileName;

        public MyFile(byte[] data, string fileName)
        {
            this.data = data;
            this.fileName = fileName;
        }
    }
}