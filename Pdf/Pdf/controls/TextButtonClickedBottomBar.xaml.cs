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
    public partial class TextButtonClickedBottomBar : ContentView
    {
        public delegate void fontSizeButtonClickedDelegate();
        public fontSizeButtonClickedDelegate FontSizeButtonClicked { get; set; }

        public delegate void colorButtonClickedDelegate();
        public colorButtonClickedDelegate ColorButtonClicked { get; set; }
        public TextButtonClickedBottomBar()
        {
            InitializeComponent();

            fontSizeButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    fontSizeButton.Foreground = Color.FromHex("#b4b4b4");
                    FontSizeButton_Clicked();
                    await Task.Delay(100);
                    fontSizeButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });

            colorButton.Clicked += ColorButton_Clicked;
        }

        private void FontSizeButton_Clicked()
        {
            FontSizeButtonClicked?.Invoke();
        }

        private void ColorButton_Clicked(object sender, EventArgs args)
        {
            ColorButtonClicked?.Invoke();
        }
    }
}