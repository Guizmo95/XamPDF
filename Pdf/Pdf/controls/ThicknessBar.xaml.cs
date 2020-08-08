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
    public partial class ThicknessBar : ContentView
    {
        public delegate void BoxViewButtonClickedDelegate(int thicknessBarSelected);
        public BoxViewButtonClickedDelegate ThicknessBarClicked { get; set; }

        public PancakeView ThicknessBarOne => ThicknessBar1;

        public PancakeView ThicknessBarTwo => ThicknessBar2;

        public PancakeView ThicknessBarThird => ThicknessBar3;

        public PancakeView ThicknessBarFourth => ThicknessBar4;

        public PancakeView ThicknessBarFifth => ThicknessBar5;

        public ThicknessBar()
        {
            InitializeComponent();

            AddGestureRecognizers();
        }

        private void AddGestureRecognizers()
        {
            BoxView1.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { BoxViewButton_Clicked(1); })
            });

            BoxView2.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { BoxViewButton_Clicked(2); })
            });

            BoxView3.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { BoxViewButton_Clicked(3); })
            });

            BoxView4.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { BoxViewButton_Clicked(4); })
            });

            BoxView4.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { BoxViewButton_Clicked(5); })
            });
        }

        private void BoxViewButton_Clicked(int selectedThicknessBar)
        {
            ThicknessBarClicked?.Invoke(selectedThicknessBar);
        }
    }
}