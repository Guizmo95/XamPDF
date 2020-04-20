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
    public partial class TextMarkupBar : ContentView
    {
        public delegate void colorButtonClickedDelegate();
        public colorButtonClickedDelegate ColorButtonClicked { get; set; }

        public delegate void backButtonClickedDelegate();
        public backButtonClickedDelegate BackButtonClicked { get; set; }

        public delegate void textMarkupStatusButtonClicked();
        public textMarkupStatusButtonClicked TextMarkupStatusButtonClicked { get; set; }

        public IconView TextMarkupButtonSatusIcon
        {
            get
            {
                return textMarkupButtonSatusIcon;
            }
        }
        public PancakeView TextMarkupButtonStatus
        {
            get
            {
                return textMarkupButtonSatus;
            }
        }

        public Button ColorButton
        {
            get
            {
                return colorButton;
            }
        }

        public TextMarkupBar()
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


            textMarkupButtonSatus.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    TextMarkupStatusButton_Clicked();
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

        private void TextMarkupStatusButton_Clicked()
        {
            TextMarkupStatusButtonClicked?.Invoke();
        }
    }
}