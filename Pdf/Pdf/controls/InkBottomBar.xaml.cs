using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;

namespace Pdf.controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InkBottomBar : ContentView
    {
        public delegate void colorButtonClickedDelegate();
        public colorButtonClickedDelegate ColorButtonClicked { get; set; }

        public delegate void backButtonClickedDelegate();
        public backButtonClickedDelegate BackButtonClicked { get; set; }

        public delegate void penSizeButtonClickedDelegate();
        public penSizeButtonClickedDelegate PenSizeButtonClicked { get; set; }

        public delegate void penStatusButtonClickedDelegate();
        public penStatusButtonClickedDelegate PenStatusButtonClicked { get; set; }

        public delegate void trashButtonClickedDelegate();
        public trashButtonClickedDelegate TrashButtonClicked { get; set; }



        public Button ColorButton
        {
            get
            {
                return colorButton;
            }
        }

        public IconView PenSizeButton
        {
            get
            {
                return penSizeButton;
            }
        }

        public PancakeView PenButtonStatus
        {
            get
            {
                return penButtonStatus;
            }
        }


        public InkBottomBar()
        {
            InitializeComponent();

            colorButton.Clicked += ColorButton_Clicked;

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

            penButtonStatus.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command( () =>
                {
                    PenStatusButton_Clicked();
                })
            });

        }

        //Set parameter because needed by visual studio - They are useless
        private void ColorButton_Clicked(object sender, EventArgs args)
        {
            ColorButtonClicked?.Invoke();
        }

        private void BackButton_Clicked()
        {
            BackButtonClicked?.Invoke();
        }

        private void PenStatusButton_Clicked()
        {
            PenStatusButtonClicked?.Invoke();
        }

    }
}