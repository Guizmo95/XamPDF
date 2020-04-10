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
        private readonly Stream fileStream;
        private  PdfViewerModel pdfViewerModel;
        private  RotatorPage rotatorPage;
        private  NavigationDrawerToolsMenu navigationDrawerToolsMenu;
        private  TextButtonClickedBottomBar textButtonClickedBottomBar;
        private FontSizeSlider fontSizeSlider;

        private Color selectedColor = Color.Black;
        private int fontSize = 6;

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

        public int FontSize
        {
            get
            {
                return fontSize;
            }

            set
            {
                pdfViewerControl.AnnotationSettings.FreeText.TextSize = value;
                fontSize = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public PdfViewer(Stream fileStream)
        {
            InitializeComponent();

            #region Instanciation
            this.fileStream = fileStream;

            this.navigationDrawerToolsMenu = new NavigationDrawerToolsMenu();

            this.rotatorPage = new RotatorPage(RotatorMode.ColorPicker);

            this.textButtonClickedBottomBar = new TextButtonClickedBottomBar();
            this.textButtonClickedBottomBar.BindingContext = this;

            this.fontSizeSlider = new FontSizeSlider();
            this.fontSizeSlider.BindingContext = this;
            #endregion

            pdfViewerControl.FreeTextAnnotationAdded += PdfViewerControl_FreeTextAnnotationAdded;

            MessagingCenter.Subscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor", (sender, helper) =>
            {
                this.SelectedColor = helper;
            });

            rotatorPage.IsVisible = false;
            mainStackLayout.Children.Add(rotatorPage);
            fontSizeSlider.IsVisible = false;
            mainStackLayout.Children.Insert(3, fontSizeSlider);

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

        #region Pdf viewer events
        private void PdfViewerControl_FreeTextAnnotationAdded(object sender, FreeTextAnnotationAddedEventArgs args)
        {
            pdfViewerControl.AnnotationMode = AnnotationMode.FreeText;
        }
        #endregion

        //TODO -- Align stack layout
        //Show the tools menu with the navigation drawer
        private void ShowToolsMenu()
        {
            navigationDrawerToolsMenu.SignaturePadButton += async () => await SignaturePadButton_Clicked();
            navigationDrawerToolsMenu.StampButtonClicked +=  () => StampButton_Clicked();
            navigationDrawerToolsMenu.TextButtonClicked +=  () => TextButton_Clicked();

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

            navigationDrawer.DrawerContentView = navigationDrawerToolsMenu;
            navigationDrawer.DrawerContentView.BackgroundColor = Color.FromHex("#f5f5f5");

            navigationDrawer.DrawerHeaderView = navigationDrawerHeader;
            navigationDrawer.DrawerHeaderHeight = 35;

            //Toggle the bottom navigation drawer
            navigationDrawer.ToggleDrawer();
        }

        private void TextButton_Clicked()
        {
            navigationDrawer.ToggleDrawer();

            textButtonClickedBottomBar.ColorButtonClicked += ColorTextButton_Clicked;
            textButtonClickedBottomBar.FontSizeButtonClicked += FontSizeButton_Clicked;

            bottomToolbar.IsVisible = false;

            mainStackLayout.Children.Insert(2, textButtonClickedBottomBar);

            pdfViewerControl.AnnotationMode = AnnotationMode.FreeText;
        }

        #region Free text controls
        private void FontSizeButton_Clicked()
        {
            if(fontSizeSlider.IsVisible == true)
            {
                fontSizeSlider.IsVisible = false;
            }
            else
            {
                fontSizeSlider.IsVisible = true;
            }
        }

        private void ColorTextButton_Clicked()
        {
            if(rotatorPage.IsVisible == true)
            {
                fontSizeSlider.IsVisible = false;
            }
            else
            {
                rotatorPage.IsVisible = true;
            }
        }
        #endregion

        #region Tools menu controls
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

            //pdfViewerControl.AnnotationMode = AnnotationMode.HandwrittenSignature;

            var page = new SignaturePage();
            page.DidFinishPoping += (parameter) =>
            {
                //Set image source
                Image image = new Image();
                image.Source = ImageSource.FromFile(parameter);
                image.WidthRequest = 200;
                image.HeightRequest = 100;

                int numPage = pdfViewerControl.PageNumber;
                //Add image as custom stamp to the first page
                pdfViewerControl.AddStamp(image, numPage);

            };
            await Navigation.PushAsync(page);
        }
        #endregion

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}