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
    public partial class EditFreeTextBar : ContentView
    {
        public delegate void trashCanButtonClickedDelegate();
        public trashCanButtonClickedDelegate TrashCanButtonClicked { get; set; }

        public delegate void fontSizeButtonClickedDelegate();
        public fontSizeButtonClickedDelegate FontSizeButtonClicked { get; set; }

        public delegate void colorButtonClickedDelegate();
        public colorButtonClickedDelegate ColorButtonClicked { get; set; }

        public delegate void backButtonClickedDelegate();
        public backButtonClickedDelegate BackButtonClicked { get; set; }

        public delegate void changeTextForfreeTextSelectedButtonClicked();
        public changeTextForfreeTextSelectedButtonClicked ChangeTextInFreeTextSelectedButtonClicked { get; set; }
        public EditFreeTextBar()
        {
            InitializeComponent();

            trashCanButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    trashCanButton.Foreground = Color.FromHex("#b4b4b4");
                    TrashCanButton_Clicked();
                    await Task.Delay(100);
                    trashCanButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });

            fontSizeButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    fontSizeButton.Foreground = Color.FromHex("#b4b4b4");
                    FontSizeButton_Clicked();
                    await Task.Delay(100);
                    fontSizeButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });

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

            changeTextForFreeTextSelected.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    changeTextForFreeTextSelected.Foreground = Color.FromHex("#b4b4b4");
                    ChangeTextForFreeTextSelectedButton_Clicked();
                    await Task.Delay(100);
                    changeTextForFreeTextSelected.Foreground = Color.FromHex("#4e4e4e");
                })
            });

        }

        private void TrashCanButton_Clicked()
        {
            TrashCanButtonClicked?.Invoke();
        }

        private void FontSizeButton_Clicked()
        {
            FontSizeButtonClicked?.Invoke();
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
        private void ChangeTextForFreeTextSelectedButton_Clicked()
        {
            ChangeTextInFreeTextSelectedButtonClicked?.Invoke();
        }
    }
}