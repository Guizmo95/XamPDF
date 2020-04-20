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
    public partial class FreeTextBottomBar : ContentView
    {
        public delegate void colorButtonClickedDelegate();
        public colorButtonClickedDelegate ColorButtonClicked { get; set; }

        public delegate void backButtonClickedDelegate();
        public backButtonClickedDelegate BackButtonClicked { get; set; }

        public delegate void freeTextButtonClicked();
        public freeTextButtonClicked FreeTextButtonClicked { get; set; }

        public Button ColorButton
        {
            get
            {
                return colorButton;
            }
        }


        public PancakeView FreeTextButtonSatus
        {
            get
            {
                return freeTextButtonSatus;
            }
        }

        public FreeTextBottomBar()
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

            freeTextButtonSatus.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    FreeTextButton_Clicked();
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

        private void FreeTextButton_Clicked()
        {
            FreeTextButtonClicked?.Invoke();
        }

    }
}