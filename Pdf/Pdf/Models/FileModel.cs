using SQLite;
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
        private int itemIndexInDocumentList;
        private int itemIndexInFavoriteDocumentList;
        private string fileName;
        private DateTime creationTime;
        private string size;
        private string filePath;
        private long fileLenght;
        private bool isFavorite;
        private string favoriteImage;


        [PrimaryKey, AutoIncrement]
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

        public int ItemIndexInDocumentList
        {
            get
            {
                return itemIndexInDocumentList;
            }

            set
            {
                itemIndexInDocumentList = value;
                OnPropertyChanged();
            }
        }

        public int ItemIndexInFavoriteDocumentList
        {
            get
            {
                return itemIndexInFavoriteDocumentList;
            }

            set
            {
                itemIndexInFavoriteDocumentList = value;
                OnPropertyChanged();
            }
        }

        public string FileName
        {
            get
            {
                return fileName;
            }

            set
            {
                fileName = value;
                OnPropertyChanged();
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

        public bool IsFavorite
        {
            get
            {
                return isFavorite;
            }

            set
            {
                isFavorite = value;
                OnPropertyChanged();

                if(value == true)
                {
                    FavoriteImage = "baseline_favorite_24.xml";
                }
                else
                {
                    FavoriteImage = "baseline_favorite_border_24.xml";
                }

            }
        }

        public string FavoriteImage
        {
            get
            {
                return favoriteImage;
            }

            set
            {
                favoriteImage = value;
                OnPropertyChanged();
            }
        }
   

        public event PropertyChangedEventHandler PropertyChanged;

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
