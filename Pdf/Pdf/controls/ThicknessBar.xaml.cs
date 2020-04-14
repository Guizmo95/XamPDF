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
    public partial class ThicknessBar : ContentView
    {
        public delegate void firstBoxViewButtonClickedDelegate();
        public firstBoxViewButtonClickedDelegate FirstBoxViewButtonClicked { get; set; }

        public delegate void secondBoxViewButtonClickedDelegate();
        public firstBoxViewButtonClickedDelegate SecondBoxViewButtonClicked { get; set; }

        public delegate void thirdBoxViewButtonClickedDelegate();
        public firstBoxViewButtonClickedDelegate ThirdBoxViewButtonClicked { get; set; }

        public delegate void fourthBoxViewButtonClickedDelegate();
        public firstBoxViewButtonClickedDelegate FourthBoxViewButtonClicked { get; set; }

        public delegate void fifthBoxViewButtonClickedDelegate();
        public firstBoxViewButtonClickedDelegate FifthBoxViewButtonClicked { get; set; }
        public ThicknessBar()
        {
            InitializeComponent();

            firstBoxView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command( () =>
                {
                    FirstBoxViewButton_Clicked();
                })
            });

            secondBoxView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    SecondBoxViewButton_Clicked();
                })
            });

            thirdBoxView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    ThirdBoxViewButton_Clicked();
                })
            });

            fourthBoxView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    FourthBoxViewButton_Clicked();
                })
            });

            fifthBoxView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    FifthBoxViewButton_Clicked();
                })
            });
        }

        private void FirstBoxViewButton_Clicked()
        {
            FirstBoxViewButtonClicked?.Invoke();
        }

        private void SecondBoxViewButton_Clicked()
        {
            SecondBoxViewButtonClicked?.Invoke();
        }

        private void ThirdBoxViewButton_Clicked()
        {
            ThirdBoxViewButtonClicked?.Invoke();
        }

        private void FourthBoxViewButton_Clicked()
        {
            FourthBoxViewButtonClicked?.Invoke();
        }

        private void FifthBoxViewButton_Clicked()
        {
            FifthBoxViewButtonClicked?.Invoke();
        }
    }
}