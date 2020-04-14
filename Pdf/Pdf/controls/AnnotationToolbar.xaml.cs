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
    public partial class AnnotationToolbar : ContentView
    {
        public delegate void stampButtonClickedDelegate();
        public stampButtonClickedDelegate StampButtonClicked { get; set; }

        public delegate void freeTextButtonClickedDelegate();
        public freeTextButtonClickedDelegate FreeTextButtonClicked { get; set; }

        public delegate void inkButtonClickedDelegate();
        public inkButtonClickedDelegate InkButtonClicked { get; set; }

        public delegate void backButtonClickedDelegate();
        public backButtonClickedDelegate BackButtonClicked { get; set; }

        public delegate void shapeButtonClickedDelegate();
        public shapeButtonClickedDelegate ShapeButtonClicked { get; set; }
        public AnnotationToolbar()
        {
            InitializeComponent();

            stampButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    backButtonImage.Foreground = Color.FromHex("#b4b4b4");
                    StampButton_Clicked();
                    await Task.Delay(100);
                    backButtonImage.Foreground = Color.FromHex("#4e4e4e");
                })
            });

            freeTextButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    freeTextButton.Foreground = Color.FromHex("#b4b4b4");
                    FreeTextButton_Clicked();
                    await Task.Delay(100);
                    freeTextButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });

            inkButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    inkButton.Foreground = Color.FromHex("#b4b4b4");
                    InkButton_Clicked();
                    await Task.Delay(100);
                    inkButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });

            shapeButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    shapeButton.Foreground = Color.FromHex("#b4b4b4");
                    ShapeButton_Clicked();
                    await Task.Delay(100);
                    shapeButton.Foreground = Color.FromHex("#4e4e4e");
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

        private void StampButton_Clicked()
        {
            StampButtonClicked?.Invoke();
        }

        private void FreeTextButton_Clicked()
        {
            FreeTextButtonClicked?.Invoke();
        }
        private void InkButton_Clicked()
        {
            InkButtonClicked?.Invoke();
        }

        private void ShapeButton_Clicked()
        {
            ShapeButtonClicked?.Invoke();
        }

        private void BackButton_Clicked()
        {
            BackButtonClicked?.Invoke();
        }
    }
}