using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pdf.Models
{
    public class FileModel
    {
        private int id;
        private string fileName;
        private DateTime creationTime;
        private long size;
        private string filePath;

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

        public DateTime CreationTime
        {
            get
            {
                return creationTime;
            }

            set
            {
                creationTime = value;
            }
        }

        public long Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }

        public string FilePath
        {
            get
            {
                return filePath;
            }

            set
            {
                filePath = value;
            }
        }

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public FileModel(int id, string fileName, DateTime creation, long size, string filePath)
        {
            this.Id = id;
            this.FileName = fileName;
            this.CreationTime = creation;
            this.Size = size;
            this.FilePath = filePath;
        }
    }
}
