using Pdf.ViewModels;
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
    public partial class PdfViewer : ContentPage
    {
        Stream fileStream;
        PdfViewerModel pdfViewerModel;
        public PdfViewer(Stream fileStream)
        {
            InitializeComponent();
            this.fileStream = fileStream;

            BindingContext = pdfViewerModel = new PdfViewerModel(fileStream);

            pdfViewerControl.Toolbar.Enabled = false;
        }

        void OnSizeChanged(object sender, EventArgs e)
        {
            if(Height > Width)
            {
                firstTopGridRow.Height = 50;
                thirdBottomGridRow.Height = 50;
            }
        }

        private void ShowToolsMenu(object sender, EventArgs e)
        {
            Grid grid = new Grid();
            ColumnDefinition c1 = new ColumnDefinition();
            ColumnDefinition c2 = new ColumnDefinition();
            c1.Width = 50;
            c2.Width = 50;

            RowDefinition r1 = new RowDefinition();
            r1.Height = 50;

            grid.ColumnDefinitions.Add(c1);
            grid.ColumnDefinitions.Add(c2);

            grid.RowDefinitions.Add(r1);

            ImageButton signaturePadButton = new ImageButton();
            signaturePadButton.Source = "drawing.png";
            ImageButton stampButton = new ImageButton();
            stampButton.Source = "stamp.png";

            grid.Children.Add(signaturePadButton);
            grid.Children.Add(stampButton);

            Grid.SetRow(signaturePadButton, 0);
            Grid.SetRow(stampButton, 0);
            Grid.SetColumn(signaturePadButton, 0);
            Grid.SetColumn(stampButton, 1);

            navigationDrawer.DrawerHeaderView = grid;

            navigationDrawer.ToggleDrawer();
        }
    }
}