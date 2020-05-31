using Android.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pdf.Models
{
    public class ItemsMenu: INotifyPropertyChanged
    {
        private int id;
        private string image;
        private Xamarin.Forms.Color textColor;
        private Xamarin.Forms.Color imageColor;
        private string textOption;

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

        public Xamarin.Forms.Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;
                OnPropertyChanged();
            }
        }

        public Xamarin.Forms.Color ImageColor
        {
            get
            {
                return imageColor;
            }
            set
            {
                imageColor = value;
                OnPropertyChanged();
            }
        }

        public string TextOption
        {
            get
            {
                return textOption;
            }
            set
            {
                textOption = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
