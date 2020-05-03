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
      
        private BottomAnnotationToolbar bottomAnnotationToolbar;
        private SelectTextMarkupBar selectTextMarkupBar;
        private ShapeBar shapeBar;

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
        private float opacity = 100;
        private bool canUndoInk = false;
        private bool canRedoInk = false;
        private int shapesNumbers = 0;
        private int textMarkupNumbers = 0;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public float OpacitySlider
        {
            get
            {
                return opacity;


            }

            set
            {
                opacity = value;
                OnPropertyChanged();

                if (selectedInkAnnotation != null)
                    selectedInkAnnotation.Settings.Opacity = value;
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
            pdfViewerControl.Unload();

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

            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Task.Run(() =>
            {
                pdfViewerControl.DocumentLoaded += PdfViewerControl_DocumentLoaded;
                //Retrieves the current horizontal offset of the PdfViewerControl
                
                var pdfStream = DependencyService.Get<IAndroidFileHelper>().GetFileStream(filePath);
                pdfViewerControl.CustomPdfRenderer = DependencyService.Get<ICustomPdfRendererService>().AlternatePdfRenderer;

                Device.BeginInvokeOnMainThread(() =>
                {

                    pdfViewerControl.LoadDocument(pdfStream);
                });
            });

            MessagingCenter.Subscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor", (sender, helper) =>
            {
                this.SelectedColor = helper;
            });
        }

        public PdfViewer(string filepath)
        {
            InitializeComponent();

            pdfViewerControl.Toolbar.Enabled = false;

            //pdfViewerControl.CustomPdfRenderer = DependencyService.Get<ICustomPdfRendererService>().AlternatePdfRenderer;
            //pdfViewerControl.BindingContext = pdfViewerModel = new PdfViewerModel(filepath);
        }

        private void PdfViewerControl_DocumentLoaded(object sender, EventArgs args)
        {
            Shell.SetNavBarIsVisible(this, false);
            Shell.SetTabBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            #region Instanciation
            this.bottomAnnotationToolbar = new BottomAnnotationToolbar();
            bottomAnnotationToolbar.BindingContext = this;
            #endregion

            #region Add content view but set them invisible 
            bottomAnnotationToolbar.IsVisible = false;
            mainStackLayout.Children.Insert(2, bottomAnnotationToolbar);

            ValidButton.IsVisible = false;
            UndoButton.IsVisible = false;
            RedoButton.IsVisible = false;
            #endregion

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

            //signatureButton.GestureRecognizers.Add(new TapGestureRecognizer()
            //{
            //    Command = new Command(async () =>
            //    {
            //        signatureButton.Foreground = Color.FromHex("#b4b4b4");
            //        await SignaturePadButton_Clicked();
            //        await Task.Delay(100);
            //        signatureButton.Foreground = Color.FromHex("4e4e4e");
            //    })
            //});

            stampButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    stampButton.Foreground = Color.FromHex("#b4b4b4");
                    StampButton_Clicked();
                    await Task.Delay(100);
                    stampButton.Foreground = Color.FromHex("4e4e4e");
                })
            });


            freeTextButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    freeTextButton.Foreground = Color.FromHex("#b4b4b4");
                    AnnotationButton_Clicked(AnnotationType.FreeText);
                    await Task.Delay(100);
                    freeTextButton.Foreground = Color.FromHex("4e4e4e");
                })
            });

            inkButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    inkButton.Foreground = Color.FromHex("#b4b4b4");
                    AnnotationButton_Clicked(AnnotationType.Ink);
                    await Task.Delay(100);
                    inkButton.Foreground = Color.FromHex("4e4e4e");
                })
            });

            shapeButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    shapeButton.Foreground = Color.FromHex("#b4b4b4");
                    AnnotationButton_Clicked(AnnotationType.Shape);



                    await Task.Delay(100);
                    shapeButton.Foreground = Color.FromHex("4e4e4e");
                })
            });

            textMarkupButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    textMarkupButton.Foreground = Color.FromHex("#b4b4b4");
                    AnnotationButton_Clicked(AnnotationType.TextMarkup);
                    await Task.Delay(100);
                    textMarkupButton.Foreground = Color.FromHex("4e4e4e");
                })
            });

            pdfViewerControl.ScrollToOffset(50, 50);
        }

        #region Pdf viewer events methods
        //Set the signature pad page
        //private async Task SignaturePadButton_Clicked()
        //{
        //    topToolbar.IsVisible = false;
        //    bottomToolbar.IsVisible = false;

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
        //            bottomToolbar.IsVisible = true;
        //        }
        //        else
        //        {
        //            topToolbar.IsVisible = true;
        //            bottomToolbar.IsVisible = true;
        //        }
        //    };
        //    await Navigation.PushAsync(page);
        //}

        private void StampButton_Clicked()
        {
            throw new NotImplementedException();
        }

        private void AnnotationButton_Clicked(AnnotationType annotationType)
        {
            bottomAbsoluteLayout.IsVisible = false;

            switch (annotationType)
            {
                case AnnotationType.Ink:
                    bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";
                    bottomAnnotationToolbar.IsVisible = true;

                    ValidButton.IsVisible = true;
                    UndoButton.IsVisible = true;
                    RedoButton.IsVisible = true;

                    this.SelectedColor = Color.Black;

                    this.shapeBar = new ShapeBar();
                    break;
                case AnnotationType.FreeText:
                    bottomAnnotationToolbar.AnnotationImage.Source = "font.png";
                    bottomAnnotationToolbar.IsVisible = true;

                    this.SelectedColor = Color.Black;
                    break;
                case AnnotationType.Shape:
                    bottomAnnotationToolbar.AnnotationImage.Source = "rectangle.png";

                    shapeBar.IsVisible = true;
                    mainStackLayout.Children.Insert(2, shapeBar);

                    //Not like free text and ink
                    #region Shape bottom bar events
                    shapeBar.ArrowButtonClicked += ArrowButton_Clicked;
                    shapeBar.LineButtonClicked += LineButton_Clicked;
                    shapeBar.EllipseButtonClicked += EllipseButton_Clicked;
                    shapeBar.RectangleButtonClicked += RectangleButton_Clicked;
                    shapeBar.BackButtonClicked += BackShapeButton_Clicked;
                    #endregion

                    break;
                case AnnotationType.TextMarkup:
                    bottomAnnotationToolbar.AnnotationImage.Source = "marker.png";

                    selectTextMarkupBar.IsVisible = true;
                    mainStackLayout.Children.Insert(2, selectTextMarkupBar);

                    #region Text markup choice bar events
                    selectTextMarkupBar.StrikethroughtButtonClicked += StriketroughtButton_Clicked;
                    selectTextMarkupBar.HightlightButtonClicked += HightlightButton_Clicked;
                    selectTextMarkupBar.UnderlineButtonClicked += UnderlineButton_Clicked;
                    selectTextMarkupBar.BackButtonClicked += BackSelectTextMarkupButton_Clicked;
                    #endregion

                    this.selectTextMarkupBar = new SelectTextMarkupBar();
                    break;
                default:
                    break;
            }
        }

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
            this.annotationType = AnnotationType.Shape;

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
            bottomAnnotationToolbar.IsVisible = false;

            bottomAbsoluteLayout.IsVisible = true;
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
            bottomAnnotationToolbar.IsVisible = false;

            bottomAbsoluteLayout.IsVisible = true;

            pdfViewerControl.EndInkSession(true);
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

        private void ArrowButton_Clicked()
        {
            shapeBar.IsVisible = false;

            if (bottomAnnotationToolbar.IsVisible == false)
                bottomAnnotationToolbar.IsVisible = true;

            //Set Annotation Image
            bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

            ValidButton.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Arrow;

            this.SelectedColor = Color.Black;
        }

        private void LineButton_Clicked()
        {
            shapeBar.IsVisible = false;

            if (bottomAnnotationToolbar.IsVisible == false)
                bottomAnnotationToolbar.IsVisible = true;

            //Set Annotation Image
            bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

            ValidButton.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Line;

            this.SelectedColor = Color.Black;
        }

        private void EllipseButton_Clicked()
        {
            shapeBar.IsVisible = false;

            if (bottomAnnotationToolbar.IsVisible == false)
                bottomAnnotationToolbar.IsVisible = true;

            //Set Annotation Image
            bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

            ValidButton.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Circle;

            this.SelectedColor = Color.Black;
        }

        private void RectangleButton_Clicked()
        {
            shapeBar.IsVisible = false;

            if (bottomAnnotationToolbar.IsVisible == false)
                bottomAnnotationToolbar.IsVisible = true;

            //Set Annotation Image
            bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

            ValidButton.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Rectangle;

            this.SelectedColor = Color.Black;
        }

        private void BackShapeButton_Clicked()
        {
            shapeBar.IsVisible = false;

            bottomAbsoluteLayout.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.None;

            //Not like free text and ink
            #region Shape bottom bar events
            shapeBar.ArrowButtonClicked -= ArrowButton_Clicked;
            shapeBar.LineButtonClicked -= LineButton_Clicked;
            shapeBar.EllipseButtonClicked -= EllipseButton_Clicked;
            shapeBar.RectangleButtonClicked -= RectangleButton_Clicked;
            shapeBar.BackButtonClicked -= BackShapeButton_Clicked;
            #endregion
        }
        #endregion

        #region Select Text markup events methods

        private void StriketroughtButton_Clicked()
        {
            if (bottomAnnotationToolbar.IsVisible == false)
                bottomAnnotationToolbar.IsVisible = true;

            //Set Annotation Image
            bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

            pdfViewerControl.AnnotationMode = AnnotationMode.Strikethrough;

            this.SelectedColor = Color.Black;

            selectTextMarkupBar.IsVisible = false;

            ValidButton.IsVisible = true;
        }

        private void UnderlineButton_Clicked()
        {
            if (bottomAnnotationToolbar.IsVisible == false)
                bottomAnnotationToolbar.IsVisible = true;

            //Set Annotation Image
            bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

            pdfViewerControl.AnnotationMode = AnnotationMode.Underline;

            this.SelectedColor = Color.Black;

            selectTextMarkupBar.IsVisible = false;

            ValidButton.IsVisible = true;
        }

        private void HightlightButton_Clicked()
        {
            if (bottomAnnotationToolbar.IsVisible == false)
                bottomAnnotationToolbar.IsVisible = true;

            //Set Annotation Image
            bottomAnnotationToolbar.AnnotationImage.Source = "pencil.png";

            pdfViewerControl.AnnotationMode = AnnotationMode.Highlight;

            this.SelectedColor = Color.Black;

            ValidButton.IsVisible = true;

            selectTextMarkupBar.IsVisible = false;
        }


        private void BackSelectTextMarkupButton_Clicked()
        {
            selectTextMarkupBar.IsVisible = false;

            bottomAbsoluteLayout.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.None;

            #region Text markup choice bar events
            selectTextMarkupBar.StrikethroughtButtonClicked -= StriketroughtButton_Clicked;
            selectTextMarkupBar.HightlightButtonClicked -= HightlightButton_Clicked;
            selectTextMarkupBar.UnderlineButtonClicked -= UnderlineButton_Clicked;
            selectTextMarkupBar.BackButtonClicked -= BackSelectTextMarkupButton_Clicked;
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

        private void StyleButton_Clicked()
        {
            if (annotationType == AnnotationType.FreeText)
            {
            }

            else
            {
                if (annotationType == AnnotationType.Ink)
                {

                }
                else
                {
                    if (annotationType == AnnotationType.Shape)
                    {
                    }
                    else
                    {
                        if (annotationType == AnnotationType.TextMarkup)
                        {

                        }
                    }
                }
            }

        }

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
        private void FirstBoxView_Clicked()
        {
            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Thickness = 1;

            if (selectedShapeAnnotation != null)
                selectedShapeAnnotation.Settings.Thickness = 1;

            switch (pdfViewerControl.AnnotationMode)
            {
                case AnnotationMode.Ink:
                    pdfViewerControl.AnnotationSettings.Ink.Thickness = 1;
                    break;
                case AnnotationMode.Rectangle:
                    pdfViewerControl.AnnotationSettings.Rectangle.Settings.Thickness = 1;
                    break;
                case AnnotationMode.Circle:
                    pdfViewerControl.AnnotationSettings.Circle.Settings.Thickness = 1;
                    break;
                case AnnotationMode.Line:
                    pdfViewerControl.AnnotationSettings.Line.Settings.Thickness = 1;
                    break;
                case AnnotationMode.Arrow:
                    pdfViewerControl.AnnotationSettings.Arrow.Settings.Thickness = 1;
                    break;
                default:
                    break;
            }
        }
        private void SecondBoxView_Clicked()
        {
            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Thickness = 3;

            if (selectedShapeAnnotation != null)
                selectedShapeAnnotation.Settings.Thickness = 3;

            switch (pdfViewerControl.AnnotationMode)
            {
                case AnnotationMode.Ink:
                    pdfViewerControl.AnnotationSettings.Ink.Thickness = 3;
                    break;
                case AnnotationMode.Rectangle:
                    pdfViewerControl.AnnotationSettings.Rectangle.Settings.Thickness = 3;
                    break;
                case AnnotationMode.Circle:
                    pdfViewerControl.AnnotationSettings.Circle.Settings.Thickness = 3;
                    break;
                case AnnotationMode.Line:
                    pdfViewerControl.AnnotationSettings.Line.Settings.Thickness = 3;
                    break;
                case AnnotationMode.Arrow:
                    pdfViewerControl.AnnotationSettings.Arrow.Settings.Thickness = 3;
                    break;
                default:
                    break;
            }
        }

        private void ThirdBoxView_Clicked()
        {
            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Thickness = 5;

            if (selectedShapeAnnotation != null)
                selectedShapeAnnotation.Settings.Thickness = 5;

            switch (pdfViewerControl.AnnotationMode)
            {
                case AnnotationMode.Ink:
                    pdfViewerControl.AnnotationSettings.Ink.Thickness = 5;
                    break;
                case AnnotationMode.Rectangle:
                    pdfViewerControl.AnnotationSettings.Rectangle.Settings.Thickness = 5;
                    break;
                case AnnotationMode.Circle:
                    pdfViewerControl.AnnotationSettings.Circle.Settings.Thickness = 5;
                    break;
                case AnnotationMode.Line:
                    pdfViewerControl.AnnotationSettings.Line.Settings.Thickness = 5;
                    break;
                case AnnotationMode.Arrow:
                    pdfViewerControl.AnnotationSettings.Arrow.Settings.Thickness = 5;
                    break;
                default:
                    break;
            }
        }

        private void FourthBoxView_Clicked()
        {
            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Thickness = 7;

            if (selectedShapeAnnotation != null)
                selectedShapeAnnotation.Settings.Thickness = 7;

            switch (pdfViewerControl.AnnotationMode)
            {
                case AnnotationMode.Ink:
                    pdfViewerControl.AnnotationSettings.Ink.Thickness = 7;
                    break;
                case AnnotationMode.Rectangle:
                    pdfViewerControl.AnnotationSettings.Rectangle.Settings.Thickness = 7;
                    break;
                case AnnotationMode.Circle:
                    pdfViewerControl.AnnotationSettings.Circle.Settings.Thickness = 7;
                    break;
                case AnnotationMode.Line:
                    pdfViewerControl.AnnotationSettings.Line.Settings.Thickness = 7;
                    break;
                case AnnotationMode.Arrow:
                    pdfViewerControl.AnnotationSettings.Arrow.Settings.Thickness = 7;
                    break;
                default:
                    break;
            }

        }

        private void FifthBoxView_Clicked()
        {
            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Thickness = 9;

            if (selectedShapeAnnotation != null)
                selectedShapeAnnotation.Settings.Thickness = 9;

            switch (pdfViewerControl.AnnotationMode)
            {
                case AnnotationMode.Ink:
                    pdfViewerControl.AnnotationSettings.Ink.Thickness = 9;
                    break;
                case AnnotationMode.Rectangle:
                    pdfViewerControl.AnnotationSettings.Rectangle.Settings.Thickness = 9;
                    break;
                case AnnotationMode.Circle:
                    pdfViewerControl.AnnotationSettings.Circle.Settings.Thickness = 9;
                    break;
                case AnnotationMode.Line:
                    pdfViewerControl.AnnotationSettings.Line.Settings.Thickness = 9;
                    break;
                case AnnotationMode.Arrow:
                    pdfViewerControl.AnnotationSettings.Arrow.Settings.Thickness = 9;
                    break;
                default:
                    break;
            }
        }

        #endregion


        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}