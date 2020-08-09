using Pdf.controls;
using Pdf.Enumerations;
using Pdf.Interfaces;
using Pdf.Models;
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
using Acr.UserDialogs;
using SlideOverKit;
using System.Collections.ObjectModel;
using System.Threading;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfViewer : MenuContainerPage, INotifyPropertyChanged
    {
        #region Private Members
        private Stream pdfStream;
        private readonly LoadingMode loadingMode;
        private readonly StampSlideUpMenu stampSlideUpMenu;

        private SfPopupLayout stylePopup;
        private SfPopupLayout errorSearchPopup;
        private SfPopupLayout passwordPopup;
        private SfPopupLayout popupMenu;

        private StyleContent styleContent;
        private PopupMenuContent popupMenuContent;
        private SearchErrorPopup searchErrorPopupContent;

        private DataTemplate styleTemplateStylePopup;
        private DataTemplate styleTemplateErrorPopup;
        private DataTemplate templateViewSetPasswordPopup;

        private ImageButton image;
        private Label label;
        private Entry entry;
        private Button acceptButton;
        private Button declineButton;

        private FreeTextAnnotation selectedFreeTextAnnotation;
        private InkAnnotation selectedInkAnnotation;
        private HandwrittenSignature selectedHandwrittenSignature;
        private ShapeAnnotation selectedShapeAnnotation;
        private TextMarkupAnnotation selectedTextMarkupAnnotation;
        private StampAnnotation selectedStampAnnotation;

        private Color selectedColor = Color.Black;
        private AnnotationType annotationType;

        private readonly string filePath;

        private bool canSaveDocument;
        private bool canUndoInk;
        private bool canRedoInk;
        private bool searchTextInstanceStarted;
        private bool isInkMenuEnabled;
        private bool isFreeTextMenuEnabled;
        private bool isShapeMenuEnabled;
        private bool isTextMarkupMenuEnabled;
        private bool isHandwrittenSignatureMenuEnabled;

        private int fontSize = 6;
        private int lastOpacityValueSelected = 4;
        private int lastThicknessValueSelected = 5;
        private int annotationsNumbers;
        #endregion

        #region Property
        public bool CanLeave { get; set; }
        public bool ToolbarIsCollapsed { get; private set; }
        public bool IsATryToOpenDocument { get; private set; }
        public bool IsPasswordPopupInitialized { get; private set; }
        public bool HasAlreadyTriedToOpenDoc { get; private set; }
        public int ShapesNumbers { get; set; }
        public int TextMarkupNumbers { get; set; }

        public bool IsTextMarkupMenuEnabled
        {
            get => isTextMarkupMenuEnabled;

            set
            {
                isTextMarkupMenuEnabled = value;
                OnPropertyChanged();

                if (isTextMarkupMenuEnabled == false)
                {
                    BackButtonAnnotationTypeToolbar_Clicked(null, null);
                }
                else
                {
                    switch (annotationType)
                    {
                        case AnnotationType.Highlight:
                            HightlightButton_Clicked(null, null);
                            break;
                        case AnnotationType.Underline:
                            UnderlineButton_Clicked(null, null);
                            break;
                        case AnnotationType.Strikethrough:
                            StrikethroughtButton_Clicked(null, null);
                            break;
                    }
                }
            }
        }

        public bool IsShapeMenuEnabled
        {
            get => isShapeMenuEnabled;

            set
            {
                isShapeMenuEnabled = value;
                OnPropertyChanged();

                if (isShapeMenuEnabled == false)
                {
                    BackButtonAnnotationTypeToolbar_Clicked(null, null);
                }
                else
                {
                    switch (annotationType)
                    {
                        case AnnotationType.Rectangle:
                            RectangleButton_Clicked(null, null);
                            break;
                        case AnnotationType.Line:
                            LineButton_Clicked(null, null);
                            break;
                        case AnnotationType.Arrow:
                            ArrowButton_Clicked(null, null);
                            break;
                        case AnnotationType.Circle:
                            CircleButton_Clicked(null, null);
                            break;
                    }
                }
            }
        }

        public bool IsInkMenuEnabled
        {
            get => isInkMenuEnabled;

            set
            {
                isInkMenuEnabled = value;
                OnPropertyChanged();

                if (isInkMenuEnabled == false)
                    BackButtonAnnotationTypeToolbar_Clicked(null, null);
                else
                    SetToolbarForInkAnnotationSelected();
            }
        }

        public bool IsHandwrittenSignatureMenuEnabled
        {
            get => isHandwrittenSignatureMenuEnabled;

            set
            {
                isHandwrittenSignatureMenuEnabled = value;
                OnPropertyChanged();

                if (isHandwrittenSignatureMenuEnabled == false)
                    BackButtonAnnotationTypeToolbar_Clicked(null, null);
                else
                    SetToolbarForHandwrittenSignatureSelected();
            }
        }

        public bool IsFreeTextMenuEnabled
        {
            get => isFreeTextMenuEnabled;

            set
            {
                isFreeTextMenuEnabled = value;
                OnPropertyChanged();

                if (isFreeTextMenuEnabled == false)
                    BackButtonAnnotationTypeToolbar_Clicked(null, null);
                else
                    SetToolbarForFreeTextAnnotationSelected();
            }
        }

        public int NumberOfAnnotation
        {
            get => annotationsNumbers;

            set
            {
                annotationsNumbers = value;
                OnPropertyChanged();

                CanSaveDocument = annotationsNumbers != 0;
            }
        }

        public bool CanSaveDocument
        {
            get => canSaveDocument;

            set
            {
                canSaveDocument = value;
                OnPropertyChanged();

                var item = (ItemsMenu)popupMenuContent.ItemsMenu[0];

                if (canSaveDocument)
                {
                    item.TextColor = Color.FromHex("#616161");
                    item.ImageColor = Color.FromHex("#373737");
                }
                else
                {
                    item.TextColor = Color.FromHex("#e0e0e0");
                    item.ImageColor = Color.FromHex("#707070");
                }
            }
        }

        public SfPdfViewer PdfViewerControl
        {
            get => pdfViewerControl;

            set => pdfViewerControl = value;
        }

        public Color SelectedColor
        {
            get => selectedColor;

            set
            {
                selectedColor = value;

                OnPropertyChanged();
                ChangeSelectedAnnotationColor(value);
                ChangeAnnotationColor(value);
            }
        }

        private void ChangeSelectedAnnotationColor(Color value)
        {
            if (selectedFreeTextAnnotation != null)
                selectedFreeTextAnnotation.Settings.TextColor = value;

            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Color = value;

            if (selectedShapeAnnotation != null)
                selectedShapeAnnotation.Settings.StrokeColor = value;

            if (selectedTextMarkupAnnotation != null)
                selectedTextMarkupAnnotation.Settings.Color = value;
        }

        private void ChangeAnnotationColor(Color value)
        {
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
            }
        }

        public int FontSize
        {
            get => fontSize;

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
            get => canUndoInk;

            set
            {
                canUndoInk = value;
                OnPropertyChanged();

                if (value)
                {
                    UndoButton.Foreground = Color.FromHex("#373737");
                    ValidButton.Foreground = Color.FromHex("#373737");
                }
                else
                {
                    UndoButton.Foreground = Color.Transparent;
                    ValidButton.Foreground = Color.Transparent;
                }
            }
        }

        public bool CanRedoInk
        {
            get => canRedoInk;

            set
            {
                canRedoInk = value;
                OnPropertyChanged();

                RedoButton.Foreground = value 
                    ? Color.FromHex("#373737") 
                        : Color.Transparent;
            }
        }
        #endregion

        protected override bool OnBackButtonPressed()
        {
            if (CanSaveDocument && CanLeave == false)
            {
                DependencyService.Get<IToastMessage>().ShortAlert("Press a second time to exit");
                CanLeave = true;
            }
            else if (CanLeave || CanSaveDocument == false)
                return base.OnBackButtonPressed();

            return true;
        }

        protected override void OnDisappearing()
        {
            if (Device.RuntimePlatform == Device.Android)
                DependencyService.Get<IThemeManager>().SetMenuStatusBarColor();

            UnsubscribeFromEvents();
            pdfViewerControl.Unload();

            base.OnDisappearing();
        }

        private void UnsubscribeFromEvents()
        {
            MessagingCenter.Unsubscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor");

            pdfViewerControl.DocumentLoaded -= PdfViewerControl_DocumentLoaded;

            styleContent.ThicknessBar.ThicknessBarClicked -= ThicknessBar_Clicked;
            styleContent.OpacityButtonClicked -= OpacityIcon_Clicked;

            stampSlideUpMenu.StampListView.SelectionChanged -= StampListView_SelectionChanged;

            if (passwordPopup != null)
            {
                passwordPopup.Closed -= PasswordPopup_Closed;
                acceptButton.Clicked -= AcceptButtonPasswordPopup_Clicked;
                declineButton.Clicked -= DeclineButton_Clicked;

                passwordPopup.IsOpen = false;
            }
        }

        protected override void OnAppearing()
        {
            // I disabled the nav bar here because the nav bar will be removed
            // before the document start loading -> Bad for UI
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            if (loadingMode == LoadingMode.ByIntent)
            {
                // I disabled the tab bar here because the nav bar will be removed
                // before the document start loading -> Bad for UI
                Shell.SetTabBarIsVisible(this, false);

                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<IThemeManager>().SetPdfViewerStatusBarColor();
            }
            else
            {
                UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);
            }

            new Task(LoadPdf).Start();

            base.OnAppearing();
        }

        private async void LoadPdf()
        {
            if (loadingMode == LoadingMode.ByIntent)
                UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);

            if (Device.RuntimePlatform == Device.Android)
            {
                pdfStream = DependencyService.Get<IAndroidFileHelper>().GetFileStream(filePath);
               
                // PDFium renderer
                pdfViewerControl.CustomPdfRenderer = DependencyService.Get<ICustomPdfRendererService>().AlternatePdfRenderer;
            }

            await Task.Run(async () =>
            {
                await pdfViewerControl.LoadDocumentAsync(pdfStream, new CancellationTokenSource());
            });
        }

        public PdfViewer(string filePath, LoadingMode loadingMode)
        {
            InitializeComponent();

            this.loadingMode = loadingMode;
            this.filePath = filePath;

            stampSlideUpMenu = new StampSlideUpMenu();
            stampSlideUpMenu.StampListView.SelectionChanged += StampListView_SelectionChanged;
            this.SlideMenu = stampSlideUpMenu;

            annotationType = AnnotationType.None;

            pdfViewerControl.IsToolbarVisible = false;
            pdfViewerControl.IsPasswordViewEnabled = false;
            pdfViewerControl.PreserveSignaturePadOrientation = true;

            MessagingCenter.Subscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor", (sender, helper) =>
            {
                this.SelectedColor = helper;
            });
        }

        private void PdfViewerControl_DocumentLoaded(object sender, EventArgs args)
        {
            if (IsPasswordPopupInitialized != true)
            {
                Shell.SetNavBarIsVisible(this, false);
                NavigationPage.SetHasNavigationBar(this, false);
            }

            if (Device.RuntimePlatform == Device.Android)
                DependencyService.Get<IThemeManager>().SetPdfViewerStatusBarColor();

            Shell.SetTabBarIsVisible(this, false);
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);

            AddGestureRecognizers();
            
            InitializeStylePopup();
            InitializeSearchErrorPopup();
            InitializePopupMenu();

            SetBindingContext();

            UserDialogs.Instance.HideLoading();
        }

        private void AddGestureRecognizers()
        {
            RedoButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if (CanRedoInk)
                    {
                        RedoInk();
                        await Task.Delay(100);

                        RedoButton.Foreground = CanRedoInk 
                            ? Color.FromHex("#373737") 
                                : Color.Transparent;
                    }
                })
            });

            UndoButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if (CanUndoInk)
                    {
                        UndoInk();
                        await Task.Delay(100);

                        UndoButton.Foreground = CanUndoInk 
                            ? Color.FromHex("#373737") 
                                : Color.Transparent;
                    }
                })
            });

            ValidButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if (CanUndoInk)
                    {
                        ValidButton.Foreground = Color.Transparent;
                        RedoButton.Foreground = Color.Transparent;
                        UndoButton.Foreground = Color.Transparent;

                        if (pdfViewerControl.AnnotationMode == AnnotationMode.Ink)
                            SaveInk();

                        pdfViewerControl.AnnotationMode = AnnotationMode.None;

                        await Task.Delay(100);

                        CanRedoInk = false;
                        CanUndoInk = false;
                    }

                    if (ShapeAnnotationsSelected() || TextMarkupAnnotationsSelected())
                    {
                        pdfViewerControl.AnnotationMode = AnnotationMode.None;

                        ValidButton.Foreground = Color.Transparent;
                    }
                })
            });

            paletteButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(PaletteButton_Clicked)
            });

            trashButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(TrashButton_Clicked)
            });
        }

        private void SetBindingContext()
        {
            paletteButton.BindingContext = this;
            trashButton.BindingContext = this;
            styleContent.BindingContext = this;
            styleContent.ThicknessBar.BindingContext = this;
        }

        private void BookmarkButton_Clicked(object sender, EventArgs e)
        {
            pdfViewerControl.BookmarkPaneVisible = true;
        }

        private async void PdfViewerControl_DoubleTapped(object sender, TouchInteractionEventArgs e)
        {
            if (ToolbarIsCollapsed)
            {
                pdfViewerControl.AnnotationSettings.IsLocked = false;

                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<INavBarHelper>().SetDefaultNavBar();

                await RevealToolbar();
            }
            else
            {
                pdfViewerControl.AnnotationSettings.IsLocked = true;

                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<INavBarHelper>().SetImmersiveMode();

                await CollapseToolbar();
            }
        }

        private async Task RevealToolbar()
        {
            await topToolbar.FadeTo(1, 150);
            await bottomMainBar.FadeTo(1, 150);

            await Task.Delay(150);

            PdfViewGrid.Margin = new Thickness(0, 45, 0, 45);

            ToolbarIsCollapsed = false;
        }

        private async Task CollapseToolbar()
        {
            PdfViewGrid.Margin = 0;

            await topToolbar.FadeTo(0, 150);
            await bottomMainBar.FadeTo(0, 150);

            ToolbarIsCollapsed = true;
        }

        private void PdfViewerControl_PageChanged(object sender, PageChangedEventArgs args)
        {
            switch (annotationType)
            {
                case AnnotationType.Ink:
                    //It break the code shouldn't be triggered after selecting ink annotatation
                    pdfViewerControl.AnnotationMode = AnnotationMode.Ink;
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
                case AnnotationType.Highlight:
                    pdfViewerControl.AnnotationMode = AnnotationMode.Highlight;
                    break;
                case AnnotationType.Underline:
                    pdfViewerControl.AnnotationMode = AnnotationMode.Underline;
                    break;
                case AnnotationType.Strikethrough:
                    pdfViewerControl.AnnotationMode = AnnotationMode.Strikethrough;
                    break;
                case AnnotationType.None:
                    break;
            }
        }

        #region Annotations Selected Event Handlers 
        private void PdfViewerControl_StampAnnotationSelected(object sender, StampAnnotationSelectedEventArgs e)
        {
            SetToolbarForStampAnnotationSelected();

            selectedStampAnnotation = sender as StampAnnotation;

            annotationType = AnnotationType.Stamp;
        }

        private void PdfViewerControl_TextMarkupSelected(object sender, TextMarkupSelectedEventArgs args)
        {
            selectedTextMarkupAnnotation = sender as TextMarkupAnnotation;

            if (selectedTextMarkupAnnotation != null)
                SelectedColor = selectedTextMarkupAnnotation.Settings.Color;

            switch (args.TextMarkupAnnotationType)
            {
                case TextMarkupAnnotationType.Strikethrough:
                    annotationType = AnnotationType.Strikethrough;
                    break;
                case TextMarkupAnnotationType.Underline:
                    annotationType = AnnotationType.Underline;
                    break;
                case TextMarkupAnnotationType.Highlight:
                    annotationType = AnnotationType.Highlight;
                    break;
            }

            IsTextMarkupMenuEnabled = true;
        }

        private void PdfViewerControl_ShapeAnnotationSelected(object sender, ShapeAnnotationSelectedEventArgs args)
        {
            selectedShapeAnnotation = sender as ShapeAnnotation;

            if (selectedShapeAnnotation != null)
                SelectedColor = selectedShapeAnnotation.Settings.StrokeColor;

            switch (args.AnnotationType)
            {
                case AnnotationMode.Rectangle:
                    annotationType = AnnotationType.Rectangle;
                    break;
                case AnnotationMode.Circle:
                    annotationType = AnnotationType.Circle;
                    break;
                case AnnotationMode.Line:
                    annotationType = AnnotationType.Line;
                    break;
                case AnnotationMode.Arrow:
                    annotationType = AnnotationType.Arrow;
                    break;
            }

            IsShapeMenuEnabled = true;
        }

        private void PdfViewerControl_InkSelected(object sender, InkSelectedEventArgs args)
        {
            if (args.IsSignature)
            {
                annotationType = AnnotationType.HandwrittenSignature;
                selectedHandwrittenSignature = sender as HandwrittenSignature;

                IsHandwrittenSignatureMenuEnabled = true;
            }
            else
            {
                annotationType = AnnotationType.Ink;
                selectedInkAnnotation = sender as InkAnnotation;

                SelectedColor = args.Color;

                IsInkMenuEnabled = true;
            }
        }

        private void PdfViewerControl_FreeTextAnnotationSelected(object sender, FreeTextAnnotationSelectedEventArgs args)
        {
            annotationType = AnnotationType.FreeText;
            selectedFreeTextAnnotation = sender as FreeTextAnnotation;

            SelectedColor = args.TextColor;

            IsFreeTextMenuEnabled = true;
        }
        #endregion

        #region Annotation Deselected Event Handlers
        private void PdfViewerControl_StampAnnotationDeselected(object sender, StampAnnotationDeselectedEventArgs e)
        {
            BackButtonAnnotationTypeToolbar_Clicked(null, null);
        }

        private void PdfViewerControl_TextMarkupDeselected(object sender, TextMarkupDeselectedEventArgs args)
        {
            IsTextMarkupMenuEnabled = false;

            selectedShapeAnnotation = null;
        }

        private void PdfViewerControl_ShapeAnnotationDeselected(object sender, ShapeAnnotationDeselectedEventArgs args)
        {
            IsShapeMenuEnabled = false;

            selectedShapeAnnotation = null;
        }

        private void PdfViewerControl_FreeTextAnnotationDeselected(object sender, FreeTextAnnotationDeselectedEventArgs args)
        {
            IsFreeTextMenuEnabled = false;

            selectedFreeTextAnnotation = null;
        }

        private void PdfViewerControl_InkDeselected(object sender, InkDeselectedEventArgs args)
        {
            IsInkMenuEnabled = false;
            IsHandwrittenSignatureMenuEnabled = false;

            selectedInkAnnotation = null;
            selectedHandwrittenSignature = null;
        }
        #endregion

        #region Annotation Added Events Handler 
        private void PdfViewerControl_FreeTextAnnotationAdded(object sender, FreeTextAnnotationAddedEventArgs args)
        {
            NumberOfAnnotation += 1;

            BackButtonAnnotationTypeToolbar_Clicked(null, null);
        }

        private void PdfViewerControl_ShapeAnnotationAdded(object sender, ShapeAnnotationAddedEventArgs args)
        {
            NumberOfAnnotation += 1;

            ShapesNumbers += 1;
        }

        private void PdfViewerControl_StampAnnotationAdded(object sender, StampAnnotationAddedEventArgs e)
        {
            NumberOfAnnotation += 1;
        }
        private void PdfViewerControl_InkAdded(object sender, InkAddedEventArgs args)
        {
            NumberOfAnnotation += 1;
        }

        private void PdfViewerControl_TextMarkupAdded(object sender, TextMarkupAddedEventArgs args)
        {
            NumberOfAnnotation += 1;

            TextMarkupNumbers += 1;
        }
        #endregion

        #region Perform ink annotations
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
            if (CanUndoInk)
                pdfViewerControl.UndoInk();
        }

        private void RedoInk()
        {
            if (CanRedoInk)
                pdfViewerControl.RedoInk();
        }

        private async void SaveInk()
        {
            pdfViewerControl.EndInkSession(true);

            await Task.Run(async () =>
            {
                await annotationTypeToolbar.LayoutTo(new Rectangle(annotationTypeToolbar.Bounds.X, annotationTypeToolbar.Bounds.Y, annotationTypeToolbar.Bounds.Width, 0), 200, Easing.Linear);
            });

            CollapseAnnotationsToolbar();
        }

        private void CollapseAnnotationsToolbar()
        {
            annotationTypeToolbar.IsVisible = false;

            pdfViewerControl.AnnotationMode = AnnotationMode.None;
            annotationType = AnnotationType.None;
            bottomMainToolbar.IsVisible = true;

            viewModeButton.IsVisible = true;
            bookmarkButton.IsVisible = true;
            searchButton.IsVisible = true;
            moreOptionButton.IsVisible = true;

            ValidButton.IsVisible = false;
            UndoButton.IsVisible = false;
            RedoButton.IsVisible = false;
        }

        #endregion

        #region BottomMainBarMethods
        private void SignatureButton_Clicked(object sender, EventArgs e)
        {
            pdfViewerControl.AnnotationMode = AnnotationMode.HandwrittenSignature;
        }

        private void StampListView_SelectionChanged(object sender, Syncfusion.ListView.XForms.ItemSelectionChangedEventArgs e)
        {
            var stampItem = (StampModel)stampSlideUpMenu.StampListView.SelectedItem;
            var stamp = new Image {Source = stampItem.Image, WidthRequest = 350, HeightRequest = 250};

            pdfViewerControl.AddStamp(stamp, pdfViewerControl.PageNumber);

            HideMenu();
        }

        private void StampButton_Clicked(object sender, EventArgs e)
        {
            ShowMenu();
        }

        private void TextMarkupButton_Clicked(object sender, EventArgs e)
        {
            CollapseBottomMainToolbar(AnnotationType.TextMarkup);
        }

        private void ShapeButton_Clicked(object sender, EventArgs e)
        {
            CollapseBottomMainToolbar(AnnotationType.Shape);
        }

        private void InkButton_Clicked(object sender, EventArgs e)
        {
            CollapseBottomMainToolbar(AnnotationType.Ink);
        }

        private void FreeTextButton_Clicked(object sender, EventArgs e)
        {
            CollapseBottomMainToolbar(AnnotationType.FreeText);
        }

        #region Enable Annotation toolbar Methods 
        private void SetToolbarForShapeAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;
            textMarkupToolbar.IsVisible = false;

            styleContent.FontSizeControl.IsVisible = false;

            stylePopup.PopupView.HeightRequest = 195;
            stylePopup.PopupView.WidthRequest = 280;

            shapeToolbar.IsVisible = true;
            styleContent.SeparatorTwo.IsVisible = true;
            styleContent.Separator1.IsVisible = true;
            styleContent.OpacityControl.IsVisible = true;
            styleContent.ThicknessBar.IsVisible = true;
        }

        private void SetToolbarForFreeTextAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;
            shapeToolbar.IsVisible = false;
            textMarkupToolbar.IsVisible = false;
            styleContent.OpacityControl.IsVisible = false;
            styleContent.SeparatorTwo.IsVisible = false;
            styleContent.ThicknessBar.IsVisible = false;

            stylePopup.PopupView.HeightRequest = 130;
            stylePopup.PopupView.WidthRequest = 280;

            styleContent.FontSizeControl.IsVisible = true;
            styleContent.Separator1.IsVisible = true;
            paletteButton.IsVisible = true;
            annotationTypeToolbar.IsVisible = true;

            imageAnnotationType.Source = "twotone_title_24.xml";

            if (annotationType != AnnotationType.FreeText)
            {
                annotationType = AnnotationType.FreeText;
                pdfViewerControl.AnnotationMode = AnnotationMode.FreeText;
                pdfViewerControl.AnnotationSettings.FreeText.TextSize = 8;
                FontSize = 8;
                styleContent.ColorPicker.SelectedIndex = 0;
            }
            else
            {
                trashButton.IsVisible = true;
            }
        }

        private void SetToolbarForInkAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;
            shapeToolbar.IsVisible = false;
            textMarkupToolbar.IsVisible = false;
            styleContent.FontSizeControl.IsVisible = false;
            viewModeButton.IsVisible = false;
            bookmarkButton.IsVisible = false;
            searchButton.IsVisible = false;
            moreOptionButton.IsVisible = false;

            imageAnnotationType.Source = "pen.png";

            paletteButton.IsVisible = true;
            annotationTypeToolbar.IsVisible = true;
            styleContent.OpacityControl.IsVisible = true;
            styleContent.ThicknessBar.IsVisible = true;
            styleContent.SeparatorTwo.IsVisible = true;
            styleContent.Separator1.IsVisible = true;
            ValidButton.IsVisible = true;
            UndoButton.IsVisible = true;
            RedoButton.IsVisible = true;

            stylePopup.PopupView.HeightRequest = 195;
            stylePopup.PopupView.WidthRequest = 280;

            if (annotationType != AnnotationType.Ink)
            {
                annotationType = AnnotationType.Ink;
                pdfViewerControl.AnnotationMode = AnnotationMode.Ink;

                pdfViewerControl.AnnotationSettings.Ink.Thickness = 9;
                styleContent.ColorPicker.SelectedIndex = 0;
            }
            else
            {
                //If annotation type is not ink then it means that we are selecting an
                //existing annotation
                trashButton.IsVisible = true;
            }
        }

        private void SetToolbarForTextMarkupAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;
            shapeToolbar.IsVisible = false;

            textMarkupToolbar.IsVisible = true;

            styleContent.OpacityControl.IsVisible = false;
            styleContent.ThicknessBar.IsVisible = false;
            styleContent.SeparatorTwo.IsVisible = false;
            styleContent.Separator1.IsVisible = false;

            stylePopup.PopupView.HeightRequest = 62;
            stylePopup.PopupView.WidthRequest = 280;
        }

        private void SetToolbarForStampAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;
            shapeToolbar.IsVisible = false;
            textMarkupToolbar.IsVisible = false;

            paletteButton.IsVisible = false;
            annotationTypeToolbar.IsVisible = true;

            imageAnnotationType.Source = "stamp.png";

            trashButton.IsVisible = true;
        }

        private void SetToolbarForHandwrittenSignatureSelected()
        {
            bottomMainToolbar.IsVisible = false;
            shapeToolbar.IsVisible = false;
            textMarkupToolbar.IsVisible = false;

            paletteButton.IsVisible = false;
            annotationTypeToolbar.IsVisible = true;

            imageAnnotationType.Source = "signature.png";

            trashButton.IsVisible = true;
        }
        #endregion

        private void CollapseBottomMainToolbar(AnnotationType annotationType)
        {
            switch (annotationType)
            {
                case AnnotationType.Ink:
                    IsInkMenuEnabled = true;
                    break;
                case AnnotationType.FreeText:
                    IsFreeTextMenuEnabled = true;
                    break;
                case AnnotationType.Shape:
                    SetToolbarForShapeAnnotationSelected();
                    break;
                case AnnotationType.TextMarkup:
                    SetToolbarForTextMarkupAnnotationSelected();
                    break;
                case AnnotationType.None:
                    break;
            }
        }
        #endregion

        #region TextMarkup toolbar methods
        private async void BackButtonTextMarkupToolbar_Clicked(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                await textMarkupToolbar.LayoutTo(new Rectangle(textMarkupToolbar.Bounds.X, textMarkupToolbar.Bounds.Y, textMarkupToolbar.Bounds.Width, 0), 200, Easing.Linear);
            });

            imageAnnotationType.HeightRequest = 29;
            imageAnnotationType.WidthRequest = 29;
            textMarkupToolbar.IsVisible = false;
            bottomMainToolbar.IsVisible = true;
        }

        private void StrikethroughtButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;

            SetStriketroughtImageInToolbar();

            if (this.annotationType != AnnotationType.Strikethrough)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Strikethrough;
                this.annotationType = AnnotationType.Strikethrough;

                styleContent.ColorPicker.SelectedIndex = 5;
            }
            else
            {
                trashButton.IsVisible = true;
            }

            InitializeTextMarkupComponents();
        }

        private void SetStriketroughtImageInToolbar()
        {
            imageAnnotationType.Source = "ic_strikethrough.png";
            imageAnnotationType.HeightRequest = 30;
            imageAnnotationType.WidthRequest = 30;
        }

        private void InitializeTextMarkupComponents()
        {
            paletteButton.IsVisible = true;
            textMarkupToolbar.IsVisible = false;

            annotationTypeToolbar.IsVisible = true;
        }

        private void UnderlineButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;

            SetUnderlineImageInToolbar();

            if (this.annotationType != AnnotationType.Underline)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Underline;
                this.annotationType = AnnotationType.Underline;
                //todo -- set for red color
                styleContent.ColorPicker.SelectedIndex = 4;
            }
            else
            {
                trashButton.IsVisible = true;
            }

            InitializeTextMarkupComponents();
        }

        private void SetUnderlineImageInToolbar()
        {
            imageAnnotationType.Source = "ic_underline.png";
            imageAnnotationType.HeightRequest = 25;
            imageAnnotationType.WidthRequest = 25;
        }

        private void HightlightButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;

            SetHightlightImageInToolbar();

            if (this.annotationType != AnnotationType.Highlight)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Highlight;
                this.annotationType = AnnotationType.Highlight;
                styleContent.ColorPicker.SelectedIndex = 5;
            }
            else
            {
                trashButton.IsVisible = true;
            }

            InitializeTextMarkupComponents();
        }

        private void SetHightlightImageInToolbar()
        {
            imageAnnotationType.Source = "ic_edit.png";
            imageAnnotationType.HeightRequest = 25;
            imageAnnotationType.WidthRequest = 25;
        }
        #endregion

        #region AnnotationType toolbar methods
        private async void BackButtonAnnotationTypeToolbar_Clicked(object sender, EventArgs e)
        {
            pdfViewerControl.SelectionMode = Syncfusion.SfPdfViewer.XForms.SelectionMode.None;

            await ReturnToTheMainToolbarTheme();
        }

        private async Task ReturnToTheMainToolbarTheme()
        {
            await Task.Run(async () =>
            {
                await annotationTypeToolbar.LayoutTo(
                    new Rectangle(annotationTypeToolbar.Bounds.X, annotationTypeToolbar.Bounds.Y,
                        annotationTypeToolbar.Bounds.Width, 0), 200, Easing.Linear);
            });

            annotationTypeToolbar.IsVisible = false;

            if (InkOrFreeTextModeSelected())
            {
                bottomMainToolbar.IsVisible = true;

                SetDefaultTopBar();
            }
            else
            {
                if (ShapeAnnotationsSelected())
                {
                    shapeToolbar.IsVisible = true;
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                }
                else
                {
                    if (TextMarkupAnnotationsSelected())
                    {
                        textMarkupToolbar.IsVisible = true;
                    }
                    else
                    {
                        if (annotationType == AnnotationType.Stamp)
                        {
                            bottomMainToolbar.IsVisible = true;
                        }
                    }
                }
            }

            annotationType = AnnotationType.None;
        }

        private bool InkOrFreeTextModeSelected()
        {
            return annotationType == AnnotationType.Ink
                   || annotationType == AnnotationType.HandwrittenSignature
                   || annotationType == AnnotationType.FreeText;
        }

        private bool TextMarkupAnnotationsSelected()
        {
            return pdfViewerControl.AnnotationMode == AnnotationMode.Strikethrough
                   || pdfViewerControl.AnnotationMode == AnnotationMode.Highlight
                   || pdfViewerControl.AnnotationMode == AnnotationMode.Underline;
        }

        private bool ShapeAnnotationsSelected()
        {
            return pdfViewerControl.AnnotationMode == AnnotationMode.Arrow
                   || pdfViewerControl.AnnotationMode == AnnotationMode.Line
                   || pdfViewerControl.AnnotationMode == AnnotationMode.Circle
                   || pdfViewerControl.AnnotationMode == AnnotationMode.Rectangle;
        }

        private void SetDefaultTopBar()
        {
            viewModeButton.IsVisible = true;
            bookmarkButton.IsVisible = true;
            searchButton.IsVisible = true;
            moreOptionButton.IsVisible = true;

            ValidButton.IsVisible = false;
            UndoButton.IsVisible = false;
            RedoButton.IsVisible = false;
        }

        private void PaletteButton_Clicked()
        {
            stylePopup.ShowRelativeToView(paletteButton, RelativePosition.AlignBottomRight, 150, 0);
        }

        private void TrashButton_Clicked()
        {
            if (selectedFreeTextAnnotation != null)
            {
                pdfViewerControl.RemoveAnnotation(selectedFreeTextAnnotation);
                selectedFreeTextAnnotation = null;
            }
            else
            {
                if (selectedInkAnnotation != null)
                {
                    pdfViewerControl.RemoveAnnotation(selectedInkAnnotation);
                    selectedInkAnnotation = null;
                }
                else
                {
                    if (selectedShapeAnnotation != null)
                    {
                        pdfViewerControl.RemoveAnnotation(selectedShapeAnnotation);

                        selectedShapeAnnotation = null;
                    }
                    else
                    {
                        if (selectedTextMarkupAnnotation != null)
                        {
                            pdfViewerControl.RemoveAnnotation(selectedTextMarkupAnnotation);

                            selectedTextMarkupAnnotation = null;
                        }
                        else
                        {
                            if (selectedStampAnnotation != null)
                            {
                                pdfViewerControl.RemoveAnnotation(selectedStampAnnotation);

                                selectedStampAnnotation = null;
                            }
                            else
                            {
                                if (selectedHandwrittenSignature != null)
                                {
                                    pdfViewerControl.RemoveAnnotation(selectedHandwrittenSignature);

                                    selectedHandwrittenSignature = null;
                                }
                            }

                            BackButtonAnnotationTypeToolbar_Clicked(null, null);
                        }
                    }
                }
            }
        }
        #endregion

        #region Shape bar methods
        private async void BackButtonInShapeMenu_Clicked(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                await shapeToolbar.LayoutTo(new Rectangle(shapeToolbar.Bounds.X, shapeToolbar.Bounds.Y, shapeToolbar.Bounds.Width, 0), 200, Easing.Linear);
            });

            CollapseShapeMenu();

            bottomMainToolbar.IsVisible = true;
        }

        private void CollapseShapeMenu()
        {
            shapeToolbar.IsVisible = false;
            paletteButton.IsVisible = false;
        }

        private void CircleButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;

            SetCircleImageInToolbar();

            if (this.annotationType != AnnotationType.Circle)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Circle;
                this.annotationType = AnnotationType.Circle;
            }
            else
            {
                //TODO -- Remove This
                trashButton.IsVisible = true;
            }

            InitializeShapeBarComponents();
        }

        private void SetCircleImageInToolbar()
        {
            imageAnnotationType.HeightRequest = 32;
            imageAnnotationType.WidthRequest = 32;
            imageAnnotationType.Source = "ic_ui.png";
        }

        private void LineButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;

            SetLineImageInToolbar();

            if (this.annotationType != AnnotationType.Line)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Line;
                this.annotationType = AnnotationType.Line;
            }
            else
            {
                //TODO -- Remove This
                trashButton.IsVisible = true;
            }

            InitializeShapeBarComponents();
        }

        private void SetLineImageInToolbar()
        {
            imageAnnotationType.HeightRequest = 28;
            imageAnnotationType.WidthRequest = 28;
            imageAnnotationType.Source = "ic_square.png";
        }

        private void ArrowButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;

            SetArrowImageInToolbar();

            if (this.annotationType != AnnotationType.Arrow)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Arrow;
                this.annotationType = AnnotationType.Arrow;
            }
            else
            {
                trashButton.IsVisible = true;
            }

            InitializeShapeBarComponents();
        }

        private void SetArrowImageInToolbar()
        {
            imageAnnotationType.HeightRequest = 30;
            imageAnnotationType.WidthRequest = 30;
            imageAnnotationType.Source = "ic_directional.png";
        }

        private void RectangleButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;

            SetRectangleImageInToolbar();

            if (this.annotationType != AnnotationType.Rectangle)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Rectangle;
                this.annotationType = AnnotationType.Rectangle;
            }
            else
            {
                trashButton.IsVisible = true;
            }

            InitializeShapeBarComponents();
        }

        private void SetRectangleImageInToolbar()
        {
            imageAnnotationType.HeightRequest = 28;
            imageAnnotationType.WidthRequest = 28;
            imageAnnotationType.Source = "ic_math.png";
        }

        private void InitializeShapeBarComponents()
        {
            shapeToolbar.IsVisible = false;

            annotationTypeToolbar.IsVisible = true;
            paletteButton.IsVisible = true;

            styleContent.ColorPicker.SelectedIndex = 0;
            pdfViewerControl.AnnotationSettings.Rectangle.Settings.Thickness = 9;
        }
        #endregion

        #region Change view mode feature
        private void ViewModeButton_Clicked(object sender, EventArgs e)
        {
            if (pdfViewerControl.PageViewMode == PageViewMode.PageByPage)
            {
                pdfViewerControl.PageViewMode = PageViewMode.Continuous;
                viewModeButton.RotateTo(90);
            }
            else
            {
                pdfViewerControl.PageViewMode = PageViewMode.PageByPage;
                viewModeButton.RotateTo(180);
            }
        }
        #endregion

        #region Search text feature
        private void SearchButton_Clicked(object sender, EventArgs e)
        {
            topMainBar.IsVisible = false;
            viewModeButton.IsVisible = false;

            searchBar.IsVisible = true;

        }

        private void CancelSearchButton_Clicked(object sender, EventArgs e)
        {
            PdfViewerControl.CancelSearch();

            topMainBar.IsVisible = true;
            viewModeButton.IsVisible = true;

            searchBar.IsVisible = false;
        }

        private void PdfViewerControl_SearchCompleted(object sender, TextSearchCompletedEventArgs args)
        {
            UserDialogs.Instance.HideLoading();

            if (args.NoMatchFound)
                ShowErrorPopupForTextSearch(TextSearchErrorType.NoMatchFound);
            else if (args.NoMoreOccurrence)
                ShowErrorPopupForTextSearch(TextSearchErrorType.NoMoreOccurrence);

            searchTextInstanceStarted = false;
        }

        private void ShowErrorPopupForTextSearch(TextSearchErrorType textSearchErrorType)
        {
            switch (textSearchErrorType)
            {
                case TextSearchErrorType.NoMatchFound:
                    searchErrorPopupContent.NoOccurenceFound.IsVisible = true;
                    searchErrorPopupContent.NoMoreOccurenceFound.IsVisible = false;
                    break;
                case TextSearchErrorType.NoMoreOccurrence:
                    searchErrorPopupContent.NoOccurenceFound.IsVisible = false;
                    searchErrorPopupContent.NoMoreOccurenceFound.IsVisible = true;
                    break;
            }

            errorSearchPopup.Show();
        }

        private void TextSearchEntry_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textSearchEntry.Text) && !string.IsNullOrEmpty(textSearchEntry.Text))
            {
                pdfViewerControl.SearchText(textSearchEntry.Text);
            }

            if (string.IsNullOrWhiteSpace(textSearchEntry.Text) || string.IsNullOrEmpty(textSearchEntry.Text))
            {
                pdfViewerControl.CancelSearch();
                searchTextInstanceStarted = false;
            }

            if (!searchTextInstanceStarted)
            {

                pdfViewerControl.SearchText(textSearchEntry.Text);
            }

            else
            {
                pdfViewerControl.SearchNext();
            }

            searchTextInstanceStarted = true;
        }

        private void SearchPreviousButton_Clicked(object sender, EventArgs e)
        {
            if (textSearchEntry.Text != string.Empty)
            {
                pdfViewerControl.SearchPreviousTextCommand.Execute(textSearchEntry.Text);
            }
        }

        private void SearchNextButton_Clicked(object sender, EventArgs e)
        {
            if (textSearchEntry.Text != string.Empty)
            {
                pdfViewerControl.SearchNextTextCommand.Execute(textSearchEntry.Text);
            }
        }

        private void PdfViewerControl_TextMatchFound(object sender, TextMatchFoundEventArgs args)
        {
            searchPreviousButton.IsVisible = true;
            searchNextButton.IsVisible = true;

            UserDialogs.Instance.HideLoading();
        }

        private void TextSearchEntry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            pdfViewerControl.CancelSearch();
            searchTextInstanceStarted = false;

            searchPreviousButton.IsVisible = false;
            searchNextButton.IsVisible = false;
        }

        private void PdfViewerControl_SearchInitiated(object sender, TextSearchInitiatedEventArgs args)
        {
            UserDialogs.Instance.ShowLoading("Loading ...", MaskType.Clear);
        }
        #endregion

        #region On Property Changed
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}