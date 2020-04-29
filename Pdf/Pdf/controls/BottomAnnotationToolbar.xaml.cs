using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomAnnotationToolbar : ContentView, INotifyPropertyChanged
    {
        public delegate void backButtonClickedDelegate();
        public backButtonClickedDelegate BackButtonClicked { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public IconView AnnotationImage
        {
            get
            {
                return annotationImage;
            }

            set
            {
                annotationImage = value;
                OnPropertyChanged();
            }
        } 

        public BottomAnnotationToolbar()
        {
            InitializeComponent();


            backButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    backButtonImage.Foreground = Color.FromHex("#b4b4b4");
                    BackButton_Clicked();
                    await Task.Delay(100);
                    backButtonImage.Foreground = Color.FromHex("#4e4e4e");
                })
            });
        }

        private void BackButton_Clicked()
        {
            BackButtonClicked?.Invoke();
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}