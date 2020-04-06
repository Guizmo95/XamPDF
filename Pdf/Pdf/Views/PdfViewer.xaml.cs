using Pdf.ViewModels;
using Syncfusion.SfPdfViewer.XForms;
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

        //Set the toolbar when leaving the signature pad
        void OnSizeChanged(object sender, EventArgs e)
        {
            if(Height > Width)
            {
                //firstTopGridRow.Height = 50;
                //thirdBottomGridRow.Height = 50;
            }
        }

        //Show the tools menu with the navigation drawer
        private void ShowToolsMenu(object sender, EventArgs e)
        {
            StackLayout DrawerContentView = new StackLayout();
            StackLayout firstChildDrawerContentView = new StackLayout();
            DrawerContentView.BackgroundColor = Color.FromHex("#f5f5f5");
            firstChildDrawerContentView.Orientation = StackOrientation.Horizontal;
            firstChildDrawerContentView.HeightRequest = 45;
            firstChildDrawerContentView.Spacing = 15;
            firstChildDrawerContentView.Padding = 15;

            DrawerContentView.Children.Add(firstChildDrawerContentView);

            StackLayout signaturePadButton = new StackLayout();
            signaturePadButton.Orientation = StackOrientation.Vertical;
            signaturePadButton.HeightRequest = 4;
            signaturePadButton.WidthRequest = 36;

            Image signatureImage = new Image();
            signatureImage.Source = "fountain.png";
            signatureImage.Aspect = Aspect.AspectFit;
            signatureImage.VerticalOptions = LayoutOptions.Center;
            signatureImage.HorizontalOptions = LayoutOptions.Center;
            Label singatureLabel = new Label();
            singatureLabel.Text = "Signature";
            singatureLabel.VerticalOptions = LayoutOptions.Center;
            singatureLabel.HorizontalOptions = LayoutOptions.Center;
            singatureLabel.FontSize = 7;
            singatureLabel.VerticalTextAlignment = TextAlignment.Center;
            singatureLabel.HorizontalTextAlignment = TextAlignment.Center;

            signaturePadButton.Children.Add(signatureImage);
            signaturePadButton.Children.Add(singatureLabel);

            signaturePadButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => 
                {
                    signaturePadButton.BackgroundColor = Color.FromHex("#bcbcbc");
                    SignaturePadButton_Clicked();
                })
            });

            StackLayout stampButton = new StackLayout();
            stampButton.Orientation = StackOrientation.Vertical;
            stampButton.HeightRequest = 4;
            stampButton.WidthRequest = 36;

            Image stampImage = new Image();
            stampImage.Source = "licensing.png";
            stampImage.Aspect = Aspect.AspectFit;
            stampImage.VerticalOptions = LayoutOptions.Center;
            stampImage.HorizontalOptions = LayoutOptions.Center;
            Label stampLabel = new Label();
            stampLabel.Text = "Stamp";
            stampLabel.VerticalOptions = LayoutOptions.Center;
            stampLabel.HorizontalOptions = LayoutOptions.Center;
            stampLabel.FontSize = 7;
            stampLabel.VerticalTextAlignment = TextAlignment.Center;
            stampLabel.HorizontalTextAlignment = TextAlignment.Center;

            stampButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => 
                {
                    stampButton.BackgroundColor = Color.FromHex("#bcbcbc");
                    StampButton_Clicked();
                })
            });

            stampButton.Children.Add(stampImage);
            stampButton.Children.Add(stampLabel);

            firstChildDrawerContentView.Children.Add(signaturePadButton);
            firstChildDrawerContentView.Children.Add(stampButton);
            firstChildDrawerContentView.BackgroundColor = Color.FromHex("#f5f5f5");

            StackLayout navigationDrawerHeader = new StackLayout();
            navigationDrawerHeader.HorizontalOptions = LayoutOptions.FillAndExpand;
            navigationDrawerHeader.VerticalOptions = LayoutOptions.FillAndExpand;
            navigationDrawerHeader.BackgroundColor = Color.FromHex("#eeeeee");
            Label navigationDrawerHeaderLabel = new Label();
            navigationDrawerHeaderLabel.Text = "Customize";
            navigationDrawerHeaderLabel.FontSize = 16;
            navigationDrawerHeaderLabel.HorizontalOptions = LayoutOptions.Center;
            navigationDrawerHeaderLabel.VerticalOptions = LayoutOptions.CenterAndExpand;
            navigationDrawerHeaderLabel.VerticalTextAlignment = TextAlignment.Center;

            navigationDrawerHeader.Children.Add(navigationDrawerHeaderLabel);

            navigationDrawer.DrawerContentView = DrawerContentView;

            navigationDrawer.DrawerHeaderHeight = 35;
            navigationDrawer.DrawerHeaderView = navigationDrawerHeader;

            //Toggle the bottom navigation drawer
            navigationDrawer.ToggleDrawer();
        }

        private void StampButton_Clicked()
        {
            throw new NotImplementedException();
        }

        //Set the signature pad page
        private void SignaturePadButton_Clicked()
        {
            //Remove the toolbar
            //firstTopGridRow.Height = 0;
            //thirdBottomGridRow.Height = 0;

            pdfViewerControl.AnnotationMode = AnnotationMode.HandwrittenSignature;
        }
        
    }
}