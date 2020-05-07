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

        public Slider OpacitySlider
        {
            get
            {
                return opacitySlider;
            }
        }


        public StyleContent()
        {
            InitializeComponent();
        }
    }
}