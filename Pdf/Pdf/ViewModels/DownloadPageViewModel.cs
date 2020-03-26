using Pdf.Helpers;
using Pdf.Interfaces;
using Pdf.Models;
using Pdf.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pdf.ViewModels
{
    public class DownloadPageViewModel:BaseViewModel
    {
        private ObservableCollection<ProgressItem> items;
        public ObservableCollection<ProgressItem> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }

        public DownloadPageViewModel()
        {
            Items = new ObservableCollection<ProgressItem>();
        } 
    }
}
