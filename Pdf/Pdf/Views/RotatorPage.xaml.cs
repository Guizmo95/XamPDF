using Pdf.Enumerations;
using Pdf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RotatorPage : ContentView
    {
        public RotatorPage()
        {
            InitializeComponent();
            rotator.BindingContext = new RotatorViewModel();
        }
    }
}