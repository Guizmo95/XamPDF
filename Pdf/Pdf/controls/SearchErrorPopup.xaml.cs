using Android.Media;
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
    public partial class SearchErrorPopup : ContentView
    {
        public Label NoMoreOccurenceFound
        {
            get
            {
                return noMoreOccurenceFound;
            }
        }

        public Label NoOccurenceFound
        {
            get
            {
                return noOccurenceFound;
            }
        }

        public SearchErrorPopup()
        {
            InitializeComponent();
        }
    }
}