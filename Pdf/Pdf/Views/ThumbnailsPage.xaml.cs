
using Acr.UserDialogs;
using Pdf.Interfaces;
using Syncfusion.SfPdfViewer.XForms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThumbnailsPage : ContentPage
    {
        Stream pdfStream;
        //SfPdfViewer pdfViewerControl;
        string filePath;


        public ObservableCollection<byte[]> ThumbnailInfoCollection { get; set; }
        public List<byte[]> ThumbnailBytes { get; set; }

        protected async override void OnAppearing()
        {
            //UserDialogs.Instance.ShowLoading();

            await Task.Run(() =>
            {
                pdfStream = DependencyService.Get<IAndroidFileHelper>().GetFileStream(filePath);

                // PDFium renderer
                pdfViewerControl.CustomPdfRenderer = DependencyService.Get<ICustomPdfRendererService>().AlternatePdfRenderer;

                pdfViewerControl.LoadDocument(pdfStream);
                //pdfStream.Close();
            });

            Device.BeginInvokeOnMainThread(() =>
            {
                base.OnAppearing();
            });
        }

        public ThumbnailsPage(string filePath)
        {
            InitializeComponent();

            pdfViewerControl.DocumentLoaded += PdfViewerControl_DocumentLoaded;

            this.filePath = filePath;

            ThumbnailBytes = new List<byte[]>();
            ThumbnailInfoCollection = new ObservableCollection<byte[]>();

            //pdfViewerControl = new SfPdfViewer();
        }

        private async void PdfViewerControl_DocumentLoaded(object sender, System.EventArgs args)
        {
            listView.ItemsSource = ThumbnailInfoCollection;

            Task.Run(async () =>
            {
                Stream exportedImage = null;
                for (int i = 0; i < pdfViewerControl.PageCount; i++)
                {
                    exportedImage = pdfViewerControl.ExportAsImageAsync(i, 0.7f).Result;

                    ConvertStreamToImageSource(exportedImage);
                }
            });



            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    UserDialogs.Instance.HideLoading();
            //});
        }

        private async Task ConvertStreamToImageSource(Stream imageStream)
        {
            imageStream.Position = 0;
            byte[] bytes = await ReadBytes(imageStream);
            ThumbnailInfoCollection.Add(bytes);
        }

        private async Task<byte[]> ReadBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await ms.WriteAsync(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public void UpdateImages()
        {
            if (ThumbnailBytes.Count != 0)
            {
                int i = 0;
                foreach (var thumbnail in ThumbnailBytes)
                {
                    i++;
                    ThumbnailInfoCollection.Add(thumbnail);
                }
            }
        }
    }
}