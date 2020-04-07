using Pdf.controls;
using Pdf.ViewModels;
using Syncfusion.SfPdfViewer.XForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
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

            ToolsButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async() =>
                {
                    ToolsButton.Foreground = Color.FromHex("#b4b4b4");
                    ShowToolsMenu();
                    await Task.Delay(100);
                    ToolsButton.Foreground = Color.FromHex("4e4e4e");
                })
            });
        }

        //Set the toolbar when leaving the signature pad
        void OnSizeChanged(object sender, EventArgs e)
        {
            if(Height > Width)
            {
                //Add the toolbar
                topToolbar.IsVisible = true;
                bottomToolbar.IsVisible = true;
            }
        }

        //TODO -- Align stack layout
        //Show the tools menu with the navigation drawer
        private void ShowToolsMenu()
        {
            #region Define the main grid
            Grid DrawerContentView = new Grid();
            RowDefinition r1 = new RowDefinition();
            RowDefinition r2 = new RowDefinition();
            r1.Height = new GridLength(90);
            r2.Height = new GridLength(11);

            DrawerContentView.RowDefinitions.Add(r1);
            DrawerContentView.RowDefinitions.Add(r2);
            #endregion

            #region Signature button
            //Signature button
            StackLayout signaturePadButton = new StackLayout();
            signaturePadButton.Orientation = StackOrientation.Vertical;
            signaturePadButton.VerticalOptions = LayoutOptions.Center;
            signaturePadButton.HorizontalOptions = LayoutOptions.Center;
            signaturePadButton.HeightRequest = 45;
            signaturePadButton.WidthRequest = 50;
            signaturePadButton.Spacing = 3;


            IconView signatureImage = new IconView();
            signatureImage.Source = "fountain.png";
            signatureImage.VerticalOptions = LayoutOptions.Center;
            signatureImage.HorizontalOptions = LayoutOptions.Center;
            signatureImage.HeightRequest = 30;
            signatureImage.WidthRequest = 50;
            signatureImage.Foreground = Color.FromHex("#4e4e4e");
            Label singatureLabel = new Label();
            singatureLabel.Text = "Signature";
            singatureLabel.VerticalOptions = LayoutOptions.Center;
            singatureLabel.HorizontalOptions = LayoutOptions.Center;
            singatureLabel.FontSize = 8;
            singatureLabel.VerticalTextAlignment = TextAlignment.Center;
            singatureLabel.HorizontalTextAlignment = TextAlignment.Center;
            singatureLabel.TextColor = Color.Black;

            signaturePadButton.Children.Add(signatureImage);
            signaturePadButton.Children.Add(singatureLabel);

            //Frame signature button
            PancakeView pancakeViewSignatureButton = new PancakeView();
            pancakeViewSignatureButton.CornerRadius = 5;
            pancakeViewSignatureButton.HorizontalOptions = LayoutOptions.Center;
            pancakeViewSignatureButton.VerticalOptions = LayoutOptions.Center;
            pancakeViewSignatureButton.Padding = 0;
            pancakeViewSignatureButton.HeightRequest = 45;
            pancakeViewSignatureButton.WidthRequest = 50;
            pancakeViewSignatureButton.Content = signaturePadButton;

            signaturePadButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    signatureImage.Foreground = Color.FromHex("#b4b4b4");
                    singatureLabel.TextColor = Color.FromHex("#b4b4b4");
                    await SignaturePadButton_Clicked();
                    await Task.Delay(100);
                    signatureImage.Foreground = Color.FromHex("#4e4e4e");
                    singatureLabel.TextColor = Color.Black;
                })
            });
            #endregion

            #region Stamp button
            //Stamp button
            StackLayout stampButton = new StackLayout();
            stampButton.Orientation = StackOrientation.Vertical;
            stampButton.VerticalOptions = LayoutOptions.Center;
            stampButton.HorizontalOptions = LayoutOptions.Center;
            stampButton.HeightRequest = 45;
            stampButton.WidthRequest = 50;
            stampButton.Spacing = 3;

            //Stamp button content
            IconView stampImage = new IconView();
            stampImage.Source = "licensing.png";
            stampImage.VerticalOptions = LayoutOptions.Center;
            stampImage.HorizontalOptions = LayoutOptions.Center;
            stampImage.HeightRequest = 30;
            stampImage.WidthRequest = 50;
            stampImage.Foreground = Color.FromHex("#4e4e4e");
            Label stampLabel = new Label();
            stampLabel.Text = "Stamp";
            stampLabel.VerticalOptions = LayoutOptions.Center;
            stampLabel.HorizontalOptions = LayoutOptions.Center;
            stampLabel.FontSize = 8;
            stampLabel.VerticalTextAlignment = TextAlignment.Center;
            stampLabel.HorizontalTextAlignment = TextAlignment.Center;
            stampLabel.TextColor = Color.Black;

            stampButton.Children.Add(stampImage);
            stampButton.Children.Add(stampLabel);

            //Frame stamp button
            PancakeView pancakeViewStampButton = new PancakeView();
            pancakeViewStampButton.CornerRadius = 5;
            pancakeViewStampButton.HorizontalOptions = LayoutOptions.Center;
            pancakeViewStampButton.VerticalOptions = LayoutOptions.Center;
            pancakeViewStampButton.Padding = 0;
            pancakeViewStampButton.HeightRequest = 45;
            pancakeViewStampButton.WidthRequest = 50;
            pancakeViewStampButton.Content = stampButton;

            stampButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    stampImage.Foreground = Color.FromHex("#b4b4b4");
                    stampLabel.TextColor = Color.FromHex("#b4b4b4");
                    //StampButton_Clicked();
                    await Task.Delay(100);
                    stampImage.Foreground = Color.FromHex("#4e4e4e");
                    stampLabel.TextColor = Color.Black;
                })
            });
            #endregion

            #region First grid row 
            Grid firstRow = new Grid();
            firstRow.Padding = new Thickness(15, 0, 15, 0);
            ColumnDefinition c1 = new ColumnDefinition();
            ColumnDefinition c2 = new ColumnDefinition();
            c1.Width = new GridLength(40);
            c2.Width = new GridLength(40);

            firstRow.ColumnDefinitions.Add(c1);
            firstRow.ColumnDefinitions.Add(c2);
            firstRow.ColumnSpacing = 13;
            firstRow.BackgroundColor = Color.FromHex("#f5f5f5");

            //Content in the first grid row
            firstRow.Children.Add(pancakeViewSignatureButton);
            firstRow.Children.Add(pancakeViewStampButton);

            Grid.SetRow(firstRow, 0);
            Grid.SetColumn(pancakeViewSignatureButton, 0);
            Grid.SetColumn(pancakeViewStampButton, 1);
            #endregion

            //Add the first grid row to the main grid
            DrawerContentView.Children.Add(firstRow);

            //StackLayout secondChildDrawerContentView = new StackLayout();
            //secondChildDrawerContentView.Orientation = StackOrientation.Horizontal;
            //secondChildDrawerContentView.HeightRequest = 11;
            //secondChildDrawerContentView.BackgroundColor = Color.FromHex("#eeeeee");

            //DrawerContentView.Children.Add(firstChildDrawerContentView);
            //DrawerContentView.Children.Add(secondChildDrawerContentView);

            #region Navigation drawer header
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
            #endregion

            navigationDrawer.DrawerContentView = DrawerContentView;
            navigationDrawer.DrawerContentView.BackgroundColor = Color.FromHex("#f5f5f5");

            navigationDrawer.DrawerHeaderView = navigationDrawerHeader;
            navigationDrawer.DrawerHeaderHeight = 35;

            //Toggle the bottom navigation drawer
            navigationDrawer.ToggleDrawer();
        }

        private void StampButton_Clicked()
        {
            throw new NotImplementedException();
        }

        //Set the signature pad page
        private async Task SignaturePadButton_Clicked()
        {
            //Toggle the bottom navigation drawer
            navigationDrawer.ToggleDrawer();

            await Navigation.PushAsync(new SignaturePage());
        }
        
    }
}