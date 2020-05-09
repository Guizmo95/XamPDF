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
        public delegate void OpacityButtonClickedDelegate(int numberOfOThepacitySelected);
        public OpacityButtonClickedDelegate OpacityButtonClicked { get; set; }

        public Grid OpacityControl
        {
            get
            {
                return opacityControl;
            }
        }

        public BoxView BoxView2
        {
            get
            {
                return boxView2;
            }
        }

        public StackLayout FontSizeControl
        {
            get
            {
                return fontSizeControl;
            }
        }

        public ColorPicker ColorPicker
        {
            get
            {
                return colorPicker;
            }
        }

        public Slider FontSizeSlider
        {
            get
            {
                return fontSizeSlider;
            }
        }

        public ThicknessBar ThicknessBar
        {
            get
            {
                return thicknessBar;
            }
        }

        public IconView OpacityImageTo25
        {
            get
            {
                return opacityImageTo25;
            }
        }

        public IconView OpacityImageTo50
        {
            get
            {
                return opacityImageTo50;
            }
        }

        public IconView OpacityImageTo75
        {
            get
            {
                return opacityImageTo75;
            }
        }

        public IconView OpacityImageTo100
        {
            get
            {
                return opacityImageTo100;
            }
        }


        public StyleContent()
        {
            InitializeComponent();

            opacityTo25Selection.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    OpacityButton_Clicked(1);
                })
            });

            opacityTo50Selection.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    OpacityButton_Clicked(2);
                })
            });

            opacityTo75Selection.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    OpacityButton_Clicked(3);
                })
            });

            opacityTo100Selection.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    OpacityButton_Clicked(4);
                })
            });
        }

        private void OpacityButton_Clicked(int numberOfOThepacitySelected)
        {
            OpacityButtonClicked?.Invoke(numberOfOThepacitySelected);
        }
    }
}