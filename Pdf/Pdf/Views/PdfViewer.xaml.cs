using Android.Content;
using Pdf.controls;
using Pdf.Enumerations;
using Pdf.Interfaces;
using Pdf.Models;
using Pdf.ViewModels;
using Syncfusion.SfPdfViewer.XForms;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfViewer : ContentPage, INotifyPropertyChanged
    {
        #region Private Members
        private Stream pdfStream;

        private PdfViewerModel pdfViewerModel;
        private SfPopupLayout stylePopup;
        private SfPopupLayout errorSearchPopup;
        private SfPopupLayout passwordPopup;
        private SfPopupLayout popupMenu;

        private readonly StyleContent styleContent;
        private PopupMenuContent popupMenuContent;
        private SearchErrorPopup searchErrorPopupContent;

        private DataTemplate styleTemplateStylePopup;
        private DataTemplate styleTemplateErrorPopup;
        private DataTemplate templateViewSetPasswordPopup;

        ImageButton image;
        Label label;
        Entry entry;
        Button acceptButton;
        Button declineButton;

        private FreeTextAnnotation selectedFreeTextAnnotation;
        private InkAnnotation selectedInkAnnotation;
        private ShapeAnnotation selectedShapeAnnotation;
        private TextMarkupAnnotation selectedTextMarkupAnnotation;
        private StampAnnotation selectedStampAnnotation;

        private Color selectedColor = Color.Black;
        private Rectangle lastRectangleBounds;
        private AnnotationMode lastTextMarkupAnnotationMode;
        private AnnotationType annotationType;

        private string filePath;

        private bool canSaveDocument = false;
        private bool canUndoInk = false;
        private bool canRedoInk = false;
        private bool toolbarIsCollapsed = false;
        private bool search_started = false;
        private bool isATryToOpenTheDocument = false;
        private bool hasAlreadyTriedToOpenDoc = false;
        private bool isPasswordPopupInitalized = false;

        private bool isInkMenuEnabled = false;
        private bool isFreeTextMenuEnabled = false;
        private bool isShapeMenuEnabled = false;
        private bool isTextMarkupMenuEnabled = false;

        private int fontSize = 6;
        private int shapesNumbers = 0;
        private int textMarkupNumbers = 0;
        private int lastThicknessBarSelected = 5;
        private int lastOpacitySelected = 4;
        private int numberOfAnnotation = 0;
        #endregion

        #region Property
        public bool IsTextMarkupMenuEnabled
        {
            get
            {
                return isTextMarkupMenuEnabled;
            }
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
                        case AnnotationType.Hightlight:
                            HightlightButton_Clicked(null, null);
                            break;
                        case AnnotationType.Underline:
                            UnderlineButton_Clicked(null, null);
                            break;
                        case AnnotationType.Strikethrought:
                            StrikethroughtButton_Clicked(null, null);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public bool IsShapeMenuEnabled
        {
            get
            {
                return isShapeMenuEnabled;
            }
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
                        default:
                            break;
                    }
                }
            }
        }

        public bool IsInkMenuEnabled
        {
            get
            {
                return isInkMenuEnabled;
            }
            set
            {
                isInkMenuEnabled = value;
                OnPropertyChanged();

                if (isInkMenuEnabled == false)
                {
                    BackButtonAnnotationTypeToolbar_Clicked(null, null);
                }
                else
                {
                    SetToolbarForInkAnnotationSelected();
                }
            }
        }

        public bool IsFreeTextMenuEnabled
        {
            get
            {
                return isFreeTextMenuEnabled;
            }
            set
            {
                isFreeTextMenuEnabled = value;
                OnPropertyChanged();

                if (isFreeTextMenuEnabled == false)
                {
                    BackButtonAnnotationTypeToolbar_Clicked(null, null);
                }
                else
                {
                    SetToolbarForFreeTextAnnotationSelected();
                }
            }
        }

        public int NumberOfAnnotation
        {
            get
            {
                return numberOfAnnotation;
            }
            set
            {
                numberOfAnnotation = value;
                OnPropertyChanged();

                if (numberOfAnnotation != 0)
                    CanSaveDocument = true;
                else
                {
                    CanSaveDocument = false;
                }
            }
        }

        public bool CanSaveDocument
        {
            get
            {
                return canSaveDocument;
            }
            set
            {
                canSaveDocument = value;
                OnPropertyChanged();

                ItemsMenu item = (ItemsMenu)popupMenuContent.ItemsMenu[0];

                if (canSaveDocument == true)
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
                    UndoButton.Foreground = Color.FromHex("#373737");
                    ValidButton.Foreground = Color.FromHex("#373737");
                }
                else
                {
                    if (value == false)
                    {
                        UndoButton.Foreground = Color.Transparent;
                        ValidButton.Foreground = Color.Transparent;
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
                    RedoButton.Foreground = Color.FromHex("#373737");
                else
                {
                    if (value == false)
                        RedoButton.Foreground = Color.Transparent;
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
                    ValidButton.Foreground = Color.FromHex("#373737");
                else
                    ValidButton.Foreground = Color.FromHex("#373737");
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
                    ValidButton.Foreground = Color.FromHex("#373737");
                else
                    ValidButton.Foreground = Color.FromHex("#373737");
            }
        }
        #endregion

        #region On Disappearing
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
            pdfViewerControl.SearchCompleted -= PdfViewerControl_SearchCompleted;
            pdfViewerControl.TextMatchFound -= PdfViewerControl_TextMatchFound;
            pdfViewerControl.SearchInitiated -= PdfViewerControl_SearchInitiated;
            pdfViewerControl.StampAnnotationAdded -= PdfViewerControl_StampAnnotationAdded;
            pdfViewerControl.StampAnnotationSelected -= PdfViewerControl_StampAnnotationSelected;
            pdfViewerControl.StampAnnotationDeselected -= PdfViewerControl_StampAnnotationDeselected;

            pdfViewerControl.DoubleTapped -= PdfViewerControl_DoubleTapped;

            styleContent.ThicknessBar.BoxViewButtonClicked -= ThicknessBar_Clicked;
            styleContent.OpacityButtonClicked -= OpacityIcon_Clicked;

            if (passwordPopup != null)
            {
                passwordPopup.Closed -= PasswordPopup_Closed;
                acceptButton.Clicked -= AcceptButtonPasswordPopup_Clicked;
                declineButton.Clicked -= DeclineButton_Clicked;

                passwordPopup.IsOpen = false;
            }

            pdfViewerControl.Unload();
            base.OnDisappearing();
        }
        #endregion

        #region On Appearing 
        protected override void OnAppearing()
        {
            pdfViewerControl.PasswordErrorOccurred += PdfViewerControl_PasswordErrorOccurred;

            pdfViewerControl.DocumentLoaded += PdfViewerControl_DocumentLoaded;

            pdfStream = DependencyService.Get<IAndroidFileHelper>().GetFileStream(filePath);

            // PDFium renderer
            pdfViewerControl.CustomPdfRenderer = DependencyService.Get<ICustomPdfRendererService>().AlternatePdfRenderer;

            pdfViewerControl.LoadDocument(pdfStream);

            MessagingCenter.Subscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor", (sender, helper) =>
            {
                this.SelectedColor = helper;
            });

            base.OnAppearing();
        }
        #endregion

        #region Constructor
        public PdfViewer(string filePath)
        {
            InitializeComponent();

            this.filePath = filePath;
            this.styleContent = new StyleContent();

            //Disable the display the default toolbar
            pdfViewerControl.Toolbar.Enabled = false;

            //Disable the display of password UI view
            pdfViewerControl.IsPasswordViewEnabled = false;
        }
        #endregion

        #region On Document Loaded Event Handler
        private void PdfViewerControl_DocumentLoaded(object sender, EventArgs args)
        {
            if (isPasswordPopupInitalized != true)
            {
                Shell.SetNavBarIsVisible(this, false);
                Shell.SetTabBarIsVisible(this, false);
                NavigationPage.SetHasNavigationBar(this, false);

                Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            }

            #region Pdf viewer events
            pdfViewerControl.FreeTextAnnotationAdded += PdfViewerControl_FreeTextAnnotationAdded;
            pdfViewerControl.FreeTextAnnotationSelected += PdfViewerControl_FreeTextAnnotationSelected;
            pdfViewerControl.FreeTextAnnotationDeselected += PdfViewerControl_FreeTextAnnotationDeselected;

            pdfViewerControl.CanRedoInkModified += PdfViewerControl_CanRedoInkModified;
            pdfViewerControl.CanUndoInkModified += PdfViewerControl_CanUndoInkModified;

            pdfViewerControl.InkSelected += PdfViewerControl_InkSelected;
            pdfViewerControl.InkDeselected += PdfViewerControl_InkDeselected;
            pdfViewerControl.InkAdded += PdfViewerControl_InkAdded;

            pdfViewerControl.ShapeAnnotationAdded += PdfViewerControl_ShapeAnnotationAdded;
            pdfViewerControl.ShapeAnnotationSelected += PdfViewerControl_ShapeAnnotationSelected;
            pdfViewerControl.ShapeAnnotationDeselected += PdfViewerControl_ShapeAnnotationDeselected;

            pdfViewerControl.TextMarkupSelected += PdfViewerControl_TextMarkupSelected;
            pdfViewerControl.TextMarkupDeselected += PdfViewerControl_TextMarkupDeselected;
            pdfViewerControl.TextMarkupAdded += PdfViewerControl_TextMarkupAdded;

            pdfViewerControl.PageChanged += PdfViewerControl_PageChanged;
            pdfViewerControl.SearchCompleted += PdfViewerControl_SearchCompleted;
            pdfViewerControl.TextMatchFound += PdfViewerControl_TextMatchFound;
            pdfViewerControl.SearchInitiated += PdfViewerControl_SearchInitiated;
            pdfViewerControl.DoubleTapped += PdfViewerControl_DoubleTapped;
            pdfViewerControl.StampAnnotationSelected += PdfViewerControl_StampAnnotationSelected;
            pdfViewerControl.StampAnnotationDeselected += PdfViewerControl_StampAnnotationDeselected;

            pdfViewerControl.StampAnnotationAdded += PdfViewerControl_StampAnnotationAdded;
            #endregion

            RedoButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if (CanRedoInk == true)
                    {
                        RedoInk();
                        await Task.Delay(100);
                        if (CanRedoInk == true)
                            RedoButton.Foreground = Color.FromHex("#373737");
                        else
                        {
                            RedoButton.Foreground = Color.Transparent;
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
                        UndoInk();
                        await Task.Delay(100);

                        if (CanUndoInk == true)
                            UndoButton.Foreground = Color.FromHex("#373737");
                        else
                        {
                            UndoButton.Foreground = Color.Transparent;
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

                    if (pdfViewerControl.AnnotationMode == AnnotationMode.Arrow
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Line
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Circle
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Rectangle)
                    {
                        pdfViewerControl.AnnotationMode = AnnotationMode.None;

                        ValidButton.Foreground = Color.Transparent;
                    }
                    else
                    {
                        if (pdfViewerControl.AnnotationMode == AnnotationMode.Strikethrough
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Highlight
                        || pdfViewerControl.AnnotationMode == AnnotationMode.Underline)
                        {
                            pdfViewerControl.AnnotationMode = AnnotationMode.None;

                            ValidButton.Foreground = Color.Transparent;
                        }
                    }
                })
            });

            paletteButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    PaletteButton_Clicked();
                })
            });

            trashButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    TrashButton_Clicked();
                })
            });

            pdfViewerControl.PreserveSignaturePadOrientation = true;

            paletteButton.BindingContext = this;
            trashButton.BindingContext = this;
            styleContent.BindingContext = this;

            #region Style popup 
            stylePopup = new SfPopupLayout();

            styleContent.ThicknessBar.BindingContext = this;
            stylePopup.ClosePopupOnBackButtonPressed = false;
            stylePopup.PopupView.ShowHeader = false;
            stylePopup.PopupView.ShowFooter = false;
            stylePopup.PopupView.HeightRequest = 192;
            stylePopup.PopupView.WidthRequest = 280;
            stylePopup.PopupView.PopupStyle.BorderColor = Color.FromHex("#fafafa");
            stylePopup.PopupView.AnimationMode = AnimationMode.SlideOnBottom;

            styleTemplateStylePopup = new DataTemplate(() =>
            {
                return styleContent;
            });

            this.stylePopup.PopupView.ContentTemplate = styleTemplateStylePopup;

            styleContent.ThicknessBar.BoxViewButtonClicked += (int numberOfThicknessBarClicked) => ThicknessBar_Clicked(numberOfThicknessBarClicked);
            styleContent.OpacityButtonClicked += (int numberOfTheOpacityClicked) => OpacityIcon_Clicked(numberOfTheOpacityClicked);
            #endregion

            #region SearchErrorPopup 
            searchErrorPopupContent = new SearchErrorPopup();

            errorSearchPopup = new SfPopupLayout();
            errorSearchPopup.PopupView.ShowHeader = false;
            errorSearchPopup.PopupView.ShowFooter = false;
            errorSearchPopup.PopupView.HeightRequest = 105;
            errorSearchPopup.PopupView.WidthRequest = 300;
            errorSearchPopup.BackgroundColor = Color.FromHex("#fafafa");
            errorSearchPopup.PopupView.BackgroundColor = Color.FromHex("#fafafa");
            errorSearchPopup.PopupView.PopupStyle.BorderColor = Color.FromHex("#fafafa");
            errorSearchPopup.PopupView.AnimationMode = AnimationMode.Fade;

            styleTemplateErrorPopup = new DataTemplate(() =>
            {
                return searchErrorPopupContent;
            });

            this.errorSearchPopup.PopupView.ContentTemplate = styleTemplateErrorPopup;

            if (passwordPopup != null)
                passwordPopup.IsOpen = false;
            #endregion

            #region Popup Menu
            popupMenu = new SfPopupLayout();
            popupMenu.PopupView.IsFullScreen = true;
            popupMenu.PopupView.AnimationDuration = 200;
            popupMenu.PopupView.AnimationMode = AnimationMode.SlideOnBottom;
            popupMenu.PopupView.PopupStyle.CornerRadius = 0;
            popupMenu.PopupView.PopupStyle.BorderThickness = 2;
            popupMenu.PopupView.PopupStyle.BorderColor = Color.White;
            popupMenu.PopupView.HorizontalOptions = LayoutOptions.FillAndExpand;
            popupMenu.PopupView.VerticalOptions = LayoutOptions.FillAndExpand;
            popupMenu.Padding = new Thickness(15, 15, 15, 15);
            popupMenu.PopupView.ShowFooter = false;
            popupMenu.PopupView.ShowCloseButton = false;
            popupMenu.PopupView.PopupStyle.HeaderBackgroundColor = Color.FromHex("#eeeeee");
            popupMenu.PopupView.PopupStyle.BorderColor = Color.FromHex("#e0e0e0");

            popupMenuContent = new PopupMenuContent();

            DataTemplate popupMenuContentTemplate = new DataTemplate(() =>
            {
                return popupMenuContent;
            });

            DataTemplate headerTemplateViewPopupMenu = new DataTemplate(() =>
            {
                StackLayout stackLayout = new StackLayout();
                stackLayout.Orientation = StackOrientation.Horizontal;
                stackLayout.Padding = new Thickness(0, 0, 13, 0);

                Label title = new Label();
                title.Text = "More";
                title.FontSize = 18;
                title.FontFamily = "GothamBold.ttf#GothamBold";
                title.VerticalTextAlignment = Xamarin.Forms.TextAlignment.Center;
                title.HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Center;
                title.TextColor = Color.FromHex("#4e4e4e");
                title.HorizontalOptions = LayoutOptions.FillAndExpand;

                image = new ImageButton();
                image.Source = "outlineClearViewer.xml";
                image.Aspect = Aspect.AspectFit;
                image.BackgroundColor = Color.Transparent;
                image.HorizontalOptions = LayoutOptions.End;

                stackLayout.Children.Add(title);
                stackLayout.Children.Add(image);

                return stackLayout;
            });

            popupMenu.PopupView.HeaderTemplate = headerTemplateViewPopupMenu;

            popupMenu.PopupView.ContentTemplate = popupMenuContentTemplate;
            #endregion

            annotationType = AnnotationType.None;

            UserDialogs.Instance.HideLoading();
        }
        #endregion

        #region Top Toolbar Event Handlers 
        private void BookmarkButton_Clicked(object sender, EventArgs e)
        {
            //Bookmark pane will be collapsed, if it is expanded
            pdfViewerControl.BookmarkPaneVisible = true;
        }

        #region PopupMenu Methods

        private void CloseButtonPopupMenu_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => {
                popupMenu.IsOpen = false;
            });
        }

        //TODO HANDLE SAVE FOR PRINT

        private async void MenuListView_SelectionChanged(object sender, Syncfusion.ListView.XForms.ItemSelectionChangedEventArgs e)
        {
            var itemMenu = (ItemsMenu)popupMenuContent.MenuListView.SelectedItem;
            popupMenuContent.MenuListView.SelectedItem = null;

            switch (itemMenu.Id)
            {
                case 0:
                    if (CanSaveDocument == true)
                        await SaveDocument();
                    break;
                case 1:
                    await PrintDocument();
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }

        //TODO CHECK IF OTHER ANNOTATION CAN BEN PRINTED FOR SAVE OR NOT THE PDF WHEN PRTINING
        private async Task PrintDocument()
        {
            Device.BeginInvokeOnMainThread(() => {
                popupMenu.IsOpen = false;
            });

            using (UserDialogs.Instance.Loading("Loading", null, null, true, MaskType.Black))
            {
                await Task.Run(() =>
                {
                    var fileName = Path.GetFileName(this.filePath);
                    pdfViewerControl.Print(fileName);
                });
            }
        }

        private async Task SaveDocument()
        {
            Device.BeginInvokeOnMainThread(() => {
                popupMenu.IsOpen = false;
            });

            using (UserDialogs.Instance.Loading("Loading", null, null, true, MaskType.Black))
            {
                NumberOfAnnotation = 0;

                Dictionary<bool, string> saveStatus = null;

                await Task.Run(async () =>
                {
                    using (Stream stream = await pdfViewerControl.SaveDocumentAsync())
                    {
                        saveStatus = await DependencyService.Get<IAndroidFileHelper>().Save(stream as MemoryStream, this.filePath);
                    }
                });

                if (saveStatus.ContainsKey(true) == true)
                {
                    DependencyService.Get<IToastMessage>().LongAlert("Document saved");
                }
                else
                {
                    DependencyService.Get<IToastMessage>().LongAlert(saveStatus[false]);
                }
            }
        }

        private void MoreOptionButton_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => {
                popupMenu.Show();
            });

            popupMenuContent.MenuListView.SelectionChanged += MenuListView_SelectionChanged;
            popupMenu.Closed += PopupMenu_Closed;
            image.Clicked += CloseButtonPopupMenu_Clicked;
        }

        private void PopupMenu_Closed(object sender, EventArgs e)
        {
            popupMenuContent.MenuListView.SelectionChanged -= MenuListView_SelectionChanged;
            popupMenu.Closed -= PopupMenu_Closed;
            image.Clicked -= CloseButtonPopupMenu_Clicked;
        }
        #endregion
        #endregion

        #region Pdf viewer events methods

        #region PDF Password Event Handlers
        private void AcceptButtonPasswordPopup_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);

            passwordPopup.IsOpen = false;
            DependencyService.Get<IKeyboardHelper>().HideKeyboard();

            isATryToOpenTheDocument = true;
        }

        private void PasswordPopup_Closed(object sender, EventArgs e)
        {
            if (isATryToOpenTheDocument == true)
            {
                pdfViewerControl.LoadDocument(pdfStream, entry.Text);
                isATryToOpenTheDocument = false;
            }
        }

        private async void DeclineButton_Clicked(object sender, EventArgs e)
        {
            passwordPopup.IsOpen = false;
            DependencyService.Get<IKeyboardHelper>().HideKeyboard();

            await Navigation.PopAsync();
        }

        private void PdfViewerControl_PasswordErrorOccurred(object sender, PasswordErrorOccurredEventArgs e)
        {
            if (isPasswordPopupInitalized == false)
            {
                passwordPopup = new SfPopupLayout();
                passwordPopup.PopupView.HeightRequest = 200;
                passwordPopup.PopupView.WidthRequest = 310;
                passwordPopup.PopupView.ShowHeader = true;
                passwordPopup.PopupView.ShowFooter = true;
                passwordPopup.PopupView.ShowCloseButton = false;
                passwordPopup.PopupView.AnimationDuration = 170;
                passwordPopup.PopupView.HeaderHeight = 63;
                passwordPopup.Closed += PasswordPopup_Closed;
                passwordPopup.StaysOpen = true;

                DataTemplate templateViewHeaderSetPasswordPopop = new DataTemplate(() =>
                {
                    Label label = new Label();
                    label.VerticalTextAlignment = Xamarin.Forms.TextAlignment.Center;
                    label.Padding = new Thickness(20, 0, 20, 0);
                    label.Text = "Password";
                    label.TextColor = Color.Black;
                    label.FontSize = 17;
                    label.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";

                    return label;
                });


                templateViewSetPasswordPopup = new DataTemplate(() =>
                {
                    StackLayout stackLayout = new StackLayout();

                    label = new Label();

                    label.Text = "This file is password protected. Please enter the password";
                    label.TextColor = Color.Black;
                    label.FontSize = 13.5;
                    label.Padding = new Thickness(20, 0, 20, 0);
                    label.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";

                    entry = new Entry();
                    entry.FontSize = 13.5;
                    entry.Margin = new Thickness(19, 0, 19, 0);
                    entry.IsPassword = true;
                    entry.Placeholder = "Enter the password";
                    entry.TextColor = Color.Black;

                    stackLayout.Children.Add(label);
                    stackLayout.Children.Add(entry);

                    return stackLayout;
                });

                DataTemplate footerTemplateViewSetPasswordPopup = new DataTemplate(() =>
                {
                    StackLayout stackLayout = new StackLayout();
                    stackLayout.Orientation = StackOrientation.Horizontal;
                    stackLayout.Spacing = 0;

                    acceptButton = new Button();
                    acceptButton.Text = "Ok";
                    acceptButton.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                    acceptButton.FontSize = 14;
                    acceptButton.TextColor = Color.Black;
                    acceptButton.HorizontalOptions = LayoutOptions.EndAndExpand;
                    acceptButton.BackgroundColor = Color.White;
                    acceptButton.VerticalOptions = LayoutOptions.Center;
                    acceptButton.WidthRequest = 86;
                    acceptButton.Clicked += AcceptButtonPasswordPopup_Clicked;
                    declineButton = new Button();
                    declineButton.Text = "Cancel";
                    declineButton.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                    declineButton.FontSize = 14;
                    declineButton.TextColor = Color.Black;
                    declineButton.HorizontalOptions = LayoutOptions.End;
                    declineButton.BackgroundColor = Color.White;
                    declineButton.VerticalOptions = LayoutOptions.Center;
                    declineButton.WidthRequest = 89;
                    declineButton.Clicked += DeclineButton_Clicked;

                    stackLayout.Children.Add(acceptButton);
                    stackLayout.Children.Add(declineButton);

                    return stackLayout;
                });

                // Adding ContentTemplate of the SfPopupLayout
                passwordPopup.PopupView.ContentTemplate = templateViewSetPasswordPopup;

                // Adding FooterTemplate of the SfPopupLayout
                passwordPopup.PopupView.FooterTemplate = footerTemplateViewSetPasswordPopup;

                // Adding FooterTemplate of the SfPopupLayout
                passwordPopup.PopupView.HeaderTemplate = templateViewHeaderSetPasswordPopop;

                isPasswordPopupInitalized = true;

                Shell.SetNavBarIsVisible(this, false);
                Shell.SetTabBarIsVisible(this, false);
                NavigationPage.SetHasNavigationBar(this, false);
                Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            }

            if (hasAlreadyTriedToOpenDoc == true)
            {
                passwordPopup.Show();
                label.Text = "The password is incorrect. Please try again";
                entry.Focus();
            }
            else
            {
                passwordPopup.Show();
                hasAlreadyTriedToOpenDoc = true;
            }
        }
        #endregion

        private async void PdfViewerControl_DoubleTapped(object sender, TouchInteractionEventArgs e)
        {
            if (this.toolbarIsCollapsed == true)
            {
                DependencyService.Get<INavBarHelper>().SetDefaultNavBar();

                await topToolbar.FadeTo(1, 150);
                await bottomMainBar.FadeTo(1, 150);

                await Task.Delay(150);

                pdfViewGrid.Margin = new Thickness(0, 45, 0, 45);

                toolbarIsCollapsed = false;
            }
            else
            {
                DependencyService.Get<INavBarHelper>().SetImmersiveMode();

                pdfViewGrid.Margin = 0;

                await topToolbar.FadeTo(0, 150);
                await bottomMainBar.FadeTo(0, 150);

                toolbarIsCollapsed = true;
            }
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

        #region Annotation Selected Event Handlers

        private void PdfViewerControl_StampAnnotationDeselected(object sender, StampAnnotationDeselectedEventArgs e)
        {
            BackButtonAnnotationTypeToolbar_Clicked(null, null);
        }

        private void PdfViewerControl_StampAnnotationSelected(object sender, StampAnnotationSelectedEventArgs e)
        {
            SetToolbarForStampAnnotationSelected();

            selectedStampAnnotation = sender as StampAnnotation;

            this.annotationType = AnnotationType.Stamp;
        }

        private void PdfViewerControl_TextMarkupDeselected(object sender, TextMarkupDeselectedEventArgs args)
        {
            IsTextMarkupMenuEnabled = false;

            selectedShapeAnnotation = null;
        }

        private void PdfViewerControl_TextMarkupSelected(object sender, TextMarkupSelectedEventArgs args)
        {
            //Cast the sender object to FreeTextAnnotation
            selectedTextMarkupAnnotation = sender as TextMarkupAnnotation;

            SelectedColor = this.selectedTextMarkupAnnotation.Settings.Color;

            switch (args.TextMarkupAnnotationType)
            {
                case TextMarkupAnnotationType.Strikethrough:
                    this.annotationType = AnnotationType.Strikethrought;
                    break;
                case TextMarkupAnnotationType.Underline:
                    this.annotationType = AnnotationType.Underline;
                    break;
                case TextMarkupAnnotationType.Highlight:
                    this.annotationType = AnnotationType.Hightlight;
                    break;
                default:
                    break;
            }

            IsTextMarkupMenuEnabled = true;
        }

        private void PdfViewerControl_ShapeAnnotationDeselected(object sender, ShapeAnnotationDeselectedEventArgs args)
        {
            IsShapeMenuEnabled = false;

            selectedShapeAnnotation = null;
        }

        //TODO -- FOUND A WAY TO DESELECT OR HANDLE THE BOTTOM BAR BUG
        private void PdfViewerControl_ShapeAnnotationSelected(object sender, ShapeAnnotationSelectedEventArgs args)
        {
            //Cast the sender object to FreeTextAnnotation
            this.selectedShapeAnnotation = sender as ShapeAnnotation;

            SelectedColor = this.selectedShapeAnnotation.Settings.StrokeColor;

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
                default:
                    break;
            }

            IsShapeMenuEnabled = true;
        }

        private void PdfViewerControl_FreeTextAnnotationDeselected(object sender, FreeTextAnnotationDeselectedEventArgs args)
        {
            IsFreeTextMenuEnabled = false;

            selectedFreeTextAnnotation = null;
        }

        private void PdfViewerControl_FreeTextAnnotationSelected(object sender, FreeTextAnnotationSelectedEventArgs args)
        {
            this.annotationType = AnnotationType.FreeText;

            //Cast the sender object to FreeTextAnnotation
            this.selectedFreeTextAnnotation = sender as FreeTextAnnotation;

            SelectedColor = args.TextColor;

            IsFreeTextMenuEnabled = true;
        }

        private void PdfViewerControl_InkSelected(object sender, InkSelectedEventArgs args)
        {
            this.annotationType = AnnotationType.Ink;
            selectedInkAnnotation = sender as InkAnnotation;

            SelectedColor = args.Color;

            IsInkMenuEnabled = true;
        }

        private void PdfViewerControl_InkDeselected(object sender, InkDeselectedEventArgs args)
        {
            IsInkMenuEnabled = false;

            selectedInkAnnotation = null;
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

            this.ShapesNumbers += 1;
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

            this.TextMarkupNumbers += 1;
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
            if (CanUndoInk == true)
                pdfViewerControl.UndoInk();
        }

        private void RedoInk()
        {
            if (CanRedoInk == true)
                pdfViewerControl.RedoInk();
        }

        private async void SaveInk()
        {
            pdfViewerControl.EndInkSession(true);

            await Task.Run(async () =>
            {
                await annotationTypeToolbar.LayoutTo(new Rectangle(annotationTypeToolbar.Bounds.X, annotationTypeToolbar.Bounds.Y, annotationTypeToolbar.Bounds.Width, 0), 200, Easing.Linear);
            });

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

        #endregion

        #region ThicknessBarEvents
        private void ThicknessBar_Clicked(int numberOfThicknessBarClicked)
        {
            if (lastThicknessBarSelected != numberOfThicknessBarClicked)
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
        private async void SignatureButton_Clicked(object sender, EventArgs e)
        {
            pdfViewerControl.AnnotationMode = AnnotationMode.HandwrittenSignature;
        }

        private void StampButton_Clicked(object sender, EventArgs e)
        {
            //Set image source
            Image image = new Image();
            image.Source = ImageSource.FromResource("Pdf.Images.stamp.png", typeof(App).GetTypeInfo().Assembly);
            image.WidthRequest = 200;
            image.HeightRequest = 100;

            //Add image as custom stamp to the first page
            pdfViewerControl.AddStamp(image, pdfViewerControl.PageNumber);
        }

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

        #region Enable Annotation toolbar Methods 
        private void SetToolbarForShapeAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;
            textMarkupToolbar.IsVisible = false;

            styleContent.FontSizeControl.IsVisible = false;

            stylePopup.PopupView.HeightRequest = 195;
            stylePopup.PopupView.WidthRequest = 280;

            shapeToolbar.IsVisible = true;
            styleContent.BoxView2.IsVisible = true;
            styleContent.BoxView1.IsVisible = true;
            styleContent.OpacityControl.IsVisible = true;
            styleContent.ThicknessBar.IsVisible = true;
        }

        private void SetToolbarForFreeTextAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;
            shapeToolbar.IsVisible = false;
            textMarkupToolbar.IsVisible = false;

            styleContent.OpacityControl.IsVisible = false;
            styleContent.BoxView2.IsVisible = false;
            styleContent.ThicknessBar.IsVisible = false;

            stylePopup.PopupView.HeightRequest = 130;
            stylePopup.PopupView.WidthRequest = 280;

            styleContent.FontSizeControl.IsVisible = true;
            styleContent.BoxView1.IsVisible = true;
            paletteButton.IsVisible = true;
            annotationTypeToolbar.IsVisible = true;

            imageAnnotationType.Source = "twotone_title_24.xml";

            if (this.annotationType != AnnotationType.FreeText)
            {
                this.annotationType = AnnotationType.FreeText;
                pdfViewerControl.AnnotationMode = AnnotationMode.FreeText;
                pdfViewerControl.AnnotationSettings.FreeText.TextSize = 8;
                this.FontSize = 8;
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
            styleContent.BoxView2.IsVisible = true;
            styleContent.BoxView1.IsVisible = true;

            ValidButton.IsVisible = true;
            UndoButton.IsVisible = true;
            RedoButton.IsVisible = true;

            stylePopup.PopupView.HeightRequest = 195;
            stylePopup.PopupView.WidthRequest = 280;

            if (this.annotationType != AnnotationType.Ink)
            {
                this.annotationType = AnnotationType.Ink;
                pdfViewerControl.AnnotationMode = AnnotationMode.Ink;

                pdfViewerControl.AnnotationSettings.Ink.Thickness = 9;
                styleContent.ColorPicker.SelectedIndex = 0;
            }
            else
            {
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
            styleContent.BoxView2.IsVisible = false;
            styleContent.BoxView1.IsVisible = false;

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
        #endregion

        private async Task CollapseBottomMainToolbar(AnnotationType annotationType)
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
                await textMarkupToolbar.LayoutTo(new Rectangle(textMarkupToolbar.Bounds.X, textMarkupToolbar.Bounds.Y, textMarkupToolbar.Bounds.Width, 0), 200, Easing.Linear);
            });

            imageAnnotationType.HeightRequest = 29;
            imageAnnotationType.WidthRequest = 29;
            textMarkupToolbar.IsVisible = false;
            bottomMainToolbar.IsVisible = true;
        }

        private async void StrikethroughtButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;

            imageAnnotationType.Source = "ic_strikethrough.png";

            imageAnnotationType.HeightRequest = 30;
            imageAnnotationType.WidthRequest = 30;

            if (this.annotationType != AnnotationType.Strikethrought)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Strikethrough;
                this.annotationType = AnnotationType.Strikethrought;

                styleContent.ColorPicker.SelectedIndex = 5;
            }
            else
            {
                trashButton.IsVisible = true;
            }

            InitializeComponentForTextMarkupAnnotation();
        }

        private async void UnderlineButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;

            imageAnnotationType.Source = "ic_underline.png";
            imageAnnotationType.HeightRequest = 25;
            imageAnnotationType.WidthRequest = 25;

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

            InitializeComponentForTextMarkupAnnotation();
        }

        private async void HightlightButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;

            imageAnnotationType.Source = "ic_edit.png";
            imageAnnotationType.HeightRequest = 25;
            imageAnnotationType.WidthRequest = 25;

            if (this.annotationType != AnnotationType.Hightlight)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Highlight;
                this.annotationType = AnnotationType.Hightlight;
                styleContent.ColorPicker.SelectedIndex = 5;
            }
            else
            {
                trashButton.IsVisible = true;
            }

            InitializeComponentForTextMarkupAnnotation();
        }

        private void InitializeComponentForTextMarkupAnnotation()
        {
            paletteButton.IsVisible = true;
            textMarkupToolbar.IsVisible = false;

            annotationTypeToolbar.IsVisible = true;
        }
        #endregion

        #region AnnotationType toolbar methods
        private async void BackButtonAnnotationTypeToolbar_Clicked(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                await annotationTypeToolbar.LayoutTo(new Rectangle(annotationTypeToolbar.Bounds.X, annotationTypeToolbar.Bounds.Y, annotationTypeToolbar.Bounds.Width, 0), 200, Easing.Linear);
            });

            annotationTypeToolbar.IsVisible = false;

            switch (this.annotationType)
            {
                case AnnotationType.Ink:
                    pdfViewerControl.SelectionMode = Syncfusion.SfPdfViewer.XForms.SelectionMode.None;
                    annotationType = AnnotationType.None;
                    bottomMainToolbar.IsVisible = true;

                    viewModeButton.IsVisible = true;
                    bookmarkButton.IsVisible = true;
                    searchButton.IsVisible = true;
                    moreOptionButton.IsVisible = true;

                    ValidButton.IsVisible = false;
                    UndoButton.IsVisible = false;
                    RedoButton.IsVisible = false;
                    break;
                case AnnotationType.FreeText:
                    pdfViewerControl.SelectionMode = Syncfusion.SfPdfViewer.XForms.SelectionMode.None;
                    annotationType = AnnotationType.None;
                    bottomMainToolbar.IsVisible = true;
                    break;
                case (AnnotationType.Arrow):
                    shapeToolbar.IsVisible = true;
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    pdfViewerControl.SelectionMode = Syncfusion.SfPdfViewer.XForms.SelectionMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Circle):
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    shapeToolbar.IsVisible = true;
                    pdfViewerControl.SelectionMode = Syncfusion.SfPdfViewer.XForms.SelectionMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Line):
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    shapeToolbar.IsVisible = true;
                    pdfViewerControl.SelectionMode = Syncfusion.SfPdfViewer.XForms.SelectionMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Rectangle):
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    shapeToolbar.IsVisible = true;
                    pdfViewerControl.SelectionMode = Syncfusion.SfPdfViewer.XForms.SelectionMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Hightlight):
                    textMarkupToolbar.IsVisible = true;
                    pdfViewerControl.SelectionMode = Syncfusion.SfPdfViewer.XForms.SelectionMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Strikethrought):
                    textMarkupToolbar.IsVisible = true;
                    pdfViewerControl.SelectionMode = Syncfusion.SfPdfViewer.XForms.SelectionMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Underline):
                    textMarkupToolbar.IsVisible = true;
                    pdfViewerControl.SelectionMode = Syncfusion.SfPdfViewer.XForms.SelectionMode.None;
                    annotationType = AnnotationType.None;
                    break;
                case (AnnotationType.Stamp):
                    pdfViewerControl.SelectionMode = Syncfusion.SfPdfViewer.XForms.SelectionMode.None;
                    annotationType = AnnotationType.None;
                    bottomMainToolbar.IsVisible = true;
                    break;
                case AnnotationType.None:
                    break;
                default:
                    break;
            }
        }

        private async void PaletteButton_Clicked()
        {
            stylePopup.ShowRelativeToView(paletteButton, RelativePosition.AlignTopRight, 150, 39.5);
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
                        if(selectedTextMarkupAnnotation != null)
                        {
                            pdfViewerControl.RemoveAnnotation(selectedTextMarkupAnnotation);

                            selectedTextMarkupAnnotation = null;
                        }
                        else
                        {
                            if(selectedStampAnnotation != null)
                            {
                                pdfViewerControl.RemoveAnnotation(selectedStampAnnotation);

                                selectedStampAnnotation = null;

                                BackButtonAnnotationTypeToolbar_Clicked(null, null);
                            }
                        }
                    }
                }
            }
        }
                
        #endregion

        #region Shape bar methods
        private async void BackButtonShapeToolbar_Clicked(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                await shapeToolbar.LayoutTo(new Rectangle(shapeToolbar.Bounds.X, shapeToolbar.Bounds.Y, shapeToolbar.Bounds.Width, 0), 200, Easing.Linear);
            });

            shapeToolbar.IsVisible = false;
            paletteButton.IsVisible = false;
            bottomMainToolbar.IsVisible = true;
        }

        private void CircleButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;
            imageAnnotationType.HeightRequest = 32;
            imageAnnotationType.WidthRequest = 32;
            imageAnnotationType.Source = "ic_ui.png";

            if (this.annotationType != AnnotationType.Circle)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Circle;
                this.annotationType = AnnotationType.Circle;
            }
            else
            {
                trashButton.IsVisible = true;
            }

            InitalizeCompenentsForShapeBar();
        }

        private void LineButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;
            imageAnnotationType.HeightRequest = 28;
            imageAnnotationType.WidthRequest = 28;

            imageAnnotationType.Source = "ic_square.png";

            if (this.annotationType != AnnotationType.Line)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Line;
                this.annotationType = AnnotationType.Line;
            }
            else
            {
                trashButton.IsVisible = true;
            }

            InitalizeCompenentsForShapeBar();
        }

        private void ArrowButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;
            imageAnnotationType.HeightRequest = 30;
            imageAnnotationType.WidthRequest = 30;

            imageAnnotationType.Source = "ic_directional.png";

            if (this.annotationType != AnnotationType.Arrow)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Arrow;
                this.annotationType = AnnotationType.Arrow;
            }
            else
            {
                trashButton.IsVisible = true;
            }

            InitalizeCompenentsForShapeBar();
        }

        private void RectangleButton_Clicked(object sender, EventArgs e)
        {
            bottomMainToolbar.IsVisible = false;
            imageAnnotationType.HeightRequest = 28;
            imageAnnotationType.WidthRequest = 28;

            imageAnnotationType.Source = "ic_math.png";

            if (this.annotationType != AnnotationType.Rectangle)
            {
                pdfViewerControl.AnnotationMode = AnnotationMode.Rectangle;
                this.annotationType = AnnotationType.Rectangle;
            }
            else
            {
                trashButton.IsVisible = true;
            }

            InitalizeCompenentsForShapeBar();
        }

        private void InitalizeCompenentsForShapeBar()
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
            {
                //Show popup
                searchErrorPopupContent.NoOccurenceFound.IsVisible = true;
                searchErrorPopupContent.NoMoreOccurenceFound.IsVisible = false;
                errorSearchPopup.Show();
            }
            else if (args.NoMoreOccurrence)
            {
                //Show popup
                searchErrorPopupContent.NoOccurenceFound.IsVisible = false;
                searchErrorPopupContent.NoMoreOccurenceFound.IsVisible = true;
                errorSearchPopup.Show();
            }

            search_started = false;
        }

        private void TextSearchEntry_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textSearchEntry.Text) && !string.IsNullOrEmpty(textSearchEntry.Text))
            {
                //Initiates text search.
                pdfViewerControl.SearchText(textSearchEntry.Text);
            }
            if (string.IsNullOrWhiteSpace(textSearchEntry.Text) || string.IsNullOrEmpty(textSearchEntry.Text))
            {
                pdfViewerControl.CancelSearch();
                search_started = false;
            }
            if (!search_started)
            {

                pdfViewerControl.SearchText(textSearchEntry.Text);
            }
            else
            {
                pdfViewerControl.SearchNext();
            }

            search_started = true;
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
            search_started = false;

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