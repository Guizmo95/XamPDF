using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditInkBar : ContentView
    {
        public delegate void colorButtonClickedDelegate();
        public colorButtonClickedDelegate ColorButtonClicked { get; set; }

        public delegate void backButtonClickedDelegate();
        public backButtonClickedDelegate BackButtonClicked { get; set; }

        public delegate void inkSizeButtonClickedDelegate();
        public inkSizeButtonClickedDelegate InkSizeButtonClicked { get; set; }

        public delegate void trashButtonClickedDelegate();
        public trashButtonClickedDelegate TrashButtonClicked { get; set; }
        public EditInkBar()
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

            penSizeButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    penSizeButton.Foreground = Color.FromHex("#b4b4b4");
                    InkSizeButton_Clicked();
                    await Task.Delay(100);
                    penSizeButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });

            trashCanButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    trashCanButton.Foreground = Color.FromHex("#b4b4b4");
                    TrashButton_Clicked();
                    await Task.Delay(100);
                    trashCanButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });
        }

        private void ColorButton_Clicked(object sender, EventArgs args)
        {
            ColorButtonClicked?.Invoke();
        }

        private void BackButton_Clicked()
        {
            BackButtonClicked?.Invoke();
        }

        private void InkSizeButton_Clicked()
        {
            InkSizeButtonClicked?.Invoke();
        }

        private void TrashButton_Clicked()
        {
            TrashButtonClicked?.Invoke();
        }
    }
}