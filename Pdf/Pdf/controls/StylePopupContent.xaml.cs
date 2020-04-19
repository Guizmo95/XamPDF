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
    public partial class StylePopupContent : ContentView
    {
        public ColorPicker ColorPicker
        {
            get
            {
                return colorPicker;
            }
        }

        public FontSizeSlider FontSizeSlider
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

        public OpacitySlider OpacitySlider
        {
            get
            {
                return opacitySlider;
            }
        }


        public StylePopupContent()
        {
            InitializeComponent();
        }
    }
}