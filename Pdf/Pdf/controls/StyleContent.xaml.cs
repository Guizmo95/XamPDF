using Pdf.Views;
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
    public partial class StyleContent : ContentView
    {
        public delegate void OpacityButtonClickedDelegate(int opacitySelected);
        public OpacityButtonClickedDelegate OpacityButtonClicked { get; set; }

        public Grid OpacityControl => opacityControl;

        public BoxView Separator1 => SeparatorLine1;

        public BoxView SeparatorTwo => SeparatorLine2;

        public StackLayout FontSizeControl => FontControl;

        public ColorPicker ColorPicker => CPicker;

        public Slider FontSizeSlider => FSlider;

        public ThicknessBar ThicknessBar => Thickness;

        public IconView OpacityOne => Opacity1;

        public IconView OpacityTwo => Opacity2;

        public IconView OpacityThird => Opacity3;

        public IconView OpacityFourth => Opacity4;

        public StyleContent()
        {
            InitializeComponent();

            AddGestureRecognizers();
        }

        private void AddGestureRecognizers()
        {
            Opacity25.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { OpacityButton_Clicked(1); })
            });

            Opacity50.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { OpacityButton_Clicked(2); })
            });

            Opacity75.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { OpacityButton_Clicked(3); })
            });

            Opacity100.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { OpacityButton_Clicked(4); })
            });
        }

        private void OpacityButton_Clicked(int opacitySelected)
        {
            OpacityButtonClicked?.Invoke(opacitySelected);
        }
    }
}