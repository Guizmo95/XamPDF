using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pdf.Models
{
    public class FileModel: INotifyPropertyChanged
    {
        private int id;
        private string fileName;
        private DateTime creationTime;
        private long size;
        private string filePath;

        public event PropertyChangedEventHandler PropertyChanged;

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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
