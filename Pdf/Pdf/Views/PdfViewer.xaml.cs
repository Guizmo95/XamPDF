using Android.Content;
using Android.Views;
using Pdf.controls;
using Pdf.Enumerations;
using Pdf.Interfaces;
using Pdf.Models;
using Pdf.ViewModels;
using Syncfusion.Pdf.Parsing;
using Syncfusion.SfPdfViewer.XForms;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfViewer : ContentPage, INotifyPropertyChanged
    {
        //#region Content view / View
        private string filePath;
        private PdfViewerModel pdfViewerModel;
        private SfPopupLayout stylePopup;
        private StyleContent styleContent;
        private DataTemplate styleTemplate;

        private FreeTextAnnotation selectedFreeTextAnnotation;
        private InkAnnotation selectedInkAnnotation;
        private ShapeAnnotation selectedShapeAnnotation;
        private TextMarkupAnnotation selectedTextMarkupAnnotation;

        private AnnotationType annotationType;

        #region Property
        private Color selectedColor = Color.Black;
        private Rectangle lastRectangleBounds;
        private AnnotationMode lastTextMarkupAnnotationMode;

        private int fontSize = 6;
        private bool canUndoInk = false;
        private bool canRedoInk = false;
        private int shapesNumbers = 0;
        private int textMarkupNumbers = 0;
        private int lastThicknessBarSelected = 5;
        private int lastOpacitySelected = 4;

        public event PropertyChangedEventHandler PropertyChanged;

        public SfPdfViewer PdfViewerControl
        {
            get
            {
                return pdfViewerControl;
            }
            set
            {
                pdfViewerControl = value;
            }
        }

        public Color SelectedColor
        {
            get
            {
                return selectedColor;
            }

            set
            {
                selectedColor = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();

                if (selectedFreeTextAnnotation != null)
                    selectedFreeTextAnnotation.Settings.TextColor = value;

                if (selectedInkAnnotation != null)
                    selectedInkAnnotation.Settings.Color = value;

                if (selectedShapeAnnotation != null)
                    selectedShapeAnnotation.Settings.StrokeColor = value;

                if (selectedTextMarkupAnnotation != null)
                    selectedTextMarkupAnnotation.Settings.Color = value;

                switch (pdfViewerControl.AnnotationMode)
                {
                    case AnnotationMode.None:
                        break;
                    case AnnotationMode.Highlight:
                        pdfViewerControl.AnnotationSettings.TextMarkup.Highlight.Color = value;
                        break;
                    case AnnotationMode.Underline:
                        pdfViewerControl.AnnotationSettings.TextMarkup.Underline.Color = value;
                        break;
                    case AnnotationMode.Ink:
                        pdfViewerControl.AnnotationSettings.Ink.Color = value;
                        break;
                    case AnnotationMode.Strikethrough:
                        pdfViewerControl.AnnotationSettings.TextMarkup.Strikethrough.Color = value;
                        break;
                    case AnnotationMode.FreeText:
                        pdfViewerControl.AnnotationSettings.FreeText.TextColor = value;
                        break;
                    case AnnotationMode.Rectangle:
                        pdfViewerControl.AnnotationSettings.Rectangle.Settings.StrokeColor = value;
                        break;
                    case AnnotationMode.Circle:
                        pdfViewerControl.AnnotationSettings.Circle.Settings.StrokeColor = value;
                        break;
                    case AnnotationMode.Line:
                        pdfViewerControl.AnnotationSettings.Line.Settings.StrokeColor = value;
                        break;
                    case AnnotationMode.Arrow:
                        pdfViewerControl.AnnotationSettings.Arrow.Settings.StrokeColor = value;
                        break;
                    case AnnotationMode.HandwrittenSignature:
                        break;
                    default:
                        break;
                }
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
                fontSize = value;
                OnPropertyChanged();

                if (selectedFreeTextAnnotation != null)
                    selectedFreeTextAnnotation.Settings.TextSize = value;

                if (pdfViewerControl.AnnotationMode == AnnotationMode.FreeText)
                    pdfViewerControl.AnnotationSettings.FreeText.TextSize = value;
            }
        }

        public bool CanUndoInk
        {
            get
            {
                return canUndoInk;
            }

            set
            {
                canUndoInk = value;
                OnPropertyChanged();

                if (value == true)
                {
                    UndoButton.Foreground = Color.Black;
                    ValidButton.Foreground = Color.Black;
                }
                else
                {
                    if (value == false)
                    {
                        UndoButton.Foreground = Color.White;
                        ValidButton.Foreground = Color.White;
                    }
                }
            }
        }

        public bool CanRedoInk
        {
            get
            {
                return canRedoInk;
            }

            set
            {
                canRedoInk = value;
                OnPropertyChanged();

                if (value == true)
                    RedoButton.Foreground = Color.Black;
                else
                {
                    if (value == false)
                        RedoButton.Foreground = Color.White;
                }
            }
        }

        public int ShapesNumbers
        {
            get
            {
                return shapesNumbers;
            }

            set
            {
                shapesNumbers = value;
                OnPropertyChanged();

                if (ShapesNumbers == 0)
                    ValidButton.Foreground = Color.White;
                else
                    ValidButton.Foreground = Color.Black;
            }
        }

        public int TextMarkupNumbers
        {
            get
            {
                return textMarkupNumbers;
            }

            set
            {
                textMarkupNumbers = value;
                OnPropertyChanged();

                if (TextMarkupNumbers == 0)
                    ValidButton.Foreground = Color.White;
                else
                    ValidButton.Foreground = Color.Black;
            }
        }

        #endregion

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor");

            pdfViewerControl.FreeTextAnnotationAdded -= PdfViewerControl_FreeTextAnnotationAdded;
            pdfViewerControl.FreeTextAnnotationSelected -= PdfViewerControl_FreeTextAnnotationSelected;
            pdfViewerControl.FreeTextAnnotationDeselected -= PdfViewerControl_FreeTextAnnotationDeselected;

            pdfViewerControl.CanRedoInkModified -= PdfViewerControl_CanRedoInkModified;
            pdfViewerControl.CanUndoInkModified -= PdfViewerControl_CanUndoInkModified;

            pdfViewerControl.InkSelected -= PdfViewerControl_InkSelected;
            pdfViewerControl.InkDeselected -= PdfViewerControl_InkDeselected;

            pdfViewerControl.ShapeAnnotationAdded -= PdfViewerControl_ShapeAnnotationAdded;
            pdfViewerControl.ShapeAnnotationSelected -= PdfViewerControl_ShapeAnnotationSelected;
            pdfViewerControl.ShapeAnnotationDeselected -= PdfViewerControl_ShapeAnnotationDeselected;

            pdfViewerControl.TextMarkupSelected -= PdfViewerControl_TextMarkupSelected;
            pdfViewerControl.TextMarkupDeselected -= PdfViewerControl_TextMarkupDeselected;
            pdfViewerControl.TextMarkupAdded -= PdfViewerControl_TextMarkupAdded;

            pdfViewerControl.DocumentLoaded -= PdfViewerControl_DocumentLoaded;

            pdfViewerControl.TextMarkupAdded -= PdfViewerControl_TextMarkupAdded;
            pdfViewerControl.PageChanged -= PdfViewerControl_PageChanged;

            styleContent.ThicknessBar.BoxViewButtonClicked -= ThicknessBar_Clicked;
            styleContent.OpacityButtonClicked -= OpacityIcon_Clicked;
            //pdfViewerControl.Unload();
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            pdfViewerControl.DocumentLoaded += PdfViewerControl_DocumentLoaded;

            var pdfStream = DependencyService.Get<IAndroidFileHelper>().GetFileStream(filePath);
            //pdfViewerControl.CustomPdfRenderer = DependencyService.Get<ICustomPdfRendererService>().AlternatePdfRenderer;

            pdfViewerControl.LoadDocument(pdfStream);

            MessagingCenter.Subscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor", (sender, helper) =>
            {
                this.SelectedColor = helper;
            });

            base.OnAppearing();
        }

        public PdfViewer(string filePath)
        {
            InitializeComponent();

            this.filePath = filePath;
            this.styleContent = new StyleContent();
            pdfViewerControl.Toolbar.Enabled = false;

            //pdfViewerControl.CustomPdfRenderer = DependencyService.Get<ICustomPdfRendererService>().AlternatePdfRenderer;
            //pdfViewerControl.BindingContext = pdfViewerModel = new PdfViewerModel(filepath);
        }

        private void PdfViewerControl_DocumentLoaded(object sender, EventArgs args)
        {
            Shell.SetNavBarIsVisible(this, false);
            Shell.SetTabBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);

            #region Pdf viewer events
            pdfViewerControl.FreeTextAnnotationAdded += PdfViewerControl_FreeTextAnnotationAdded;
            pdfViewerControl.FreeTextAnnotationSelected += PdfViewerControl_FreeTextAnnotationSelected;
            pdfViewerControl.FreeTextAnnotationDeselected += PdfViewerControl_FreeTextAnnotationDeselected;

            pdfViewerControl.CanRedoInkModified += PdfViewerControl_CanRedoInkModified;
            pdfViewerControl.CanUndoInkModified += PdfViewerControl_CanUndoInkModified;

            pdfViewerControl.InkSelected += PdfViewerControl_InkSelected;
            pdfViewerControl.InkDeselected += PdfViewerControl_InkDeselected;

            pdfViewerControl.ShapeAnnotationAdded += PdfViewerControl_ShapeAnnotationAdded;
            pdfViewerControl.ShapeAnnotationSelected += PdfViewerControl_ShapeAnnotationSelected;
            pdfViewerControl.ShapeAnnotationDeselected += PdfViewerControl_ShapeAnnotationDeselected;

            pdfViewerControl.TextMarkupSelected += PdfViewerControl_TextMarkupSelected;
            pdfViewerControl.TextMarkupDeselected += PdfViewerControl_TextMarkupDeselected;
            pdfViewerControl.TextMarkupAdded += PdfViewerControl_TextMarkupAdded;

            pdfViewerControl.PageChanged += PdfViewerControl_PageChanged;
            PdfViewerControl.Tapped += PdfViewerControl_Tapped;
            #endregion

            RedoButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if (CanRedoInk == true)
                    {
                        RedoButton.Foreground = Color.FromHex("#b4b4b4");
                        RedoInk();
                        await Task.Delay(100);
                        if (CanRedoInk == true)
                            RedoButton.Foreground = Color.Black;
                        else
                        {
                            RedoButton.Foreground = Color.White;
                        }
                    }
                })
            });

            UndoButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if (CanUndoInk == true)
                    {
                        UndoButton.Foreground = Color.FromHex("#b4b4b4");
                        UndoInk();
                        await Task.Delay(100);

                        if (CanUndoInk == true)
                            UndoButton.Foreground = Color.Black;
                        else
                        {
                            UndoButton.Foreground = Color.White;
                        }
                    }
                })
            });

            ValidButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if (CanUndoInk == true)
                    {
                        ValidButton.Foreground = Color.White;
                        RedoButton.Foreground = Color.White;
                        UndoButton.Foreground = Color.White;

                        if (pdfViewerControl.AnnotationMode == AnnotationMode.Ink)
                            SaveInk();

                        pdfViewerControl.AnnotationMode = AnnotationMode.None;

                        await Task.Delay(100);

                        CanRedoInk = false;
                        CanUndoInk = false;
                    }

                    if (pdfViewerControl.AnnotationMode == AnnotationMode.Arrow
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Line
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Circle
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Rectangle)
                    {
                        pdfViewerControl.AnnotationMode = AnnotationMode.None;

                        ValidButton.Foreground = Color.White;
                    }
                    else
                    {
                        if (pdfViewerControl.AnnotationMode == AnnotationMode.Strikethrough
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Highlight
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Underline)
                        {
                            pdfViewerControl.AnnotationMode = AnnotationMode.None;

                            ValidButton.Foreground = Color.White;
                        }
                    }
                })
            });

            paletteButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command( () =>
                {
                    PaletteButton_Clicked();
                })
            });

            paletteButton.BindingContext = this;
            styleContent.BindingContext = this;

            stylePopup = new SfPopupLayout();
            
            styleContent.ThicknessBar.BindingContext = this;
            stylePopup.ClosePopupOnBackButtonPressed = false;
            stylePopup.PopupView.ShowHeader = false;
            stylePopup.PopupView.ShowFooter = false;
            stylePopup.PopupView.HeightRequest = 190;
            stylePopup.PopupView.WidthRequest = 280;
            stylePopup.PopupView.PopupStyle.BorderColor = Color.FromHex("#fafafa");
            stylePopup.PopupView.AnimationMode = AnimationMode.Fade;

            styleTemplate = new DataTemplate(() =>
            {
                return styleContent;
            });

            this.stylePopup.PopupView.ContentTemplate = styleTemplate;

            styleContent.ThicknessBar.BoxViewButtonClicked += (int numberOfThicknessBarClicked) => ThicknessBar_Clicked(numberOfThicknessBarClicked);
            styleContent.OpacityButtonClicked += (int numberOfTheOpacityClicked) => OpacityIcon_Clicked(numberOfTheOpacityClicked);

            annotationType = AnnotationType.None;
        }

        private async void PdfViewerControl_Tapped(object sender, TouchInteractionEventArgs e)
        {
            
        }

        private void PdfViewerControl_PageChanged(object sender, PageChangedEventArgs args)
        {
            switch (annotationType)
            {
                case AnnotationType.Ink:
                    pdfViewerControl.AnnotationMode = AnnotationMode.Ink;
                    break;
                case AnnotationType.FreeText:
                    break;
                case AnnotationType.Rectangle:
                    pdfViewerControl.AnnotationMode = AnnotationMode.Rectangle;
                    break;
                case AnnotationType.Line:
                    pdfViewerControl.AnnotationMode = AnnotationMode.Line;
                    break;
                case AnnotationType.Arrow:
                    pdfViewerControl.AnnotationMode = AnnotationMode.Arrow;
                    break;
                case AnnotationType.Circle:
                    pdfViewerControl.AnnotationMode = AnnotationMode.Circle;
                    break;
                case AnnotationType.Hightlight:
                    pdfViewerControl.AnnotationMode = AnnotationMode.Highlight;
                    break;
                case AnnotationType.Underline:
                    pdfViewerControl.AnnotationMode = AnnotationMode.Underline;
                    break;
                case AnnotationType.Strikethrought:
                    pdfViewerControl.AnnotationMode = AnnotationMode.Strikethrough;
                    break;
                case AnnotationType.None:
                    break;
                default:
                    break;
            }
        }

        #region Pdf viewer events methods
        //Set the signature pad page
        //private async Task SignaturePadButton_Clicked()
        //{
        //    topToolbar.IsVisible = false;
        //    bottomGrid.IsVisible = false;

        //    //pdfViewerControl.AnnotationMode = AnnotationMode.HandwrittenSignature;

        //    var page = new SignaturePage();
        //    page.DidFinishPoping += (parameter) =>
        //    {
        //        if (parameter != null)
        //        {
        //            //Set image source
        //            Image image = new Image();
        //            image.Source = ImageSource.FromFile(parameter);
        //            image.WidthRequest = 200;
        //            image.HeightRequest = 100;

        //            int numPage = pdfViewerControl.PageNumber;
        //            //Add image as custom stamp to the first page
        //            pdfViewerControl.AddStamp(image, numPage);

        //            topToolbar.IsVisible = true;
        //            bottomGrid.IsVisible = true;
        //        }
        //        else
        //        {
        //            topToolbar.IsVisible = true;
        //            bottomGrid.IsVisible = true;
        //        }
        //    };
        //    await Navigation.PushAsync(page);
        //}

        private void StampButton_Clicked()
        {
            throw new NotImplementedException();
        }

        //private void AnnotationButton_Clicked(AnnotationType annotationType)
        //{
        //    bottomGrid.IsVisible = false;

        //    switch (annotationType)
        //    {
        //        case AnnotationType.Ink:
        //            bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";
        //            bottomAnnotationToolbar.IsVisible = true;

        //            ValidButton.IsVisible = true;
        //            UndoButton.IsVisible = true;
        //            RedoButton.IsVisible = true;

        //            this.SelectedColor = Color.Black;

        //            this.shapeBar = new ShapeBar();
        //            break;
        //        case AnnotationType.FreeText:
        //            //bottomAnnotationToolbar.AnnotationImage.Source = "font.png";
        //            //bottomAnnotationToolbar.IsVisible = true;

        //            this.SelectedColor = Color.Black;
        //            break;
        //        case AnnotationType.Shape:
        //            //bottomAnnotationToolbar.AnnotationImage.Source = "rectangle.png";

        //            //shapeBar.IsVisible = true;
        //            //mainGrid.Children.Insert(2, shapeBar);

        //            //Not like free text and ink
        //            #region Shape bottom bar events
        //            shapeBar.ArrowButtonClicked += ArrowButton_Clicked;
        //            shapeBar.LineButtonClicked += LineButton_Clicked;
        //            shapeBar.EllipseButtonClicked += EllipseButton_Clicked;
        //            shapeBar.RectangleButtonClicked += RectangleButton_Clicked;
        //            shapeBar.BackButtonClicked += BackShapeButton_Clicked;
        //            #endregion

        //            break;
        //        case AnnotationType.TextMarkup:
        //            bottomAnnotationToolbar.AnnotationImage.Source = "marker.png";

        //            textMarkupToolbar.HeightRequest = 0;
        //            textMarkupToolbar.IsVisible = true;
        //            textMarkupToolbar.LayoutTo(new Rectangle(textMarkupToolbar.Bounds.X, textMarkupToolbar.Bounds.Y, textMarkupToolbar.Bounds.Width, 45), 500, Easing.CubicOut);

        //            #region Text markup choice bar events
        //            //selectTextMarkupBar.StrikethroughtButtonClicked += StriketroughtButton_Clicked;
        //            //selectTextMarkupBar.HightlightButtonClicked += HightlightButton_Clicked;
        //            //selectTextMarkupBar.UnderlineButtonClicked += UnderlineButton_Clicked;
        //            //selectTextMarkupBar.BackButtonClicked += BackSelectTextMarkupButton_Clicked;
        //            #endregion
        //            break;
        //        default:
        //            break;
        //    }
        //}

        private void PdfViewerControl_TextMarkupAdded(object sender, TextMarkupAddedEventArgs args)
        {
            this.TextMarkupNumbers += 1;
        }

        private void PdfViewerControl_TextMarkupDeselected(object sender, TextMarkupDeselectedEventArgs args)
        {
            this.annotationType = AnnotationType.None;

        }

        private void PdfViewerControl_TextMarkupSelected(object sender, TextMarkupSelectedEventArgs args)
        {
            this.annotationType = AnnotationType.TextMarkup;

            selectedTextMarkupAnnotation = sender as TextMarkupAnnotation;
        }

        private void PdfViewerControl_ShapeAnnotationDeselected(object sender, ShapeAnnotationDeselectedEventArgs args)
        {
            this.annotationType = AnnotationType.None;

        }

        //TODO -- FOUND A WAY TO DESELECT OR HANDLE THE BOTTOM BAR BUG
        private void PdfViewerControl_ShapeAnnotationSelected(object sender, ShapeAnnotationSelectedEventArgs args)
        {
            //this.annotationType = AnnotationType.Shape;

            //Cast the sender object as shape annotation.
            this.selectedShapeAnnotation = sender as ShapeAnnotation;
        }

        private void PdfViewerControl_ShapeAnnotationAdded(object sender, ShapeAnnotationAddedEventArgs args)
        {
            this.ShapesNumbers += 1;
        }

        private void PdfViewerControl_FreeTextAnnotationDeselected(object sender, FreeTextAnnotationDeselectedEventArgs args)
        {
            this.annotationType = AnnotationType.None;

        }

        private void PdfViewerControl_FreeTextAnnotationSelected(object sender, FreeTextAnnotationSelectedEventArgs args)
        {
            this.annotationType = AnnotationType.FreeText;

            //Cast the sender object to FreeTextAnnotation
            this.selectedFreeTextAnnotation = sender as FreeTextAnnotation;
        }

        private void PdfViewerControl_FreeTextAnnotationAdded(object sender, FreeTextAnnotationAddedEventArgs args)
        {

        }

        private void PdfViewerControl_CanUndoInkModified(object sender, CanUndoInkModifiedEventArgs args)
        {
            CanUndoInk = args.CanUndoInk;
        }

        private void PdfViewerControl_CanRedoInkModified(object sender, CanRedoInkModifiedEventArgs args)
        {
            CanRedoInk = args.CanRedoInk;
        }

        private void UndoInk()
        {
            if (CanUndoInk == true)
                pdfViewerControl.UndoInk();
        }

        private void RedoInk()
        {
            if (CanRedoInk == true)
                pdfViewerControl.RedoInk();
        }

        private void SaveInk()
        {
            //bottomAnnotationToolbar.IsVisible = false;

            //bottomLayout.IsVisible = true;

            //pdfViewerControl.EndInkSession(true);
        }

        private void PdfViewerControl_InkSelected(object sender, InkSelectedEventArgs args)
        {
            this.annotationType = AnnotationType.Ink;
            //Casts the sender object as Ink annotation.
            selectedInkAnnotation = sender as InkAnnotation;
        }

        private void PdfViewerControl_InkDeselected(object sender, InkDeselectedEventArgs args)
        {
            this.annotationType = AnnotationType.None;
        }
        #endregion

        #region Shape events methods

        //private void ArrowButton_Clicked()
        //{
        //    shapeBar.IsVisible = false;

        //    if (bottomAnnotationToolbar.IsVisible == false)
        //        bottomAnnotationToolbar.IsVisible = true;

        //    //Set Annotation Image
        //    bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

        //    ValidButton.IsVisible = true;

        //    pdfViewerControl.AnnotationMode = AnnotationMode.Arrow;

        //    this.SelectedColor = Color.Black;
        //}

        //private void LineButton_Clicked()
        //{
        //    shapeBar.IsVisible = false;

        //    if (bottomAnnotationToolbar.IsVisible == false)
        //        bottomAnnotationToolbar.IsVisible = true;

        //    //Set Annotation Image
        //    bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

        //    ValidButton.IsVisible = true;

        //    pdfViewerControl.AnnotationMode = AnnotationMode.Line;

        //    this.SelectedColor = Color.Black;
        //}

        //private void EllipseButton_Clicked()
        //{
        //    shapeBar.IsVisible = false;

        //    if (bottomAnnotationToolbar.IsVisible == false)
        //        bottomAnnotationToolbar.IsVisible = true;

        //    //Set Annotation Image
        //    bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

        //    ValidButton.IsVisible = true;

        //    pdfViewerControl.AnnotationMode = AnnotationMode.Circle;

        //    this.SelectedColor = Color.Black;
        //}

        //private void RectangleButton_Clicked()
        //{
        //    shapeBar.IsVisible = false;

        //    if (bottomAnnotationToolbar.IsVisible == false)
        //        bottomAnnotationToolbar.IsVisible = true;

        //    //Set Annotation Image
        //    bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

        //    ValidButton.IsVisible = true;

        //    pdfViewerControl.AnnotationMode = AnnotationMode.Rectangle;

        //    this.SelectedColor = Color.Black;
        //}

        //private void BackShapeButton_Clicked()
        //{
        //    shapeBar.IsVisible = false;

        //    bottomLayout.IsVisible = true;

        //    pdfViewerControl.AnnotationMode = AnnotationMode.None;

        //    //Not like free text and ink
        //    #region Shape bottom bar events
        //    shapeBar.ArrowButtonClicked -= ArrowButton_Clicked;
        //    shapeBar.LineButtonClicked -= LineButton_Clicked;
        //    shapeBar.EllipseButtonClicked -= EllipseButton_Clicked;
        //    shapeBar.RectangleButtonClicked -= RectangleButton_Clicked;
        //    shapeBar.BackButtonClicked -= BackShapeButton_Clicked;
        //    #endregion
        //}
        #endregion

        #region Select Text markup events methods

        //private void StriketroughtButton_Clicked()
        //{
        //    if (bottomAnnotationToolbar.IsVisible == false)
        //        bottomAnnotationToolbar.IsVisible = true;

        //    //Set Annotation Image
        //    bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

        //    pdfViewerControl.AnnotationMode = AnnotationMode.Strikethrough;

        //    this.SelectedColor = Color.Black;

        //    //selectTextMarkupBar.IsVisible = false;

        //    ValidButton.IsVisible = true;
        //}

        //private void UnderlineButton_Clicked()
        //{
        //    if (bottomAnnotationToolbar.IsVisible == false)
        //        bottomAnnotationToolbar.IsVisible = true;

        //    //Set Annotation Image
        //    bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

        //    pdfViewerControl.AnnotationMode = AnnotationMode.Underline;

        //    this.SelectedColor = Color.Black;

        //    //selectTextMarkupBar.IsVisible = false;

        //    ValidButton.IsVisible = true;
        //}

        //private void HightlightButton_Clicked()
        //{
        //    if (bottomAnnotationToolbar.IsVisible == false)
        //        bottomAnnotationToolbar.IsVisible = true;

        //    //Set Annotation Image
        //    bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

        //    pdfViewerControl.AnnotationMode = AnnotationMode.Highlight;

        //    this.SelectedColor = Color.Black;

        //    ValidButton.IsVisible = true;

        //    //selectTextMarkupBar.IsVisible = false;
        //}


        private void BackSelectTextMarkupButton_Clicked()
        {
            //selectTextMarkupBar.IsVisible = false;

            bottomLayout.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.None;

            #region Text markup choice bar events
            //selectTextMarkupBar.StrikethroughtButtonClicked -= StriketroughtButton_Clicked;
            //selectTextMarkupBar.HightlightButtonClicked -= HightlightButton_Clicked;
            //selectTextMarkupBar.UnderlineButtonClicked -= UnderlineButton_Clicked;
            //selectTextMarkupBar.BackButtonClicked -= BackSelectTextMarkupButton_Clicked;
            #endregion
        }

        #endregion

        #region Commom methods for annotations
        private void ThicknessButton_Clicked()
        {

        }

        private void ColorButton_Clicked()
        {
        }
        #endregion

        #region Popop selected annotation method events

        //private void StyleButton_Clicked()
        //{
        //    if (annotationType == AnnotationType.FreeText)
        //    {
        //    }

        //    else
        //    {
        //        if (annotationType == AnnotationType.Ink)
        //        {

        //        }
        //        else
        //        {
        //            if (annotationType == AnnotationType.Shape)
        //            {
        //            }
        //            else
        //            {
        //                if (annotationType == AnnotationType.TextMarkup)
        //                {

        //                }
        //            }
        //        }
        //    }

        //}

        private void RemvoveButton_Clicked()
        {
            if (selectedFreeTextAnnotation != null)
            {
                pdfViewerControl.RemoveAnnotation(selectedFreeTextAnnotation);
            }
            else
            {
                if (selectedInkAnnotation != null)
                {
                    pdfViewerControl.RemoveAnnotation(selectedInkAnnotation);
                }
                else
                {
                    if (selectedShapeAnnotation != null)
                    {
                        pdfViewerControl.RemoveAnnotation(selectedShapeAnnotation);
                    }
                    else
                    {
                        if (selectedTextMarkupAnnotation != null)
                        {
                            pdfViewerControl.RemoveAnnotation(selectedTextMarkupAnnotation);
                        }
                    }
                }
            }
        }

        #endregion

        #region ThicknessBarEvents
        private void ThicknessBar_Clicked(int numberOfThicknessBarClicked)
        {
            if(lastThicknessBarSelected != numberOfThicknessBarClicked)
            {
                switch (lastThicknessBarSelected)
                {
                    case 1:
                        styleContent.ThicknessBar.FirstThicknessBar.BorderThickness = 0;
                        break;
                    case 2:
                        styleContent.ThicknessBar.SecondThicknessBar.BorderThickness = 0;
                        break;
                    case 3:
                        styleContent.ThicknessBar.ThirdThicknessBar.BorderThickness = 0;
                        break;
                    case 4:
                        styleContent.ThicknessBar.FourthThicknessBar.BorderThickness = 0;
                        break;
                    case 5:
                        styleContent.ThicknessBar.FifthThicknessBar.BorderThickness = 0;
                        break;
                    default:
                        break;
                }

                switch (numberOfThicknessBarClicked)
                {
                    case 1:
                        styleContent.ThicknessBar.FirstThicknessBar.BorderThickness = 1;
                        ChangeThicknessForAnnotation(2);
                        break;
                    case 2:
                        styleContent.ThicknessBar.SecondThicknessBar.BorderThickness = 2;
                        ChangeThicknessForAnnotation(4);
                        break;
                    case 3:
                        styleContent.ThicknessBar.ThirdThicknessBar.BorderThickness = 2;
                        ChangeThicknessForAnnotation(6);
                        break;
                    case 4:
                        styleContent.ThicknessBar.FourthThicknessBar.BorderThickness = 2;
                        ChangeThicknessForAnnotation(8);
                        break;
                    case 5:
                        styleContent.ThicknessBar.FifthThicknessBar.BorderThickness = 2;
                        ChangeThicknessForAnnotation(9);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                return;
            }

            lastThicknessBarSelected = numberOfThicknessBarClicked;
        }

        private void ChangeThicknessForAnnotation(int thicknessValue)
        {
            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Thickness = thicknessValue;
            else
            {
                if (selectedShapeAnnotation != null)
                    selectedShapeAnnotation.Settings.Thickness = thicknessValue;
                else
                {
                    switch (pdfViewerControl.AnnotationMode)
                    {
                        case AnnotationMode.Ink:
                            pdfViewerControl.AnnotationSettings.Ink.Thickness = thicknessValue;
                            break;
                        case AnnotationMode.Rectangle:
                            pdfViewerControl.AnnotationSettings.Rectangle.Settings.Thickness = thicknessValue;
                            break;
                        case AnnotationMode.Circle:
                            pdfViewerControl.AnnotationSettings.Circle.Settings.Thickness = thicknessValue;
                            break;
                        case AnnotationMode.Line:
                            pdfViewerControl.AnnotationSettings.Line.Settings.Thickness = thicknessValue;
                            break;
                        case AnnotationMode.Arrow:
                            pdfViewerControl.AnnotationSettings.Arrow.Settings.Thickness = thicknessValue;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion

        #region Opacity Methods 

        private void OpacityIcon_Clicked(int numberOfTheOpacitySelected)
        {
            if (lastOpacitySelected != numberOfTheOpacitySelected)
            {
                switch (lastOpacitySelected)
                {
                    case 1:
                        styleContent.OpacityImageTo25.Source = "baseline_invert_colors_off_24.xml";
                        break;
                    case 2:
                        styleContent.OpacityImageTo50.Source = "baseline_invert_colors_off_24.xml";
                        break;
                    case 3:
                        styleContent.OpacityImageTo75.Source = "baseline_invert_colors_off_24.xml";
                        break;
                    case 4:
                        styleContent.OpacityImageTo100.Source = "baseline_invert_colors_off_24.xml";
                        break;
                    default:
                        break;
                }

                switch (numberOfTheOpacitySelected)
                {
                    case 1:
                        styleContent.OpacityImageTo25.Source = "baseline_opacity_24.xml";
                        ChangeOpacityValue(0.25f);
                        break;
                    case 2:
                        styleContent.OpacityImageTo50.Source = "baseline_opacity_24.xml";
                        ChangeOpacityValue(0.50f);
                        break;
                    case 3:
                        styleContent.OpacityImageTo75.Source = "baseline_opacity_24.xml";
                        ChangeOpacityValue(0.75f);
                        break;
                    case 4:
                        styleContent.OpacityImageTo100.Source = "baseline_opacity_24.xml";
                        ChangeOpacityValue(1f);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                return;
            }

            lastOpacitySelected = numberOfTheOpacitySelected;
        }

        private void ChangeOpacityValue(float opacityValue)
        {
            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Opacity = opacityValue;
            else
            {
                if (selectedShapeAnnotation != null)
                    selectedShapeAnnotation.Settings.Opacity = opacityValue;
                else
                {
                    switch (pdfViewerControl.AnnotationMode)
                    {
                        case AnnotationMode.Ink:
                            pdfViewerControl.AnnotationSettings.Ink.Opacity = opacityValue;
                            break;
                        case AnnotationMode.Rectangle:
                            pdfViewerControl.AnnotationSettings.Rectangle.Settings.Opacity = opacityValue;
                            break;
                        case AnnotationMode.Circle:
                            pdfViewerControl.AnnotationSettings.Circle.Settings.Opacity = opacityValue;
                            break;
                        case AnnotationMode.Line:
                            pdfViewerControl.AnnotationSettings.Line.Settings.Opacity = opacityValue;
                            break;
                        case AnnotationMode.Arrow:
                            pdfViewerControl.AnnotationSettings.Arrow.Settings.Opacity = opacityValue;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion

        #region BottomMainBarMethods
        private async void TextMarkupButton_Clicked(object sender, EventArgs e)
        {
            await CollapseBottomMainToolbar(AnnotationType.TextMarkup);
        }

        private async void ShapeButton_Clicked(object sender, EventArgs e)
        {
            await CollapseBottomMainToolbar(AnnotationType.Shape);
        }

        private async void InkButton_Clicked(object sender, EventArgs e)
        {
            await CollapseBottomMainToolbar(AnnotationType.Ink);
        }

        private async void FreeTextButton_Clicked(object sender, EventArgs e)
        {
            await CollapseBottomMainToolbar(AnnotationType.FreeText);
        }

        private void SetToolbarForShapeAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;
            this.colorPicker.IsVisible = false;
            styleContent.FontSizeControl.IsVisible = false;

            stylePopup.PopupView.HeightRequest = 240;
            stylePopup.PopupView.WidthRequest = 280;

            shapeToolbar.IsVisible = true;
            styleContent.BoxView2.IsVisible = true;
            styleContent.OpacityControl.IsVisible = true;
            styleContent.ThicknessBar.IsVisible = true;
        }

        private void SetToolbarForFreeTextAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;
            styleContent.OpacityControl.IsVisible = false;
            styleContent.BoxView2.IsVisible = false;
            styleContent.ThicknessBar.IsVisible = false;
            colorPicker.IsVisible = false;

            stylePopup.PopupView.HeightRequest = 160;
            stylePopup.PopupView.WidthRequest = 280;

            styleContent.FontSizeControl.IsVisible = true;
            paletteButton.IsVisible = true;
            annotationTypeToolbar.IsVisible = true;

            imageAnnotationType.Source = "twotone_title_24.xml";

            pdfViewerControl.AnnotationMode = AnnotationMode.FreeText;
            pdfViewerControl.AnnotationSettings.FreeText.TextSize = 8;
            this.annotationType = AnnotationType.FreeText;
            this.SelectedColor = Color.Black;
            this.FontSize = 8;
        }

        private void SetToolbarForInkAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;
            this.colorPicker.IsVisible = false;
            styleContent.FontSizeControl.IsVisible = false;

            imageAnnotationType.Source = "twotone_gesture_24.xml";

            paletteButton.IsVisible = true;
            annotationTypeToolbar.IsVisible = true;
            styleContent.OpacityControl.IsVisible = true;
            styleContent.ThicknessBar.IsVisible = true;
            styleContent.BoxView2.IsVisible = true;

            stylePopup.PopupView.HeightRequest = 240;
            stylePopup.PopupView.WidthRequest = 280;

            pdfViewerControl.AnnotationMode = AnnotationMode.Ink;
            pdfViewerControl.AnnotationSettings.Ink.Thickness = 9;
            this.annotationType = AnnotationType.Ink;
            this.SelectedColor = Color.Black;
        }

        private void SetToolbarForTextMarkupAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;

            colorPicker.IsVisible = true;
            textMarkupToolbar.IsVisible = true;
        }

        private async Task CollapseBottomMainToolbar(AnnotationType annotationType)
        {

            switch (annotationType)
            {
                case AnnotationType.Ink:
                    SetToolbarForInkAnnotationSelected();
                    break;
                case AnnotationType.FreeText:
                    SetToolbarForFreeTextAnnotationSelected();
                    break;
                case AnnotationType.Shape:
                    SetToolbarForShapeAnnotationSelected();
                    break;
                case AnnotationType.TextMarkup:
                    SetToolbarForTextMarkupAnnotationSelected();
                    break;
                case AnnotationType.None:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region TextMarkup toolbar methods
        private async void BackButtonTextMarkupToolbar_Clicked(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                await textMarkupToolbar.LayoutTo(new Rectangle(textMarkupToolbar.Bounds.X, textMarkupToolbar.Bounds.Y, textMarkupToolbar.Bounds.Width, 0), 250, Easing.Linear);
            });

            textMarkupToolbar.IsVisible = false;
            bottomMainToolbar.IsVisible = true;
        }

        private async void StrikethroughtButton_Clicked(object sender, EventArgs e)
        {
            textMarkupToolbar.IsVisible = false;

            imageAnnotationType.Source = "ic_strikethrough.png";
            annotationTypeToolbar.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Strikethrough;
            this.annotationType = AnnotationType.Strikethrought;
            this.SelectedColor = Color.Yellow;
        }

        private async void UnderlineButton_Clicked(object sender, EventArgs e)
        {
            textMarkupToolbar.IsVisible = false;

            imageAnnotationType.Source = "ic_underline.png";
            annotationTypeToolbar.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Underline;
            this.annotationType = AnnotationType.Underline;
            this.SelectedColor = Color.Red;
        }

        private async void HightlightButton_Clicked(object sender, EventArgs e)
        {
            textMarkupToolbar.IsVisible = false;

            imageAnnotationType.Source = "ic_edit.png";
            annotationTypeToolbar.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Highlight;
            this.annotationType = AnnotationType.Hightlight;
            this.SelectedColor = Color.Yellow;
        }
        #endregion

        #region AnnotationType toolbar methods

        private async void BackButtonAnnotationTypeToolbar_Clicked(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                await annotationTypeToolbar.LayoutTo(new Rectangle(annotationTypeToolbar.Bounds.X, annotationTypeToolbar.Bounds.Y, annotationTypeToolbar.Bounds.Width, 0), 150, Easing.Linear);
            });

            annotationTypeToolbar.IsVisible = false;

            switch (this.annotationType)
            {
                case AnnotationType.Ink:
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    annotationType = AnnotationType.None;
                    bottomMainToolbar.IsVisible = true; 
                    break;
                case AnnotationType.FreeText:
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    annotationType = AnnotationType.None;
                    bottomMainToolbar.IsVisible = true;
                    break;
                case (AnnotationType.Arrow) :
                    shapeToolbar.IsVisible = true;
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Circle):
                    shapeToolbar.IsVisible = true;
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Line):
                    shapeToolbar.IsVisible = true;
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Rectangle):
                    shapeToolbar.IsVisible = true;
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Hightlight):
                    textMarkupToolbar.IsVisible = true;
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Strikethrought):
                    textMarkupToolbar.IsVisible = true;
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Underline):
                    textMarkupToolbar.IsVisible = true;
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case AnnotationType.None:
                    break;
                default:
                    break;
            }
        }

        private async void PaletteButton_Clicked()
        {
            var viewPortWidth = Application.Current.MainPage.Width;
            var value = (viewPortWidth / 2) - 125;
            stylePopup.ShowRelativeToView(paletteButton, RelativePosition.AlignTopRight,0, -7);
        }
        #endregion

        #region Shape bar methods
        private async void BackButtonShapeToolbar_Clicked(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                await shapeToolbar.LayoutTo(new Rectangle(shapeToolbar.Bounds.X, shapeToolbar.Bounds.Y, shapeToolbar.Bounds.Width, 0), 250, Easing.Linear);
            });

            shapeToolbar.IsVisible = false;
            paletteButton.IsVisible = false;
            bottomMainToolbar.IsVisible = true;
        }

        private void CircleButton_Clicked(object sender, EventArgs e)
        {
            shapeToolbar.IsVisible = false;

            imageAnnotationType.Source = "ic_ui.png";
            paletteButton.IsVisible = true;
            annotationTypeToolbar.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Circle;
            pdfViewerControl.AnnotationSettings.Circle.Settings.Thickness = 9;
            this.annotationType = AnnotationType.Circle;
            this.SelectedColor = Color.Black;

        }

        private void LineButton_Clicked(object sender, EventArgs e)
        {
            shapeToolbar.IsVisible = false;
            paletteButton.IsVisible = true;
            imageAnnotationType.Source = "ic_square.png";
            annotationTypeToolbar.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Line;
            pdfViewerControl.AnnotationSettings.Line.Settings.Thickness = 9;
            this.annotationType = AnnotationType.Line;
            this.SelectedColor = Color.Black;
        }

        private void ArrowButton_Clicked(object sender, EventArgs e)
        {
            shapeToolbar.IsVisible = false;
            paletteButton.IsVisible = true;
            imageAnnotationType.Source = "ic_directional.png";
            annotationTypeToolbar.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Arrow;
            pdfViewerControl.AnnotationSettings.Arrow.Settings.Thickness = 9;
            this.annotationType = AnnotationType.Arrow;
            this.SelectedColor = Color.Black;
        }

        private void RectangleButton_Clicked(object sender, EventArgs e)
        {
            shapeToolbar.IsVisible = false;
            paletteButton.IsVisible = true;
            imageAnnotationType.Source = "ic_math.png";
            annotationTypeToolbar.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Rectangle;
            pdfViewerControl.AnnotationSettings.Rectangle.Settings.Thickness = 9;
            this.annotationType = AnnotationType.Rectangle;
            this.SelectedColor = Color.Black;
        }

        #endregion

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}