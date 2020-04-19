using Syncfusion.SfPdfViewer.XForms;
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
    public partial class EditPopupMenu : ContentView
    {
        public delegate void styleButtonClickedDelegate();
        public styleButtonClickedDelegate StyleButtonClicked { get; set; }

        public delegate void removeButtonClickedDelegate();
        public removeButtonClickedDelegate RemoveButtonClicked { get; set; }

        public StackLayout EditPopupContent
        {
            get
            {
                return editPopupContent;
            }
        }

        public EditPopupMenu()
        {
            InitializeComponent();
        }

        private void StyleButton_Clicked(object sender, EventArgs e)
        {
            StyleButtonClicked?.Invoke();
        }

        private void RemoveButton_Clicked(object sender, EventArgs e)
        {
            RemoveButtonClicked?.Invoke();
        }
    }
}