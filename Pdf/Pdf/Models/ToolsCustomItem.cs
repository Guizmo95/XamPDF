using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pdf.Models
{
    public class ToolsCustomItem: INotifyPropertyChanged
    {
        private int id;
        private string libelle;
        private string detail;
        private string image;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public string Libelle
        {
            get
            {
                return libelle;
            }
            set
            {
                libelle = value;
                OnPropertyChanged();
            }
        }

        public string Detail
        {
            get
            {
                return detail;
            }

            set
            {
                detail = value;
                OnPropertyChanged();
            }
        }

        public string Image
        {
            get
            {
                return image;
            }

            set
            {
                image = value;
                OnPropertyChanged();
            }
        }

        public ToolsCustomItem(int id, string image, string libelle = null,  string detail = null)
        {
            this.id = id;
            this.libelle = libelle;
            this.image = image;
            this.detail = detail;
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
