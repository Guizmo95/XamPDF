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
        #region Content view / View
        private readonly Stream fileStream;
        private readonly PdfViewerModel pdfViewerModel;
        private readonly RotatorPage rotatorPage;
        private readonly FreeTextBottomBar freeTextBottomBar;
        private readonly FontSizeSlider fontSizeSlider;
        private readonly EditFreeTextBar editFreeTextBar;
        private readonly InkBottomBar inkBottomBar;
        private readonly ThicknessBar thicknessBar;
        private readonly EditInkBar editInkBar;
        private readonly AnnotationToolbar annotationToolbar;
        private readonly OpacitySlider opacitySlider;
        private readonly ShapeBar shapeBar;
        #endregion

        private FreeTextAnnotation selectedFreeTextAnnotation;
        private InkAnnotation selectedInkAnnotation;
       

        #region Property
        private Color selectedColor = Color.Black;
        private Color statusColor = Color.FromHex("b4b4b4");
        private int fontSize = 6;
        private float opacity = 100;
        private bool canUndoInk = false;
        private bool canRedoInk = false;
        private int shapesNumbers = 0;

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

                if(selectedFreeTextAnnotation != null)
                    selectedFreeTextAnnotation.Settings.TextColor = value;

                if(pdfViewerControl.AnnotationMode == AnnotationMode.FreeText)
                    pdfViewerControl.AnnotationSettings.FreeText.TextColor = value;

                if (selectedInkAnnotation != null)
                    selectedInkAnnotation.Settings.Color = value;

                if (pdfViewerControl.AnnotationMode == AnnotationMode.Ink)
                    pdfViewerControl.AnnotationSettings.Ink.Color = value;

                if (rotatorPage.IsVisible == true)
                    rotatorPage.IsVisible = false;

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
                fontSize = value;

                if(selectedFreeTextAnnotation != null)
                    selectedFreeTextAnnotation.Settings.TextSize = value;

                if(pdfViewerControl.AnnotationMode == AnnotationMode.FreeText)
                    pdfViewerControl.AnnotationSettings.FreeText.TextSize = value;

                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
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
                // Call OnPropertyChanged whenever the property is updated
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

                OnPropertyChanged();
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

                if (value == true)
                    RedoButton.Foreground = Color.Black;
                else
                {
                    if(value == false)
                        RedoButton.Foreground = Color.White;
                }

                OnPropertyChanged();
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

                if (selectedInkAnnotation != null)
                    selectedInkAnnotation.Settings.Opacity = value;

                OnPropertyChanged();
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

                if (ShapesNumbers == 0)
                    ValidButton.Foreground = Color.White;
                else
                    ValidButton.Foreground = Color.FromHex("#b4b4b4");

                OnPropertyChanged();
            }
        }
        #endregion

        public PdfViewer(Stream fileStream)
        {
            InitializeComponent();

            #region Instanciation
            this.fileStream = fileStream;

            this.annotationToolbar = new AnnotationToolbar();

            this.rotatorPage = new RotatorPage(RotatorMode.ColorPicker);

            this.freeTextBottomBar = new FreeTextBottomBar();
            this.freeTextBottomBar.BindingContext = this;

            this.fontSizeSlider = new FontSizeSlider();
            this.fontSizeSlider.BindingContext = this;

            this.editFreeTextBar = new EditFreeTextBar();
            this.editFreeTextBar.BindingContext = this;

            this.inkBottomBar = new InkBottomBar();
            this.inkBottomBar.BindingContext = this;

            this.thicknessBar = new ThicknessBar();

            this.editInkBar = new EditInkBar();
            this.editInkBar.BindingContext = this;

            this.opacitySlider = new OpacitySlider();
            this.opacitySlider.BindingContext = this;

            this.shapeBar = new ShapeBar();
            #endregion

            #region Pdf viewer events
            pdfViewerControl.FreeTextAnnotationAdded += PdfViewerControl_FreeTextAnnotationAdded;
            pdfViewerControl.FreeTextPopupDisappeared += PdfViewerControl_FreeTextPopupDisappeared;
            pdfViewerControl.FreeTextAnnotationSelected += PdfViewerControl_FreeTextAnnotationSelected;
            pdfViewerControl.FreeTextAnnotationDeselected += PdfViewerControl_FreeTextAnnotationDeselected;
            pdfViewerControl.CanRedoInkModified += PdfViewerControl_CanRedoInkModified;
            pdfViewerControl.CanUndoInkModified += PdfViewerControl_CanUndoInkModified;

            pdfViewerControl.InkSelected += PdfViewerControl_InkSelected;
            pdfViewerControl.InkDeselected += PdfViewerControl_InkDeselected;

            pdfViewerControl.ShapeAnnotationAdded += PdfViewerControl_ShapeAnnotationAdded;

            RedoButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if(CanRedoInk == true)
                    {
                        RedoButton.Foreground = Color.FromHex("#b4b4b4");
                        Redo();
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
                    if(CanUndoInk == true)
                    {
                        UndoButton.Foreground = Color.FromHex("#b4b4b4");
                        Undo();
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
                        ValidButton.Foreground = Color.FromHex("#b4b4b4");

                        if(pdfViewerControl.AnnotationMode == AnnotationMode.Ink)
                            SaveInk();

                        await Task.Delay(100);

                        CanRedoInk = false;
                        CanUndoInk = false;
                        StatusColor = Color.Transparent;
                    }

                    if (pdfViewerControl.AnnotationMode == AnnotationMode.Arrow
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Line
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Circle
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Rectangle)
                    {
                        pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    }
                })
            });
            #endregion

            #region Shape bottom bar events
            shapeBar.ArrowButtonClicked += ArrowButton_Clicked;
            shapeBar.LineButtonClicked += LineButton_Clicked;
            shapeBar.EllipseButtonClicked += EllipseButton_Clicked;
            shapeBar.RectangleButtonClicked += RectangleButton_Clicked;
            shapeBar.BackButtonClicked += BackShapeButton_Clicked;
            #endregion

            #region Ink bottom bar events 
            inkBottomBar.ColorButtonClicked += ColorInkButton_Clicked;
            inkBottomBar.BackButtonClicked += BackPenButton_Clicked;
            inkBottomBar.PenSizeButtonClicked += InkThicknessButton_Clicked;
            inkBottomBar.PenStatusButtonClicked += InkStatusButton_Clicked;
            #endregion

            #region Ink bottom bar edit events 
            editInkBar.ColorButtonClicked += ColorInkButton_Clicked;
            editInkBar.TrashButtonClicked += TrashButton_Clicked;
            editInkBar.InkSizeButtonClicked += InkThicknessButton_Clicked;
            editInkBar.BackButtonClicked += BackInkEditButton_Clicked;
            #endregion

            #region Free Text button events 
            //TextButtonClickedBottomBar events
            freeTextBottomBar.ColorButtonClicked += ColorTextButton_Clicked;
            freeTextBottomBar.FontSizeButtonClicked += FontSizeButton_Clicked;
            freeTextBottomBar.BackButtonClicked += BackTextButton_Clicked;
            freeTextBottomBar.FreeTextButtonClicked += FreeTextButtonBottomBar_Clicked;
            #endregion

            #region Free Text edit button events
            //EditFreeTextBar events
            editFreeTextBar.TrashCanButtonClicked += TrashCanButton_Clicked;
            editFreeTextBar.FontSizeButtonClicked += FontSizeButton_Clicked;
            editFreeTextBar.ColorButtonClicked += ColorTextButton_Clicked;
            editFreeTextBar.BackButtonClicked += BackButtonInFreeTextEditor_Clicked;
            editFreeTextBar.ChangeTextInFreeTextSelectedButtonClicked += ChangeFreeTextSelected;

            MessagingCenter.Subscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor", (sender, helper) =>
            {
                this.SelectedColor = helper;
            });
            #endregion

            #region ThicknessBarEvent
            thicknessBar.FirstBoxViewButtonClicked += FirstBoxView_Clicked;
            thicknessBar.SecondBoxViewButtonClicked += SecondBoxView_Clicked;
            thicknessBar.ThirdBoxViewButtonClicked += ThirdBoxView_Clicked;
            thicknessBar.FourthBoxViewButtonClicked += FourthBoxView_Clicked;
            thicknessBar.FifthBoxViewButtonClicked += FifthBoxView_Clicked;
            #endregion

            #region Add content view but set them invisible 
            annotationToolbar.IsVisible = false;
            mainStackLayout.Children.Insert(2, annotationToolbar);
            rotatorPage.IsVisible = false;
            mainStackLayout.Children.Add(rotatorPage);
            fontSizeSlider.IsVisible = false;
            mainStackLayout.Children.Insert(3, fontSizeSlider);
            opacitySlider.IsVisible = false;
            mainStackLayout.Children.Insert(3, opacitySlider);
            freeTextBottomBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, freeTextBottomBar);
            editFreeTextBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, editFreeTextBar);
            inkBottomBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, inkBottomBar);
            thicknessBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, thicknessBar);
            editInkBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, editInkBar);
            shapeBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, shapeBar);

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
        private void PdfViewerControl_ShapeAnnotationAdded(object sender, ShapeAnnotationAddedEventArgs args)
        {
            this.ShapesNumbers += 1;
        }

        private void AnnotationToolbar_Clicked()
        {
            //Set events
            annotationToolbar.StampButtonClicked += StampButton_Clicked;
            annotationToolbar.FreeTextButtonClicked += FreeTextButton_Clicked;
            annotationToolbar.InkButtonClicked += InkButton_Clicked;
            annotationToolbar.ShapeButtonClicked += ShapeButton_Clicked;
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
            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            if (fontSizeSlider.IsVisible == true)
                fontSizeSlider.IsVisible = false;

            editFreeTextBar.IsVisible = false;
            annotationToolbar.IsVisible = true;
        }

        private void PdfViewerControl_FreeTextAnnotationSelected(object sender, FreeTextAnnotationSelectedEventArgs args)
        {
            if(freeTextBottomBar.IsVisible == true)
                freeTextBottomBar.IsVisible = false;

            if (annotationToolbar.IsVisible == true)
                annotationToolbar.IsVisible = false;

            if (inkBottomBar.IsVisible == true)
                inkBottomBar.IsVisible = false;

            if (editInkBar.IsVisible == true)
                editInkBar.IsVisible = false;
                
            if(bottomToolbar.IsVisible == true)
                bottomToolbar.IsVisible = false;

            editFreeTextBar.IsVisible = true;

            //Cast the sender object to FreeTextAnnotation
            selectedFreeTextAnnotation = sender as FreeTextAnnotation;
        }

        private void PdfViewerControl_FreeTextPopupDisappeared(object sender, FreeTextPopupDisappearedEventArgs args)
        {
            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            if (fontSizeSlider.IsVisible == true)
                fontSizeSlider.IsVisible = false;

            freeTextBottomBar.IsVisible = false;

            annotationToolbar.IsVisible = true;
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

        private void Undo()
        {
            if (CanUndoInk == true)
                pdfViewerControl.UndoInk();

        }

        private void Redo()
        {
            if (CanRedoInk == true)
                pdfViewerControl.RedoInk();
        }

        private void SaveInk()
        {
            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            if (thicknessBar.IsVisible == true)
                thicknessBar.IsVisible = false;

            inkBottomBar.IsVisible = false;

            annotationToolbar.IsVisible = true;

            pdfViewerControl.EndInkSession(true);
        }

        private void PdfViewerControl_InkSelected(object sender, InkSelectedEventArgs args)
        {
            //Casts the sender object as Ink annotation.
            selectedInkAnnotation = sender as InkAnnotation;

            if (annotationToolbar.IsVisible == true)
                annotationToolbar.IsVisible = false;

            if (freeTextBottomBar.IsVisible == true)
                freeTextBottomBar.IsVisible = false;

            if (editFreeTextBar.IsVisible == true)
                editFreeTextBar.IsVisible = false;

            if (inkBottomBar.IsVisible == true)
                inkBottomBar.IsVisible = false;

            if (bottomToolbar.IsVisible == true)
                bottomToolbar.IsVisible = false;

            editInkBar.IsVisible = true;
        }

        private void PdfViewerControl_InkDeselected(object sender, InkDeselectedEventArgs args)
        {
            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            if (thicknessBar.IsVisible == true)
                thicknessBar.IsVisible = false;

            editInkBar.IsVisible = false;
            annotationToolbar.IsVisible = true;
        }
        #endregion

        #region FreeTextBottomBar events methods
        private void FontSizeButton_Clicked()
        {
            if (fontSizeSlider.IsVisible == true)
            {
                fontSizeSlider.IsVisible = false;
            }
            else
            {
                if (rotatorPage.IsVisible == true)
                    fontSizeSlider.IsVisible = false;

                fontSizeSlider.IsVisible = true;
            }
        }

        private void ColorTextButton_Clicked()
        {
            if (rotatorPage.IsVisible == true)
            {
                rotatorPage.IsVisible = false;
            }
            else
            {
                if (fontSizeSlider.IsVisible == true)
                    fontSizeSlider.IsVisible = false;

                rotatorPage.IsVisible = true;
            }
        }

        private void BackTextButton_Clicked()
        {
            if(fontSizeSlider.IsVisible == true)
                fontSizeSlider.IsVisible = false;

            if(rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            freeTextBottomBar.IsVisible = false;

            annotationToolbar.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.None;
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
        #endregion

        #region EditFreeTextBar events methods
        private void TrashCanButton_Clicked()
        {
            if(selectedFreeTextAnnotation != null)
            {
                //Delete the selected free text annotation
                pdfViewerControl.RemoveAnnotation(selectedFreeTextAnnotation);

                if (rotatorPage.IsVisible == true)
                    rotatorPage.IsVisible = false;

                if (fontSizeSlider.IsVisible == true)
                    fontSizeSlider.IsVisible = false;

                editFreeTextBar.IsVisible = false;

                annotationToolbar.IsVisible = true;
            }
        }

        private void BackButtonInFreeTextEditor_Clicked()
        {
            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            if (fontSizeSlider.IsVisible == true)
                fontSizeSlider.IsVisible = false;

            editFreeTextBar.IsVisible = false;

            annotationToolbar.IsVisible = true;

            //Trick for deselect free text
            pdfViewerControl.AnnotationMode = AnnotationMode.FreeText;
            pdfViewerControl.AnnotationMode = AnnotationMode.None;
        }

        //TODO -- LOOK FOR CHANGE TEXT OF FREE TEXT SELECTED
        private void ChangeFreeTextSelected()
        {
            

        }
        #endregion

        #region Annotation toolbar events

        private void ShapeButton_Clicked()
        {
            if (annotationToolbar.IsVisible == true)
                annotationToolbar.IsVisible = false;

            if (shapeBar.IsVisible == false)
                shapeBar.IsVisible = true;

            ValidButton.IsVisible = true;
        }

        private void BackAnnotationToolbarButton_Clicked()
        {
            //Remove events
            annotationToolbar.StampButtonClicked -= StampButton_Clicked;
            annotationToolbar.FreeTextButtonClicked -= FreeTextButton_Clicked;
            annotationToolbar.InkButtonClicked -= InkButton_Clicked;
            annotationToolbar.ShapeButtonClicked -= ShapeButton_Clicked;
            annotationToolbar.BackButtonClicked -= BackAnnotationToolbarButton_Clicked;

            annotationToolbar.IsVisible = false;

            bottomToolbar.IsVisible = true;
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
        private void ColorInkButton_Clicked()
        {
            if (rotatorPage.IsVisible == true)
            {
                rotatorPage.IsVisible = false;
            }
            else
            {
                if (thicknessBar.IsVisible == true)
                    thicknessBar.IsVisible = false;

                rotatorPage.IsVisible = true;
            }
        }

        private void InkThicknessButton_Clicked()
        {
            if (thicknessBar.IsVisible == false)
            {
                if (rotatorPage.IsVisible == true)
                    rotatorPage.IsVisible = false;

                thicknessBar.IsVisible = true;
            }

            else
                if (thicknessBar.IsVisible == true)
                thicknessBar.IsVisible = false;
        }


        private void BackPenButton_Clicked()
        {

            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            if (thicknessBar.IsVisible == true)
                thicknessBar.IsVisible = false;

            ValidButton.IsVisible = false;
            UndoButton.IsVisible = false;
            RedoButton.IsVisible = false;

            inkBottomBar.IsVisible = false;

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

        #region Ink edit bottom bar events methods
        private void TrashButton_Clicked()
        {
            if (selectedInkAnnotation != null)
                //Removes the selected annotation from the PDF viewer.
                pdfViewerControl.RemoveAnnotation(selectedInkAnnotation);

            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            if (thicknessBar.IsVisible == true)
                thicknessBar.IsVisible = false;

            editInkBar.IsVisible = false;

            annotationToolbar.IsVisible = true;
        }


        private void BackInkEditButton_Clicked()
        {
            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            if (thicknessBar.IsVisible == true)
                thicknessBar.IsVisible = false;

            if(selectedInkAnnotation != null)
                //Trick for deselect and re-select ink annotation 
                pdfViewerControl.AnnotationMode = AnnotationMode.Ink;

            this.StatusColor = Color.FromHex("b4b4b4");

            editInkBar.IsVisible = false;

            annotationToolbar.IsVisible = true;
        }
        #endregion

        #region Shape events methods

        private void ArrowButton_Clicked()
        {
            pdfViewerControl.AnnotationMode = AnnotationMode.Arrow;
        }

        private void LineButton_Clicked()
        {
            pdfViewerControl.AnnotationMode = AnnotationMode.Line;
        }

        private void EllipseButton_Clicked()
        {
            pdfViewerControl.AnnotationMode = AnnotationMode.Circle;
        }

        private void RectangleButton_Clicked()
        {
            pdfViewerControl.AnnotationMode = AnnotationMode.Rectangle;
        }

        private void BackShapeButton_Clicked()
        {
            shapeBar.IsVisible = false;

            pdfViewerControl.AnnotationMode = AnnotationMode.None;

            ValidButton.IsVisible = false;

            annotationToolbar.IsVisible = true;
        }

        #endregion

        #region ThicknessBarEvents
        private void FirstBoxView_Clicked()
        {
            if (pdfViewerControl.AnnotationMode == AnnotationMode.Ink)
                pdfViewerControl.AnnotationSettings.Ink.Thickness = 1;

            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Thickness = 1;
        }
        private void SecondBoxView_Clicked()
        {
            if (pdfViewerControl.AnnotationMode == AnnotationMode.Ink)
                pdfViewerControl.AnnotationSettings.Ink.Thickness = 3;

            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Thickness = 3;
        }

        private void ThirdBoxView_Clicked()
        {
            if (pdfViewerControl.AnnotationMode == AnnotationMode.Ink)
                pdfViewerControl.AnnotationSettings.Ink.Thickness = 5;

            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Thickness = 5;
        }

        private void FourthBoxView_Clicked()
        {
            if (pdfViewerControl.AnnotationMode == AnnotationMode.Ink)
                pdfViewerControl.AnnotationSettings.Ink.Thickness = 7;

            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Thickness = 7;
        }

        private void FifthBoxView_Clicked()
        {
            if (pdfViewerControl.AnnotationMode == AnnotationMode.Ink)
                pdfViewerControl.AnnotationSettings.Ink.Thickness = 9;

            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Thickness = 9;
        }

        #endregion

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}