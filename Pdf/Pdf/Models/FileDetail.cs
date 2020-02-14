using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Models
{
    public class FileDetail
    {
        private string fileName;
        private string createdDate;
        private int poids;

        public string FileName
        {
            get
            {
                return fileName;
            }

            set
            {
                fileName = value;
            }
        }

        public string CreatedDate
        {
            get
            {
                return createdDate;
            }

            set
            {
                createdDate = value;
            }
        }

        public int Poids
        {
            get
            {
                return poids;
            }

            set
            {
                poids = value;
            }
        }
    }
}
