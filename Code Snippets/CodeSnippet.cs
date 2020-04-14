using Pdf.controls;
using Pdf.Enumerations;
using Pdf.ViewModels;
using Syncfusion.SfPdfViewer.XForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;


namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfViewer : ContentPage, INotifyPropertyChanged
    {
        Stream fileStream;
        PdfViewerModel pdfViewerModel;
        RotatorPage rotatorPage;


        private Color selectedColor = Color.Black;


        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;


        public Color SelectedColor
        {
            get
            {
                return selectedColor;
            }


            set
            {
                pdfViewerControl.AnnotationSettings.FreeText.TextColor = value;
                selectedColor = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }


        public PdfViewer(Stream fileStream)
        {
            InitializeComponent();
            this.fileStream = fileStream;
            this.rotatorPage = new RotatorPage(RotatorMode.ColorPicker);


            MessagingCenter.Subscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor", (sender, helper) =>
            {
                this.SelectedColor = helper;
            });


            rotatorPage.IsVisible = false;
            mainStackLayout.Children.Add(rotatorPage);


            BindingContext = pdfViewerModel = new PdfViewerModel(fileStream);


            pdfViewerControl.Toolbar.Enabled = true;


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


        //TODO -- Align stack layout
        //Show the tools menu with the navigation drawer
        private void ShowToolsMenu()
        {
            #region Define the main grid
            Grid DrawerContentView = new Grid();
            DrawerContentView.RowSpacing = 0;
            RowDefinition r1 = new RowDefinition();
            RowDefinition r2 = new RowDefinition();
            RowDefinition r3 = new RowDefinition();
            r1.Height = new GridLength(90);
            r2.Height = new GridLength(17);
            r3.Height = new GridLength(90);


            DrawerContentView.RowDefinitions.Add(r1);
            DrawerContentView.RowDefinitions.Add(r2);
            DrawerContentView.RowDefinitions.Add(r3);
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


            #region Text button 
            //Text button
            StackLayout textButton = new StackLayout();
            textButton.Orientation = StackOrientation.Vertical;
            textButton.VerticalOptions = LayoutOptions.Center;
            textButton.HorizontalOptions = LayoutOptions.Center;
            textButton.HeightRequest = 45;
            textButton.WidthRequest = 50;
            textButton.Spacing = 3;


            //Text button content
            IconView textImage = new IconView();
            textImage.Source = "font.png";
            textImage.VerticalOptions = LayoutOptions.Center;
            textImage.HorizontalOptions = LayoutOptions.Center;
            textImage.HeightRequest = 30;
            textImage.WidthRequest = 50;
            textImage.Foreground = Color.FromHex("#4e4e4e");
            Label textLabel = new Label();
            textLabel.Text = "Text";
            textLabel.VerticalOptions = LayoutOptions.Center;
            textLabel.HorizontalOptions = LayoutOptions.Center;
            textLabel.FontSize = 8;
            textLabel.VerticalTextAlignment = TextAlignment.Center;
            textLabel.HorizontalTextAlignment = TextAlignment.Center;
            textLabel.TextColor = Color.Black;


            textButton.Children.Add(textImage);
            textButton.Children.Add(textLabel);


            textButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    textImage.Foreground = Color.FromHex("#b4b4b4");
                    textLabel.TextColor = Color.FromHex("#b4b4b4");
                    TextButton_Clicked();
                    await Task.Delay(100);
                    textImage.Foreground = Color.FromHex("#4e4e4e");
                    textLabel.TextColor = Color.Black;
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
            firstRow.Children.Add(signaturePadButton);
            firstRow.Children.Add(stampButton);


            firstRow.VerticalOptions = LayoutOptions.Center;


            Grid.SetRow(firstRow, 0);
            Grid.SetColumn(signaturePadButton, 0);
            Grid.SetColumn(stampButton, 1);
            #endregion


            #region Second grid row 
            Grid secondRow = new Grid();
            secondRow.BackgroundColor = Color.FromHex("#eeeeee");


            Grid.SetRow(secondRow, 1);
            #endregion


            #region Third grid row 
            Grid thirdRow = new Grid();
            thirdRow.Padding = new Thickness(15, 0, 15, 0);
            ColumnDefinition c3 = new ColumnDefinition();
            ColumnDefinition c4 = new ColumnDefinition();
            c3.Width = new GridLength(40);
            c4.Width = new GridLength(40);


            thirdRow.ColumnDefinitions.Add(c3);
            thirdRow.ColumnDefinitions.Add(c4);
            thirdRow.ColumnSpacing = 13;
            thirdRow.BackgroundColor = Color.FromHex("#f5f5f5");


            //Content in the first grid row
            thirdRow.Children.Add(textButton);


            thirdRow.VerticalOptions = LayoutOptions.Center;


            Grid.SetRow(thirdRow, 2);
            Grid.SetColumn(textButton, 0);
            #endregion


            //Add the first grid row to the main grid
            DrawerContentView.Children.Add(firstRow);
            //Add the second grid row to the main grid
            DrawerContentView.Children.Add(secondRow);
            //Add the third grid row to the main grid
            DrawerContentView.Children.Add(thirdRow);


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


        private void TextButton_Clicked()
        {
            navigationDrawer.ToggleDrawer();


            PancakeView pancakeView = new PancakeView();
            pancakeView.BackgroundColor = Color.Transparent;
            pancakeView.HorizontalOptions = LayoutOptions.FillAndExpand;
            pancakeView.HasShadow = true;


            AbsoluteLayout absoluteLayout = new AbsoluteLayout();
            absoluteLayout.VerticalOptions = LayoutOptions.End;
            absoluteLayout.BackgroundColor = Color.FromHex("f5f5f5");
            absoluteLayout.HeightRequest = 47;
            absoluteLayout.HorizontalOptions = LayoutOptions.FillAndExpand;


            Button colorButton = new Button();
            colorButton.CornerRadius = 20;
            colorButton.BorderWidth = 2;
            colorButton.HeightRequest = 30;
            colorButton.WidthRequest = 30;
            colorButton.BindingContext = this;
            colorButton.SetBinding(Button.BorderColorProperty, new Binding("SelectedColor"));
            colorButton.SetBinding(Button.BackgroundColorProperty, new Binding("SelectedColor"));


            IconView fontSizeButton = new IconView();
            fontSizeButton.Source = "text.png";
            fontSizeButton.VerticalOptions = LayoutOptions.Center;
            fontSizeButton.HorizontalOptions = LayoutOptions.Center;
            fontSizeButton.HeightRequest = 30;
            fontSizeButton.WidthRequest = 50;
            fontSizeButton.Foreground = Color.FromHex("#4e4e4e");


            fontSizeButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    fontSizeButton.Foreground = Color.FromHex("#b4b4b4");
                    FontSizeButton_Clicked();
                    await Task.Delay(100);
                    fontSizeButton.Foreground = Color.FromHex("#4e4e4e");
                })
            });


            colorButton.Clicked += ColorTextButton_Clicked;


            AbsoluteLayout.SetLayoutBounds(colorButton, new Rectangle(0.95, 0.5, -1, -1));
            AbsoluteLayout.SetLayoutFlags(colorButton, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(fontSizeButton, new Rectangle(0.85, 0.5, -1, -1));
            AbsoluteLayout.SetLayoutFlags(fontSizeButton, AbsoluteLayoutFlags.PositionProportional);
            absoluteLayout.Children.Add(colorButton);
            absoluteLayout.Children.Add(fontSizeButton);


            pancakeView.Content = absoluteLayout;


            bottomToolbar.IsVisible = false;


            mainStackLayout.Children.Insert(2, pancakeView);


            pdfViewerControl.AnnotationMode = AnnotationMode.FreeText;
        }


        private void FontSizeButton_Clicked()
        {
            FontSizeSlider fontSizeSlider = new FontSizeSlider();
            mainStackLayout.Children.Insert(3, fontSizeSlider);
        }


        private void ColorTextButton_Clicked(object sender, EventArgs e)
        {
            SelectedColor = Color.Black;


            rotatorPage.IsVisible = true;
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


            //topToolbar.IsVisible = false;
            //bottomToolbar.IsVisible = false;


            pdfViewerControl.AnnotationMode = AnnotationMode.HandwrittenSignature;


            //var page = new SignaturePage();
            //page.DidFinishPoping += (parameter) =>
            //{
            //    //Set image source
            //    Image image = new Image();
            //    image.Source = ImageSource.FromFile(parameter);
            //    image.WidthRequest = 200;
            //    image.HeightRequest = 100;


            //    int numPage = pdfViewerControl.PageNumber;
            //    //Add image as custom stamp to the first page
            //    pdfViewerControl.AddStamp(image, numPage);


            //};
            //await Navigation.PushAsync(page);
        }


        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
