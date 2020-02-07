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
    public partial class GetDownload : ContentPage
    {
        string fileName;
        public GetDownload(string fileName)
        {
            InitializeComponent();
            this.fileName = fileName;
        }


    }
}