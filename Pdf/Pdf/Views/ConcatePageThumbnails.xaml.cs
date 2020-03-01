using Pdf.Interfaces;
using Pdf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConcatePageThumbnails : ContentPage
    {
        private readonly FileInfo fileInfo;

        //TODO -- Gerer le retour 
        public ConcatePageThumbnails(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;

            IGetThumbnails getThumbnails = DependencyService.Get<IGetThumbnails>();

            string directoryPath = getThumbnails.GetBitmaps(fileInfo.FullName);
            
            CollectionViewThumbnails.ItemsSource = Directory.GetFiles(directoryPath);
        }


    }
}