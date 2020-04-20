﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShapeCustomBar : ContentView
    {
        public delegate void backButtonClickedDelegate();
        public backButtonClickedDelegate BackButtonClicked { get; set; }

        public delegate void colorButtonClickedDelegate();
        public colorButtonClickedDelegate ColorButtonClicked { get; set; }

        public IconView ShapeImageButton
        {
            get
            {
                return shapeImage;
            }
        }

        public AbsoluteLayout MainAbsoluteLayout
        {
            get
            {
                return mainAbsoluteLayout;
            }
        }

        public ShapeCustomBar()
        {
            InitializeComponent();

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

            colorButton.Clicked += ColorButton_Clicked;
        }

        private void BackButton_Clicked()
        {
            BackButtonClicked?.Invoke();
        }

        //Set parameter because needed by visual studio - They are useless
        private void ColorButton_Clicked(object sender, EventArgs args)
        {
            ColorButtonClicked?.Invoke();
        }
    }
}