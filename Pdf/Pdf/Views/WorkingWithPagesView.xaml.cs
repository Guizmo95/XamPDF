using Pdf.Helpers;
using Pdf.Interfaces;
using Pdf.ViewModels;
using Syncfusion.GridCommon.ScrollAxis;
using Syncfusion.ListView.XForms;
using Syncfusion.ListView.XForms.Control.Helpers;
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
    public partial class WorkingWithPagesView : ContentPage
    {
        private readonly ThumbnailsViewModel thumbnailsViewModel;
        private readonly FileInfo fileInfo;
        IList<model> list = new List<model>();
        public WorkingWithPagesView(FileInfo fileInfo)
        {
            InitializeComponent();

            this.fileInfo = fileInfo;
            this.thumbnailsViewModel = new ThumbnailsViewModel(fileInfo);

            VisualContainer visualContainer;
            visualContainer = ThumbnailsListView.GetVisualContainer();

            int numberOfPage = DependencyService.Get<IGetThumbnails>().GetAllPages(fileInfo.FullName);

            int i = 0;

            while (i < numberOfPage)
            {
                list.Add(new model { image = ImageSource.FromFile("award.png") });
                i++;
            }

            ThumbnailsListView.ItemsSource = list;

            var scrollview = ThumbnailsListView.GetScrollView();
            scrollview.Scrolled += (sender, e) => Scrollview_Scrolled(sender, e, visualContainer);
        }

        private void Scrollview_Scrolled(object sender, ScrolledEventArgs e, VisualContainer vs)
        {
            VisibleLinesCollection visibleLineInfos = vs.ScrollRows.GetVisibleLines();

            var first = visibleLineInfos.First();
            int firstIndex = first.LineIndex;

            int lastItemVisble = visibleLineInfos.Count;

            int i = firstIndex;
            while (i <= lastItemVisble)
            {
                MemoryStream stream = PdfToImage.FileToImage(fileInfo, i);

                byte[] files = stream.ToArray();

                var m = new model { image = ImageSource.FromStream(() => new MemoryStream(files)) };

                list[i] = m;

                i++;
            }
        }
    }

    public class model
    {
        public ImageSource image { get; set; }
    }
}