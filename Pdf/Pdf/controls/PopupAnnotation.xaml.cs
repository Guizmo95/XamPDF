using Syncfusion.XForms.PopupLayout;
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
    public partial class PopupAnnotation : ContentView
    {
        public SfPopupLayout SfPopupLayout
        {
            get
            {
                return popupLayout;
            }
        }

        public PopupAnnotation()
        {
            InitializeComponent();
        }
    }
}