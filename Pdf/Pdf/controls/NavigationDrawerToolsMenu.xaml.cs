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
    public partial class NavigationDrawerToolsMenu : ContentView
    {
        public delegate void signaturePadButtonClickedDelegate();
        public signaturePadButtonClickedDelegate SignaturePadButton { get; set; }

        public delegate void stampButtonButtonClickedDelegate();
        public stampButtonButtonClickedDelegate StampButtonClicked { get; set; }

        public delegate void textButtonClickedDelegate();
        public textButtonClickedDelegate TextButtonClicked { get; set; }

        public delegate void penButtonClickedDelegate();
        public penButtonClickedDelegate PenButtonClicked { get; set; }

        public NavigationDrawerToolsMenu()
        {
            InitializeComponent();

            signaturePadButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    signatureImage.Foreground = Color.FromHex("#b4b4b4");
                    signatureLabel.TextColor = Color.FromHex("#b4b4b4");
                    SignaturePadButton_Clicked();
                    await Task.Delay(100);
                    signatureImage.Foreground = Color.FromHex("#4e4e4e");
                    signatureLabel.TextColor = Color.Black;
                })
            });

            stampButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    stampImage.Foreground = Color.FromHex("#b4b4b4");
                    stampLabel.TextColor = Color.FromHex("#b4b4b4");
                    StampButton_Clicked();
                    await Task.Delay(100);
                    stampImage.Foreground = Color.FromHex("#4e4e4e");
                    stampLabel.TextColor = Color.Black;
                })
            });

            textButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    textImage.Foreground = Color.FromHex("#b4b4b4");
                    textLabel.TextColor = Color.FromHex("#b4b4b4");
                    TextButton_Clicked();
                    await Task.Delay(100);
                    textImage.Foreground = Color.FromHex("#4e4e4e");
                    textLabel.TextColor = Color.Black;
                })
            });

            penButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    penButtonImage.Foreground = Color.FromHex("#b4b4b4");
                    penLabel.TextColor = Color.FromHex("#b4b4b4");
                    PenButton_Clicked();
                    await Task.Delay(100);
                    penButtonImage.Foreground = Color.FromHex("#4e4e4e");
                    penLabel.TextColor = Color.Black;
                })
            });
        }
        private void SignaturePadButton_Clicked()
        {
            SignaturePadButton?.Invoke();
        }

        private void StampButton_Clicked()
        {
            StampButtonClicked?.Invoke();
        }

        private void TextButton_Clicked()
        {
            TextButtonClicked?.Invoke();
        }

        private void PenButton_Clicked()
        {
            PenButtonClicked?.Invoke();
        }


    }
}