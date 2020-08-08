using Pdf.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.controls
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