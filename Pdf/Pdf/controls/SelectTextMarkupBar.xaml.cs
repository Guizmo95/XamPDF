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
    public partial class SelectTextMarkupBar : ContentView
    {
        public delegate void strikethroughtButtonClickedDelegate();
        public strikethroughtButtonClickedDelegate StrikethroughtButtonClicked { get; set; }

        public delegate void underlineButtonClickedDelegate();
        public underlineButtonClickedDelegate UnderlineButtonClicked { get; set; }

        public delegate void hightlightButtonClickedDelegate();
        public hightlightButtonClickedDelegate HightlightButtonClicked { get; set; }

        public delegate void backButtonClickedDelegate();
        public backButtonClickedDelegate BackButtonClicked { get; set; }
        public SelectTextMarkupBar()
        {
            InitializeComponent();

            strikethroughtButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    strikethroughtButton.Foreground = Color.FromHex("#b4b4b4");
                    StrikethroughtButton_Clicked();
                    await Task.Delay(100);
                    strikethroughtButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });

            underlineButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    underlineButton.Foreground = Color.FromHex("#b4b4b4");
                    UnderlineButton_Clicked();
                    await Task.Delay(100);
                    underlineButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });

            hightlightButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    hightlightButton.Foreground = Color.FromHex("#b4b4b4");
                    HightlightButton_Clicked();
                    await Task.Delay(100);
                    hightlightButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });

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

        private void StrikethroughtButton_Clicked()
        {
            StrikethroughtButtonClicked?.Invoke();
        }
        private void UnderlineButton_Clicked()
        {
            UnderlineButtonClicked?.Invoke();
        }

        private void HightlightButton_Clicked()
        {
            HightlightButtonClicked?.Invoke();
        }

        private void BackButton_Clicked()
        {
            BackButtonClicked?.Invoke();
        }
    }
}