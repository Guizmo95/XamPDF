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
        public delegate void BoxViewButtonClickedDelegate(int numberOfTheThicknessBarSelected);
        public BoxViewButtonClickedDelegate BoxViewButtonClicked { get; set; }

        public PancakeView FirstBoxView
        {
            get
            {
                return firstBoxView;
            }
        }

        public PancakeView SecondBoxView
        {
            get
            {
                return secondBoxView;
            }
        }

        public PancakeView ThirdBoxView
        {
            get
            {
                return thirdBoxView;
            }
        }

        public PancakeView FourthBoxView
        {
            get
            {
                return fourthBoxView;
            }
        }

        public PancakeView FifthBoxView
        {
            get
            {
                return fifthBoxView;
            }
        }

        public PancakeView FirstThicknessBar
        {
            get
            {
                return firstThicknessBar;
            }
        }

        public PancakeView SecondThicknessBar
        {
            get
            {
                return secondThicknessBar;
            }
        }

        public PancakeView ThirdThicknessBar
        {
            get
            {
                return thirdThicknessBar;
            }
        }

        public PancakeView FourthThicknessBar
        {
            get
            {
                return fourthThicknessBar;
            }
        }

        public PancakeView FifthThicknessBar
        {
            get
            {
                return fifthThicknessBar;
            }
        }

        public ThicknessBar()
        {
            InitializeComponent();

            firstBoxView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command( () =>
                {
                    BoxViewButton_Clicked(1);
                })
            });

            secondBoxView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    BoxViewButton_Clicked(2);
                })
            });

            thirdBoxView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    BoxViewButton_Clicked(3);
                })
            });

            fourthBoxView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    BoxViewButton_Clicked(4);
                })
            });

            fifthBoxView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    BoxViewButton_Clicked(5);
                })
            });
        }

        private void BoxViewButton_Clicked(int numberOfTheThicknessBarSelected)
        {
            BoxViewButtonClicked?.Invoke(numberOfTheThicknessBarSelected);
        }
    }
}