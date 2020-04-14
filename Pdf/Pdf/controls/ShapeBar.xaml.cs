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
    public partial class ShapeBar : ContentView
    {
        public delegate void arrowButtonClickedDelegate();
        public arrowButtonClickedDelegate ArrowButtonClicked { get; set; }

        public delegate void lineButtonClickedDelegate();
        public lineButtonClickedDelegate LineButtonClicked { get; set; }

        public delegate void ellipseButtonClickedDelegate();
        public ellipseButtonClickedDelegate EllipseButtonClicked { get; set; }

        public delegate void rectangleButtonClickedDelegate();
        public rectangleButtonClickedDelegate RectangleButtonClicked { get; set; }

        public delegate void backButtonClickedDelegate();
        public backButtonClickedDelegate BackButtonClicked { get; set; }

        public ShapeBar()
        {
            InitializeComponent();

            arrowButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    arrowButton.Foreground = Color.FromHex("#b4b4b4");
                    ArrowButton_Clicked();
                    await Task.Delay(100);
                    arrowButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });

            lineButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    lineButton.Foreground = Color.FromHex("#b4b4b4");
                    LineButton_Clicked();
                    await Task.Delay(100);
                    lineButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });

            ellipseButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    ellipseButton.Foreground = Color.FromHex("#b4b4b4");
                    EllipseButton_Clicked();
                    await Task.Delay(100);
                    ellipseButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });

            rectangleButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    rectangleButton.Foreground = Color.FromHex("#b4b4b4");
                    RectangleButton_Clicked();
                    await Task.Delay(100);
                    rectangleButton.Foreground = Color.FromHex("#4e4e4e");
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

        private void ArrowButton_Clicked()
        {
            ArrowButtonClicked?.Invoke();
        }
        private void LineButton_Clicked()
        {
            LineButtonClicked?.Invoke();
        }

        private void EllipseButton_Clicked()
        {
            EllipseButtonClicked?.Invoke();
        }

        private void RectangleButton_Clicked()
        {
            RectangleButtonClicked?.Invoke();
        }

        private void BackButton_Clicked()
        {
            BackButtonClicked?.Invoke();
        }

    }
}