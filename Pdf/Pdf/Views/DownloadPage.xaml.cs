using Pdf.Helpers;
using Pdf.Interfaces;
using Pdf.Models;
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
    public partial class DownloadPage : ContentPage
    {
        DownloadPageViewModel viewModel;

        public DownloadPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new DownloadPageViewModel();
        }
    }
}