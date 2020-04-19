using Pdf.controls;
using Pdf.Enumerations;
using Pdf.ViewModels;
using Syncfusion.SfPdfViewer.XForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        #region Content view / View
        private readonly Stream fileStream;
        private readonly PdfViewerModel pdfViewerModel;
        private readonly FreeTextBottomBar freeTextBottomBar;
        private readonly InkBottomBar inkBottomBar;
        private readonly AnnotationToolbar annotationToolbar;
        private readonly ShapeBar shapeBar;
        private readonly ShapeCustomBar shapeCustomBar;
        private readonly TextMarkupBar textMarkupBar;
        private readonly SelectTextMarkupBar selectTextMarkupBar;
        private readonly PopupAnnotation popupAnnotationTools;
        private readonly PopupAnnotation stylePopup;
        private readonly DataTemplate editMenuTemplateView;
        private readonly DataTemplate styleMenuTemplateView;
        private readonly EditPopupMenu editPopupMenu;
        private readonly StylePopupContent stylePopupContent;
        #endregion

        private FreeTextAnnotation selectedFreeTextAnnotation;
        private InkAnnotation selectedInkAnnotation;
        private ShapeAnnotation selectedShapeAnnotation;
        private TextMarkupAnnotation selectedTextMarkupAnnotation;

        private AnnotationMode annotationType;
        

        #region Property
        private Color selectedColor = Color.Black;
        private Color statusColor = Color.FromHex("b4b4b4");
        private int fontSize = 6;
        private float opacity = 100;
        private bool canUndoInk = false;
        private bool canRedoInk = false;
        private bool canUndo = false;
        private bool canRedo = false;
        private int shapesNumbers = 0;
        private int textMarkupNumbers = 0;
        private Rectangle lastRectangleBounds;
        private AnnotationMode lastTextMarkupAnnotationMode;

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

                if(pdfViewerControl.AnnotationMode == AnnotationMode.FreeText)
                    pdfViewerControl.AnnotationSettings.FreeText.TextSize = value;
            }
        }

        public Color StatusColor
        {
            get
            {
                return statusColor;
            }

            set
            {
                statusColor = value;
                OnPropertyChanged();
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
                    if(value == false)
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

        public bool CanUndo
        {
            get
            {
                return canUndo;
            }

            set
            {
                canUndo = value;
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

        public bool CanRedo
        {
            get
            {
                return canRedo;
            }

            set
            {
                canRedo = value;
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
        #endregion

        public PdfViewer(Stream fileStream)
        {
            InitializeComponent();

            #region Instanciation
            this.fileStream = fileStream;

            this.annotationToolbar = new AnnotationToolbar();

            this.freeTextBottomBar = new FreeTextBottomBar();
            this.freeTextBottomBar.BindingContext = this;

            this.inkBottomBar = new InkBottomBar();
            this.inkBottomBar.BindingContext = this;

            this.shapeBar = new ShapeBar();

            this.shapeCustomBar = new ShapeCustomBar();
            this.shapeCustomBar.BindingContext = this;

            this.textMarkupBar = new TextMarkupBar();
            this.textMarkupBar.BindingContext = this;

            this.selectTextMarkupBar = new SelectTextMarkupBar();

            this.popupAnnotationTools = new PopupAnnotation();
            this.stylePopup = new PopupAnnotation();

            this.editPopupMenu = new EditPopupMenu();
            this.stylePopupContent = new StylePopupContent();
            this.stylePopupContent.BindingContext = this;

            editMenuTemplateView = new DataTemplate(() =>
            {
                return editPopupMenu;
            });

            styleMenuTemplateView = new DataTemplate(() =>
            {
                return stylePopupContent;
            });

            this.popupAnnotationTools.SfPopupLayout.PopupView.ContentTemplate = editMenuTemplateView;
            this.stylePopup.SfPopupLayout.PopupView.ContentTemplate = styleMenuTemplateView;

            this.stylePopup.SfPopupLayout.PopupView.AnimationDuration = 0;
            this.stylePopup.SfPopupLayout.IsOpen = true;
            this.stylePopup.SfPopupLayout.IsOpen = false;

            this.stylePopup.SfPopupLayout.PopupView.AnimationDuration = 350;
            this.popupAnnotationTools.SfPopupLayout.PopupView.AnimationDuration = 350;

            popupAnnotationTools.SfPopupLayout.PopupView.HeightRequest = 30;
            popupAnnotationTools.SfPopupLayout.PopupView.WidthRequest = 140;

            stylePopup.SfPopupLayout.PopupView.HeightRequest = 70;
            stylePopup.SfPopupLayout.PopupView.WidthRequest = 200;
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
            pdfViewerControl.CanRedoModified += PdfViewerControl_CanRedoModified;
            pdfViewerControl.CanUndoModified += PdfViewerControl_CanUndoModified;

            RedoButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if(CanRedoInk == true)
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

                    if(CanRedo == true)
                    {
                        RedoButton.Foreground = Color.FromHex("#b4b4b4");
                        Redo();
                        await Task.Delay(100);
                        if (CanRedo == true)
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
                    if(CanUndoInk == true)
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

                    if(CanUndo == true)
                    {
                        UndoButton.Foreground = Color.FromHex("#b4b4b4");
                        Undo();
                        await Task.Delay(100);

                        if (CanUndo == true)
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
                        ValidButton.Foreground = Color.FromHex("#b4b4b4");

                        if(pdfViewerControl.AnnotationMode == AnnotationMode.Ink)
                            SaveInk();

                        pdfViewerControl.AnnotationMode = AnnotationMode.None;

                        await Task.Delay(100);

                        CanRedoInk = false;
                        CanUndoInk = false;
                        StatusColor = Color.Transparent;
                    }

                    if (CanUndo == true)
                    {
                        ValidButton.Foreground = Color.FromHex("#b4b4b4");

                        await Task.Delay(100);


                        if (pdfViewerControl.AnnotationMode != AnnotationMode.Ink)
                        {

                            textMarkupBar.IsVisible = false;

                            annotationToolbar.IsVisible = true;

                            pdfViewerControl.AnnotationMode = AnnotationMode.None;
                        }

                        //CanRedo = false;
                        //CanUndo = false;
                        StatusColor = Color.Transparent;
                    }

                    if (pdfViewerControl.AnnotationMode == AnnotationMode.Arrow
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Line
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Circle
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Rectangle)
                    {
                        pdfViewerControl.AnnotationMode = AnnotationMode.None;

                        ValidButton.Foreground = Color.White;

                        shapeCustomBar.IsVisible = false;
                        annotationToolbar.IsVisible = true;
                    }
                    else
                    {
                        if (pdfViewerControl.AnnotationMode == AnnotationMode.Strikethrough
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Highlight
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Underline)
                        {
                            pdfViewerControl.AnnotationMode = AnnotationMode.None;

                            ValidButton.Foreground = Color.White;

                            textMarkupBar.IsVisible = false;

                            annotationToolbar.IsVisible = true;
                        }
                    }
                })
            });
            #endregion

            #region Popup menu events 
            editPopupMenu.RemoveButtonClicked += RemvoveButton_Clicked;
            editPopupMenu.StyleButtonClicked += StyleButton_Clicked;
            #endregion

            #region Free Text button events 
            //TextButtonClickedBottomBar events
            freeTextBottomBar.ColorButtonClicked += ColorButton_Clicked;
            freeTextBottomBar.BackButtonClicked += BackTextButton_Clicked;
            freeTextBottomBar.FreeTextButtonClicked += FreeTextButtonBottomBar_Clicked;
            #endregion

            #region Ink bottom bar events 
            inkBottomBar.ColorButtonClicked += ColorButton_Clicked;
            inkBottomBar.BackButtonClicked += BackPenButton_Clicked;
            inkBottomBar.PenSizeButtonClicked += ThicknessButton_Clicked;
            inkBottomBar.PenStatusButtonClicked += InkStatusButton_Clicked;
            #endregion


            #region Custom shape bar events
            shapeCustomBar.BackButtonClicked += BackShapeCustomButton_Clicked;
            shapeCustomBar.ShapeSizeButtonClicked += ThicknessButton_Clicked;
            shapeCustomBar.ColorButtonClicked += ColorButton_Clicked;
            shapeCustomBar.TrashButtonClicked += TrashShapeButton_Clicked;
            #endregion

            #region Text markup events
            textMarkupBar.ColorButtonClicked += ColorButton_Clicked;
            textMarkupBar.BackButtonClicked += BackTextMarkupButton_Clicked;
            textMarkupBar.TrashCanButtonClicked += TrashTextMarkupButton_Clicked;
            textMarkupBar.TextMarkupStatusButtonClicked += TextMarkupStatusButton_Clicked;
            #endregion


            #region Free Text edit button events

            MessagingCenter.Subscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor", (sender, helper) =>
            {
                this.SelectedColor = helper;
            });
            #endregion



            #region Add content view but set them invisible 
            annotationToolbar.IsVisible = false;
            mainStackLayout.Children.Insert(2, annotationToolbar);

            freeTextBottomBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, freeTextBottomBar);

            inkBottomBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, inkBottomBar);

            shapeBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, shapeBar);

            shapeCustomBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, shapeCustomBar);

            textMarkupBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, textMarkupBar);

            selectTextMarkupBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, selectTextMarkupBar);

            ValidButton.IsVisible = false;
            UndoButton.IsVisible = false;
            RedoButton.IsVisible = false;
            #endregion

            BindingContext = pdfViewerModel = new PdfViewerModel(fileStream);

            pdfViewerControl.Toolbar.Enabled = true;

            signatureButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    signatureButton.Foreground = Color.FromHex("#b4b4b4");
                    await SignaturePadButton_Clicked();
                    await Task.Delay(100);
                    signatureButton.Foreground = Color.FromHex("4e4e4e");
                })
            });

            annotationToolbarButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    annotationToolbarButton.Foreground = Color.FromHex("#b4b4b4");
                    AnnotationToolbar_Clicked();
                    await Task.Delay(100);
                    annotationToolbarButton.Foreground = Color.FromHex("4e4e4e");
                })
            });

        }


        #region Pdf viewer events methods

        private void Undo()
        {
            if (CanUndo == true)
                pdfViewerControl.PerformUndo();
        }

        private void Redo()
        {
            if (CanRedo == true)
                pdfViewerControl.PerformRedo();
        }

        private void PdfViewerControl_CanRedoModified(object sender, CanRedoModifiedEventArgs args)
        {
            CanRedo = args.CanRedo;
        }

        private void PdfViewerControl_CanUndoModified(object sender, CanUndoModifiedEventArgs args)
        {
            CanUndo = args.CanUndo;
        }

        private void PdfViewerControl_TextMarkupAdded(object sender, TextMarkupAddedEventArgs args)
        {
            this.TextMarkupNumbers += 1;
        }

        private void PdfViewerControl_TextMarkupDeselected(object sender, TextMarkupDeselectedEventArgs args)
        { 

            if (bottomToolbar.IsVisible == true)
                bottomToolbar.IsVisible = false;

            #region Remove edit Text markup bar
            textMarkupBar.TrashCanButton.IsVisible = false;
            textMarkupBar.TextMarkupIcon.IsVisible = false;
            textMarkupBar.TextMarkupButtonStatus.IsVisible = true;

            AbsoluteLayout.SetLayoutBounds(textMarkupBar.ColorButton, new Rectangle(0.95, 0.5, -1, -1));

            textMarkupBar.IsVisible = false;
            #endregion

            annotationToolbar.IsVisible = true;
        }

        private void PdfViewerControl_TextMarkupSelected(object sender, TextMarkupSelectedEventArgs args)
        {
            if (freeTextBottomBar.IsVisible == true)
                freeTextBottomBar.IsVisible = false;

            if (annotationToolbar.IsVisible == true)
                annotationToolbar.IsVisible = false;

            if (inkBottomBar.IsVisible == true)
                inkBottomBar.IsVisible = false;

            if (bottomToolbar.IsVisible == true)
                bottomToolbar.IsVisible = false;

            if (shapeBar.IsVisible == true)
                shapeBar.IsVisible = false;

            if (shapeCustomBar.IsVisible == true)
                shapeCustomBar.IsVisible = false;

            if (selectTextMarkupBar.IsVisible == true)
                selectTextMarkupBar.IsVisible = false;

            selectedTextMarkupAnnotation = sender as TextMarkupAnnotation;

            if(args.TextMarkupAnnotationType == TextMarkupAnnotationType.Highlight)
            {
                textMarkupBar.TextMarkupIcon.Source = "draw.png";
            }
            else
            {
                if(args.TextMarkupAnnotationType == TextMarkupAnnotationType.Strikethrough)
                {
                    textMarkupBar.TextMarkupIcon.Source = "strikethrough.png";
                }
                else
                {
                    if(args.TextMarkupAnnotationType == TextMarkupAnnotationType.Underline)
                    {
                        textMarkupBar.TextMarkupIcon.Source = "underline.png";
                    }
                }
            }

            #region Set edit text markup bar
            textMarkupBar.TrashCanButton.IsVisible = true;
            textMarkupBar.TextMarkupIcon.IsVisible = true;
            textMarkupBar.TextMarkupButtonStatus.IsVisible = false;

            AbsoluteLayout.SetLayoutBounds(textMarkupBar.ColorButton, new Rectangle(0.85, 0.5, -1, -1));
            #endregion

            textMarkupBar.IsVisible = true;
        }

        private void PdfViewerControl_ShapeAnnotationDeselected(object sender, ShapeAnnotationDeselectedEventArgs args)
        {
            if (shapeBar.IsVisible == true)
                shapeBar.IsVisible = false;

            if (bottomToolbar.IsVisible == true)
                bottomToolbar.IsVisible = false;

            shapeCustomBar.IsVisible = false;

            annotationToolbar.IsVisible = true;
        }

        //TODO -- FOUND A WAY TO DESELECT OR HANDLE THE BOTTOM BAR BUG
        private void PdfViewerControl_ShapeAnnotationSelected(object sender, ShapeAnnotationSelectedEventArgs args)
        {
            if (freeTextBottomBar.IsVisible == true)
                freeTextBottomBar.IsVisible = false;


            if (annotationToolbar.IsVisible == true)
                annotationToolbar.IsVisible = false;

            if (inkBottomBar.IsVisible == true)
                inkBottomBar.IsVisible = false;

            if (bottomToolbar.IsVisible == true)
                bottomToolbar.IsVisible = false;

            if (shapeBar.IsVisible == true)
                shapeBar.IsVisible = false;

            if(shapeCustomBar.IsVisible == false)
                shapeCustomBar.IsVisible = true;

            ValidButton.IsVisible = false;

            //Cast the sender object as shape annotation.
            this.selectedShapeAnnotation = sender as ShapeAnnotation;
        }

        private void PdfViewerControl_ShapeAnnotationAdded(object sender, ShapeAnnotationAddedEventArgs args)
        {
            this.ShapesNumbers += 1;
        }

        private void AnnotationToolbar_Clicked()
        {
            //Not like free text and ink
            #region Shape bottom bar events
            shapeBar.ArrowButtonClicked += ArrowButton_Clicked;
            shapeBar.LineButtonClicked += LineButton_Clicked;
            shapeBar.EllipseButtonClicked += EllipseButton_Clicked;
            shapeBar.RectangleButtonClicked += RectangleButton_Clicked;
            shapeBar.BackButtonClicked += BackShapeButton_Clicked;
            #endregion

            #region Text markup choice bar events
            selectTextMarkupBar.StrikethroughtButtonClicked += StriketroughtButton_Clicked;
            selectTextMarkupBar.HightlightButtonClicked += HightlightButton_Clicked;
            selectTextMarkupBar.UnderlineButtonClicked += UnderlineButton_Clicked;
            selectTextMarkupBar.BackButtonClicked += BackSelectTextMarkupButton_Clicked;

            #endregion

            //Set events
            annotationToolbar.StampButtonClicked += StampButton_Clicked;
            annotationToolbar.FreeTextButtonClicked += FreeTextButton_Clicked;
            annotationToolbar.InkButtonClicked += InkButton_Clicked;
            annotationToolbar.ShapeButtonClicked += ShapeButton_Clicked;
            annotationToolbar.TextMarkupButtonClicked += TextMarkupButton_Clicked;
            annotationToolbar.BackButtonClicked += BackAnnotationToolbarButton_Clicked;

            bottomToolbar.IsVisible = false;

            annotationToolbar.IsVisible = true;
        }

        //Set the signature pad page
        private async Task SignaturePadButton_Clicked()
        {
            topToolbar.IsVisible = false;
            bottomToolbar.IsVisible = false;

            //pdfViewerControl.AnnotationMode = AnnotationMode.HandwrittenSignature;

            var page = new SignaturePage();
            page.DidFinishPoping += (parameter) =>
            {
                if(parameter != null)
                {
                    //Set image source
                    Image image = new Image();
                    image.Source = ImageSource.FromFile(parameter);
                    image.WidthRequest = 200;
                    image.HeightRequest = 100;

                    int numPage = pdfViewerControl.PageNumber;
                    //Add image as custom stamp to the first page
                    pdfViewerControl.AddStamp(image, numPage);

                    topToolbar.IsVisible = true;
                    bottomToolbar.IsVisible = true;
                }
                else
                {
                    topToolbar.IsVisible = true;
                    bottomToolbar.IsVisible = true;
                }
            };
            await Navigation.PushAsync(page);
        }

        private void PdfViewerControl_FreeTextAnnotationDeselected(object sender, FreeTextAnnotationDeselectedEventArgs args)
        {
            if(popupAnnotationTools.SfPopupLayout.IsOpen == true)
            {
                popupAnnotationTools.SfPopupLayout.StaysOpen = false;
                popupAnnotationTools.SfPopupLayout.IsOpen = false;
            }
            else
            {
                if(stylePopup.SfPopupLayout.IsOpen == true)
                {
                    stylePopup.SfPopupLayout.StaysOpen = false;
                    stylePopup.SfPopupLayout.IsOpen = false;
                }
            }
        }

        private void PdfViewerControl_FreeTextAnnotationSelected(object sender, FreeTextAnnotationSelectedEventArgs args)
        {
            //Get the bounds
            Rectangle bounds = args.Bounds;
            lastRectangleBounds = args.Bounds;

            popupAnnotationTools.SfPopupLayout.PopupView.HeightRequest = 30;
            popupAnnotationTools.SfPopupLayout.PopupView.WidthRequest = 140;

            popupAnnotationTools.SfPopupLayout.StaysOpen = true;
            popupAnnotationTools.SfPopupLayout.Show(bounds.Left / 2 - (150 / 2), bounds.Top / 2 + 40);

            this.annotationType = AnnotationMode.FreeText;

            //Cast the sender object to FreeTextAnnotation
            this.selectedFreeTextAnnotation = sender as FreeTextAnnotation;
        }

        private void PdfViewerControl_FreeTextAnnotationAdded(object sender, FreeTextAnnotationAddedEventArgs args)
        {
            //Disable the free text button 
            this.StatusColor = Color.Transparent;
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
            inkBottomBar.IsVisible = false;

            annotationToolbar.IsVisible = true;

            pdfViewerControl.EndInkSession(true);
        }

        private void PdfViewerControl_InkSelected(object sender, InkSelectedEventArgs args)
        {
            //Get the bounds
            Rectangle bounds = args.Bounds;
            lastRectangleBounds = args.Bounds;

            popupAnnotationTools.SfPopupLayout.StaysOpen = true;
            popupAnnotationTools.SfPopupLayout.Show(bounds.Left / 2 + 60, bounds.Top / 2 + 40);

            this.annotationType = AnnotationMode.Ink;
            //Casts the sender object as Ink annotation.
            selectedInkAnnotation = sender as InkAnnotation;
        }

        private void PdfViewerControl_InkDeselected(object sender, InkDeselectedEventArgs args)
        {
            if (popupAnnotationTools.SfPopupLayout.IsOpen == true)
            {
                popupAnnotationTools.SfPopupLayout.StaysOpen = false;
                popupAnnotationTools.SfPopupLayout.IsOpen = false;
            }
            else
            {
                if (stylePopup.SfPopupLayout.IsOpen == true)
                {
                    stylePopup.SfPopupLayout.StaysOpen = false;
                    stylePopup.SfPopupLayout.IsOpen = false;
                }
            }
        }
        #endregion

        #region FreeTextBottomBar events methods

        private void BackTextButton_Clicked()
        {
            freeTextBottomBar.IsVisible = false;
            #endregion

            annotationToolbar.IsVisible = true;
        }

        private void FreeTextButtonBottomBar_Clicked()
        {
            if (pdfViewerControl.AnnotationMode == AnnotationMode.FreeText)
            {
                this.StatusColor = Color.Transparent;
                pdfViewerControl.AnnotationMode = AnnotationMode.None;
            }
            else
            {
                this.StatusColor = Color.FromHex("#b4b4b4");
                pdfViewerControl.AnnotationMode = AnnotationMode.FreeText;
            }
        }

        #region Annotation toolbar events 

        private void TextMarkupButton_Clicked()
        {
            if (annotationToolbar.IsVisible == true)
                annotationToolbar.IsVisible = false;

            if (selectTextMarkupBar.IsVisible == false)
                selectTextMarkupBar.IsVisible = true;
        }

        private void ShapeButton_Clicked()
        {
            if (annotationToolbar.IsVisible == true)
                annotationToolbar.IsVisible = false;

            if (shapeBar.IsVisible == false)
                shapeBar.IsVisible = true;
        }

        private void BackAnnotationToolbarButton_Clicked()
        {
            annotationToolbar.IsVisible = false;

            bottomToolbar.IsVisible = true;

            ////Remove events
            //annotationToolbar.StampButtonClicked -= StampButton_Clicked;
            //annotationToolbar.FreeTextButtonClicked -= FreeTextButton_Clicked;
            //annotationToolbar.InkButtonClicked -= InkButton_Clicked;
            //annotationToolbar.ShapeButtonClicked -= ShapeButton_Clicked;
            //annotationToolbar.BackButtonClicked -= BackAnnotationToolbarButton_Clicked;

            //#region Shape bottom bar events
            //shapeBar.ArrowButtonClicked -= ArrowButton_Clicked;
            //shapeBar.LineButtonClicked -= LineButton_Clicked;
            //shapeBar.EllipseButtonClicked -= EllipseButton_Clicked;
            //shapeBar.RectangleButtonClicked -= RectangleButton_Clicked;
            //shapeBar.BackButtonClicked -= BackShapeButton_Clicked;
            //#endregion

            //#region Ink bottom bar events 
            //inkBottomBar.ColorButtonClicked -= ColorButton_Clicked;
            //inkBottomBar.BackButtonClicked -= BackPenButton_Clicked;
            //inkBottomBar.PenSizeButtonClicked -= ThicknessButton_Clicked;
            //inkBottomBar.PenStatusButtonClicked -= InkStatusButton_Clicked;
            //#endregion

            //#region Free Text button events 
            ////TextButtonClickedBottomBar events
            //freeTextBottomBar.ColorButtonClicked -= ColorButton_Clicked;
            //freeTextBottomBar.FontSizeButtonClicked -= FontSizeButton_Clicked;
            //freeTextBottomBar.BackButtonClicked -= BackTextButton_Clicked;
            //freeTextBottomBar.FreeTextButtonClicked -= FreeTextButtonBottomBar_Clicked;
            //#endregion
        }

        private void InkButton_Clicked()
        {
            if (annotationToolbar.IsVisible == true)
                annotationToolbar.IsVisible = false;

            if (inkBottomBar.IsVisible == false)
                inkBottomBar.IsVisible = true;


            ValidButton.IsVisible = true;
            UndoButton.IsVisible = true;
            RedoButton.IsVisible = true;

            this.StatusColor = Color.FromHex("b4b4b4");

            pdfViewerControl.AnnotationMode = AnnotationMode.Ink;

            this.SelectedColor = Color.Black;
        }

        private void FreeTextButton_Clicked()
        {
            if (annotationToolbar.IsVisible == true)
                annotationToolbar.IsVisible = false;

            if (freeTextBottomBar.IsVisible == false)
                freeTextBottomBar.IsVisible = true;

            this.StatusColor = Color.FromHex("b4b4b4");

            pdfViewerControl.AnnotationMode = AnnotationMode.FreeText;

            this.SelectedColor = Color.Black;
        }

        private void StampButton_Clicked()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Ink bottom bar events methods
        private void BackPenButton_Clicked()
        {



            ValidButton.IsVisible = false;
            UndoButton.IsVisible = false;
            RedoButton.IsVisible = false;

            #region Remove edit free text bar

            inkBottomBar.IsVisible = false;
            #endregion

            annotationToolbar.IsVisible = true;

            //Trick for deselect ink annotation 
            pdfViewerControl.AnnotationMode = AnnotationMode.Ink;
            pdfViewerControl.AnnotationMode = AnnotationMode.None;
        }

        private void InkStatusButton_Clicked()
        {
            if (pdfViewerControl.AnnotationMode == AnnotationMode.Ink)
            {
                this.StatusColor = Color.Transparent;
                pdfViewerControl.AnnotationMode = AnnotationMode.None;
            }
            else
            {
                this.StatusColor = Color.FromHex("#b4b4b4");
                pdfViewerControl.AnnotationMode = AnnotationMode.Ink;
            }
        }


        #endregion

        #region Shape events methods

        private void ArrowButton_Clicked()
        {
            shapeBar.IsVisible = false;

            shapeCustomBar.ShapeImageButton.Source = "arrowDiagonal.png";
            shapeCustomBar.IsVisible = true;

            ValidButton.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Arrow;

            this.SelectedColor = Color.Black;
        }

        private void LineButton_Clicked()
        {
            shapeBar.IsVisible = false;

            shapeCustomBar.ShapeImageButton.Source = "minus.png";
            shapeCustomBar.IsVisible = true;

            ValidButton.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Line;

            this.SelectedColor = Color.Black;
        }

        private void EllipseButton_Clicked()
        {
            shapeBar.IsVisible = false;

            shapeCustomBar.ShapeImageButton.Source = "oval.png";
            shapeCustomBar.IsVisible = true;

            ValidButton.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Circle;

            this.SelectedColor = Color.Black;
        }

        private void RectangleButton_Clicked()
        {
            shapeBar.IsVisible = false;

            shapeCustomBar.ShapeImageButton.Source = "rectangleShape.png";
            shapeCustomBar.IsVisible = true;

            ValidButton.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Rectangle;

            this.SelectedColor = Color.Black;
        }

        private void BackShapeButton_Clicked()
        {
            shapeBar.IsVisible = false;


            pdfViewerControl.AnnotationMode = AnnotationMode.None;

            annotationToolbar.IsVisible = true;
        }

        #endregion

        #region Shape custom bar event method

        private void TrashShapeButton_Clicked()
        {
            if(selectedShapeAnnotation != null)
                pdfViewerControl.RemoveAnnotation(selectedShapeAnnotation);



            shapeCustomBar.IsVisible = false;

            annotationToolbar.IsVisible = true;
        }
        
        private void BackShapeCustomButton_Clicked()
        {


            ValidButton.IsVisible = false;

            shapeCustomBar.IsVisible = false;

            annotationToolbar.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.None;
        }
        #endregion

        #region Select Text markup events methods

        private void StriketroughtButton_Clicked()
        {
            textMarkupBar.TextMarkupButtonSatusIcon.Source = "strikethrough.png";

            this.StatusColor = Color.FromHex("#b4b4b4");

            pdfViewerControl.AnnotationMode = AnnotationMode.Strikethrough;

            this.SelectedColor = Color.Black;

            selectTextMarkupBar.IsVisible = false;

            ValidButton.IsVisible = true;
            UndoButton.IsVisible = true;
            RedoButton.IsVisible = true;

            textMarkupBar.IsVisible = true;
        }

        private void UnderlineButton_Clicked()
        {
            textMarkupBar.TextMarkupButtonSatusIcon.Source = "underline.png";

            this.StatusColor = Color.FromHex("#b4b4b4");

            pdfViewerControl.AnnotationMode = AnnotationMode.Underline;

            this.SelectedColor = Color.Black;

            selectTextMarkupBar.IsVisible = false;

            ValidButton.IsVisible = true;
            UndoButton.IsVisible = true;
            RedoButton.IsVisible = true;

            textMarkupBar.IsVisible = true;
        }

        private void HightlightButton_Clicked()
        {
            textMarkupBar.TextMarkupButtonSatusIcon.Source = "draw.png";

            this.StatusColor = Color.FromHex("#b4b4b4");

            pdfViewerControl.AnnotationMode = AnnotationMode.Highlight;

            this.SelectedColor = Color.Black;

            ValidButton.IsVisible = true;
            UndoButton.IsVisible = true;
            RedoButton.IsVisible = true;

            selectTextMarkupBar.IsVisible = false;

            textMarkupBar.IsVisible = true;
        }

        private void BackSelectTextMarkupButton_Clicked()
        {
            selectTextMarkupBar.IsVisible = false;

            annotationToolbar.IsVisible = true;
        }
        #endregion

        #region Text markup event methods 

        private void BackTextMarkupButton_Clicked()
        {
            #region Remove edit Text markup bar
            if(textMarkupBar.TrashCanButton.IsVisible == true)
                textMarkupBar.TrashCanButton.IsVisible = false;

            if(textMarkupBar.TextMarkupIcon.IsVisible == true)
                textMarkupBar.TextMarkupIcon.IsVisible = false;

            if(textMarkupBar.TextMarkupButtonStatus.IsVisible == false)
                textMarkupBar.TextMarkupButtonStatus.IsVisible = true;

            AbsoluteLayout.SetLayoutBounds(textMarkupBar.ColorButton, new Rectangle(0.95, 0.5, -1, -1));

            textMarkupBar.IsVisible = false;
            #endregion

            annotationToolbar.IsVisible = true;
        }

        private void TrashTextMarkupButton_Clicked()
        {
            if(selectedTextMarkupAnnotation != null)
            {
                pdfViewerControl.RemoveAnnotation(selectedTextMarkupAnnotation);


                #region Remove edit Text markup bar
                textMarkupBar.TrashCanButton.IsVisible = false;
                textMarkupBar.TextMarkupIcon.IsVisible = false;
                textMarkupBar.TextMarkupButtonStatus.IsVisible = true;

                AbsoluteLayout.SetLayoutBounds(textMarkupBar.ColorButton, new Rectangle(0.95, 0.5, -1, -1));

                textMarkupBar.IsVisible = false;
                #endregion

                annotationToolbar.IsVisible = true;
            }
        }

        private void TextMarkupStatusButton_Clicked()
        {
            if (pdfViewerControl.AnnotationMode == AnnotationMode.None)
            {
                this.StatusColor = Color.FromHex("#b4b4b4");
                pdfViewerControl.AnnotationMode = lastTextMarkupAnnotationMode;
            }
            else
            {
                this.StatusColor = Color.Transparent;
                lastTextMarkupAnnotationMode = pdfViewerControl.AnnotationMode;
                pdfViewerControl.AnnotationMode = AnnotationMode.None;
            }
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
            if(annotationType == AnnotationMode.FreeText)
            {
                popupAnnotationTools.SfPopupLayout.IsOpen = false;

                stylePopup.SfPopupLayout.StaysOpen = true;

                stylePopup.SfPopupLayout.Show(lastRectangleBounds.Left / 2, lastRectangleBounds.Top / 2);
            }

            else
            {
                if(annotationType == AnnotationMode.Ink)
                {
                    stylePopupContent.ThicknessBar.IsVisible = true;
                    stylePopupContent.FontSizeSlider.IsVisible = false;

                    popupAnnotationTools.SfPopupLayout.IsOpen = false;

                    stylePopup.SfPopupLayout.StaysOpen = true;

                    stylePopup.SfPopupLayout.Show(lastRectangleBounds.Left / 2 - 60, lastRectangleBounds.Top / 2 );
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
                        if(selectedTextMarkupAnnotation != null)
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

        #region

        #endregion

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}