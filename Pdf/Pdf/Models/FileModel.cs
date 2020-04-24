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
        private string size;
        private string filePath;
        private long fileLenght;

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

        public string Size
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

        public long FileLenght
        {
            get
            {
                return fileLenght;
            }

            set
            {
                fileLenght = value;
                OnPropertyChanged();
            }
        }

        public FileModel(int id, string fileName, DateTime creation, long fileLenght, string filePath)
        {
            this.Id = id;
            this.FileName = fileName;
            this.CreationTime = creation;
            this.FileLenght = fileLenght;
            this.FilePath = filePath;

            GetHumanReadableFileSize();
        }

        public void GetHumanReadableFileSize()
        {
            if(FileLenght != 0)
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                double len = FileLenght;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }

                // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
                // show a single decimal place, and no space.
                this.Size = String.Format("{0:0.##} {1}", len, sizes[order]);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
