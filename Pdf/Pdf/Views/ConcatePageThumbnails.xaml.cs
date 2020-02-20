using Pdf.Models;
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
    public partial class ConcatePageThumbnails : ContentPage
    {
        public ConcatePageThumbnails()
        {
            InitializeComponent();

            List<Thumbnail> thumbnails = new List<Thumbnail>();
            thumbnails.Add(new Thumbnail { ImageUrl = "at.jpg" });
            thumbnails.Add(new Thumbnail { ImageUrl = "at.jpg" });
            thumbnails.Add(new Thumbnail { ImageUrl = "at.jpg" });
            thumbnails.Add(new Thumbnail { ImageUrl = "at.jpg" });

            CollectionViewThumbnails.ItemsSource = thumbnails;
        }

        
    }
}