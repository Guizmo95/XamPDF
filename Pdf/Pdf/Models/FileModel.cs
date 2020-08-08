using SQLite;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Pdf.Models
{
    public class FileModel: INotifyPropertyChanged
    {
        private int id;
        private int itemIndexInDocumentList;
        private int itemIndexInFavoriteDocumentList;
        private long fileLength;

        private string fileName;
        private string size;
        private string filePath;
        private string favoriteImage;
        
        private bool isFavorite;

        private DateTime creationTime;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get => id;

            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        public int ItemIndexInDocumentList
        {
            get => itemIndexInDocumentList;

            set
            {
                itemIndexInDocumentList = value;
                OnPropertyChanged();
            }
        }

        public int ItemIndexInFavoriteDocumentList
        {
            get => itemIndexInFavoriteDocumentList;

            set
            {
                itemIndexInFavoriteDocumentList = value;
                OnPropertyChanged();
            }
        }

        public string FileName
        {
            get => fileName;

            set
            {
                fileName = value;
                OnPropertyChanged();
            }
        }

        public DateTime CreationTime
        {
            get => creationTime;

            set
            {
                creationTime = value;
                OnPropertyChanged();
            }
        }

        public string Size
        {
            get => size;

            set
            {
                size = value;
                OnPropertyChanged();
            }
        }

        public string FilePath
        {
            get => filePath;

            set
            {
                filePath = value;
                OnPropertyChanged();
            }
        }

        public long FileLenght
        {
            get => fileLength;

            set
            {
                fileLength = value;
                OnPropertyChanged();
            }
        }

        public bool IsFavorite
        {
            get => isFavorite;

            set
            {
                isFavorite = value;
                OnPropertyChanged();

                FavoriteImage = value 
                    ? "baseline_favorite_24.xml" 
                        : "baseline_favorite_border_24.xml";

            }
        }

        public string FavoriteImage
        {
            get => favoriteImage;

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
                var order = 0;

                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }

                // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
                // show a single decimal place, and no space.
                this.Size = $"{len:0.##} {sizes[order]}";
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
