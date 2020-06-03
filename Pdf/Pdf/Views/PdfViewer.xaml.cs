using Android.Content;
using Android.OS;
using Android.Text;
using Android.Views;
using Pdf.controls;
using Pdf.Enumerations;
using Pdf.Interfaces;
using Pdf.Models;
using Pdf.ViewModels;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.SfPdfViewer.XForms;
using Syncfusion.ListView.XForms;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Java.Lang;
using Android.Content.Res;
using Acr.UserDialogs;
using Unity.Injection;
using Syncfusion.Pdf.Interactive;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfViewer : ContentPage, INotifyPropertyChanged
    {
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

        Label label;
        Entry entry;
        Button acceptButton;
        Button declineButton;

        private FreeTextAnnotation selectedFreeTextAnnotation;
        private InkAnnotation selectedInkAnnotation;
        private ShapeAnnotation selectedShapeAnnotation;
        private TextMarkupAnnotation selectedTextMarkupAnnotation;

        private AnnotationType annotationType;

        private Color selectedColor = Color.Black;
        private Rectangle lastRectangleBounds;
        private AnnotationMode lastTextMarkupAnnotationMode;

        private string filePath;

        private bool canSaveDocument;
        private bool canUndoInk = false;
        private bool canRedoInk = false;
        private bool toolbarIsCollapsed = false;
        private bool search_started;
        private bool isNoMatchFound;
        private bool isNoMoreOccurrenceFound;
        private bool isATryToOpenTheDocument = false;
        private bool hasAlreadyTriedToOpenDoc = false;
        private bool isPasswordPopupInitalized = false;

        private int fontSize = 6;
        private int shapesNumbers = 0;
        private int textMarkupNumbers = 0;
        private int lastThicknessBarSelected = 5;
        private int lastOpacitySelected = 4;
        private int numberOfAnnotation = 0;


        #region Property


        private int NumberOfAnnotation
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
            pdfViewerControl.SearchCompleted -= PdfViewerControl_SearchCompleted;
            pdfViewerControl.TextMatchFound -= PdfViewerControl_TextMatchFound;
            pdfViewerControl.Tapped -= PdfViewerControl_Tapped;
            pdfViewerControl.SearchInitiated -= PdfViewerControl_SearchInitiated;
            pdfViewerControl.StampAnnotationAdded -= PdfViewerControl_StampAnnotationAdded;

            styleContent.ThicknessBar.BoxViewButtonClicked -= ThicknessBar_Clicked;
            styleContent.OpacityButtonClicked -= OpacityIcon_Clicked;

            pdfViewerControl.PasswordErrorOccurred -= PdfViewerControl_PasswordErrorOccurred;

            if (passwordPopup != null)
            {
                passwordPopup.Closed -= PasswordPopup_Closed;
                acceptButton.Clicked -= AcceptButtonPasswordPopup_Clicked;
                declineButton.Clicked -= DeclineButton_Clicked;

                passwordPopup.IsOpen = false;
            }

            //pdfViewerControl.Unload();
            base.OnDisappearing();
        }

        protected async Task WaitAndExecute(int milisec, Action actionToExecute)
        {
            await Task.Delay(milisec);
            actionToExecute();
        }

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
            pdfViewerControl.Tapped += PdfViewerControl_Tapped;
            pdfViewerControl.SearchCompleted += PdfViewerControl_SearchCompleted;
            pdfViewerControl.TextMatchFound += PdfViewerControl_TextMatchFound;
            pdfViewerControl.SearchInitiated += PdfViewerControl_SearchInitiated;

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
                        UndoInk();
                        await Task.Delay(100);

                        if (CanUndoInk == true)
                            UndoButton.Foreground = Color.FromHex("#373737");
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
                Command = new Command(() =>
                {
                    PaletteButton_Clicked();
                })
            });

            pdfViewerControl.PreserveSignaturePadOrientation = true;

            paletteButton.BindingContext = this;
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
            stylePopup.PopupView.AnimationMode = AnimationMode.Fade;

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
            errorSearchPopup.PopupView.WidthRequest = 275;
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
            popupMenu.PopupView.ShowFooter = false;
            popupMenu.PopupView.PopupStyle.HeaderBackgroundColor = Color.FromHex("#eeeeee");
            popupMenu.PopupView.PopupStyle.BorderColor = Color.FromHex("#e0e0e0");

            popupMenuContent = new PopupMenuContent();

            DataTemplate popupMenuContentTemplate = new DataTemplate(() =>
            {
                return popupMenuContent;
            });

            DataTemplate headerTemplateViewPopupMenu = new DataTemplate(() =>
            {
                Label title = new Label();
                title.Text = "More";
                title.FontSize = 18;
                title.FontFamily = "GothamBold.ttf#GothamBold";
                title.VerticalTextAlignment = Xamarin.Forms.TextAlignment.Center;
                title.HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Center;
                title.TextColor = Color.FromHex("#4e4e4e");

                return title;
            });

            popupMenu.PopupView.HeaderTemplate = headerTemplateViewPopupMenu;

            popupMenu.PopupView.ContentTemplate = popupMenuContentTemplate;
            #endregion

            annotationType = AnnotationType.None;

            activityIndicator.IsVisible = false;
            activityIndicator.IsRunning = false;
        }


        #region PopupMenu Methods

        //TODO HANDLE SAVE FOR PRINT
        private async Task CompressPDF()
        {

        }


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
                    popupMenu.IsOpen = false;
                    await CompressPDF();
                    break;
                default:
                    break;
            }
        }

        //TODO CHECK IF OTHER ANNOTATION CAN BEN PRINTED FOR SAVE OR NOT THE PDF WHEN PRTINING
        private async Task PrintDocument()
        {
            Stream stream;
            var fileName = Path.GetFileName(this.filePath);

            popupMenu.IsOpen = false;

            using (UserDialogs.Instance.Loading("Loading", null, null, true, MaskType.Black))
            {
                await Task.Run(async () =>
                {
                    if (CanSaveDocument == true)
                    {
                        stream = await pdfViewerControl.SaveDocumentAsync();
                    }
                    else
                    {
                        stream = new FileStream(this.filePath, FileMode.Open);
                    }

                    await DependencyService.Get<IAndroidFileHelper>().Print(stream, fileName);
                });
            }
        }

        private async Task SaveDocument()
        {
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

            popupMenu.IsOpen = false;
        }

        private void MoreOptionButton_Clicked(object sender, EventArgs e)
        {
            popupMenu.Show();

            popupMenuContent.MenuListView.SelectionChanged += MenuListView_SelectionChanged;
            popupMenu.Closed += PopupMenu_Closed;
        }

        private void PopupMenu_Closed(object sender, EventArgs e)
        {
            popupMenuContent.MenuListView.SelectionChanged -= MenuListView_SelectionChanged;
            popupMenu.Closed -= PopupMenu_Closed;

        }

        #endregion  

        private void AcceptButtonPasswordPopup_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsRunning = true;
            activityIndicator.IsVisible = true;

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

            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;

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

        private void BookmarkButton_Clicked(object sender, EventArgs e)
        {
            //Bookmark pane will be collapsed, if it is expanded
            pdfViewerControl.BookmarkPaneVisible = true;
        }

        private async void PdfViewerControl_Tapped(object sender, TouchInteractionEventArgs e)
        {
            if (this.toolbarIsCollapsed == false)
            {
                DependencyService.Get<INavBarHelper>().SetImmersiveMode();

                pdfViewGrid.Margin = 0;

                await topToolbar.FadeTo(0, 150);
                await bottomMainBar.FadeTo(0, 150);

                toolbarIsCollapsed = true;
            }
            else
            {
                DependencyService.Get<INavBarHelper>().SetDefaultNavBar();

                await topToolbar.FadeTo(1, 150);
                await bottomMainBar.FadeTo(1, 150);

                await Task.Delay(150);

                pdfViewGrid.Margin = new Thickness(0, 45, 0, 45);

                toolbarIsCollapsed = false;
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

        #region Pdf viewer events methods

        private async void SignatureButton_Clicked(object sender, EventArgs e)
        {
            //var page = new SignaturePage();
            //page.DidFinishPoping += (parameter) =>
            //{
            //    if (!String.IsNullOrWhiteSpace(parameter) || !String.IsNullOrEmpty(parameter))
            //    {
            //        //set image source
            //        Image image = new Image();
            //        image.Source = ImageSource.FromFile(parameter);
            //        image.WidthRequest = 200;
            //        image.HeightRequest = 200;

            //        int numpage = pdfViewerControl.PageNumber;
            //        //add image as custom stamp to the first page
            //        pdfViewerControl.AddStamp(image, numpage, new Point(20,20));
            //    }
            //};
            //await Navigation.PushAsync(page);

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
            NumberOfAnnotation += 1;

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
            NumberOfAnnotation += 1;
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
            colorPicker.IsVisible = false;
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
            styleContent.ColorPicker.SelectedIndex = 0;
            this.FontSize = 8;
        }

        private void SetToolbarForInkAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;
            colorPicker.IsVisible = false;
            styleContent.FontSizeControl.IsVisible = false;
            viewModeButton.IsVisible = false;
            bookmarkButton.IsVisible = false;
            searchButton.IsVisible = false;
            moreOptionButton.IsVisible = false;

            imageAnnotationType.Source = "twotone_gesture_24.xml";

            paletteButton.IsVisible = true;
            annotationTypeToolbar.IsVisible = true;
            styleContent.OpacityControl.IsVisible = true;
            styleContent.ThicknessBar.IsVisible = true;
            styleContent.BoxView2.IsVisible = true;

            ValidButton.IsVisible = true;
            UndoButton.IsVisible = true;
            RedoButton.IsVisible = true;

            stylePopup.PopupView.HeightRequest = 240;
            stylePopup.PopupView.WidthRequest = 280;

            pdfViewerControl.AnnotationMode = AnnotationMode.Ink;
            pdfViewerControl.AnnotationSettings.Ink.Thickness = 9;
            this.annotationType = AnnotationType.Ink;
            styleContent.ColorPicker.SelectedIndex = 0;
        }

        private void SetToolbarForTextMarkupAnnotationSelected()
        {
            bottomMainToolbar.IsVisible = false;
            paletteButton.IsVisible = false;

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
                await textMarkupToolbar.LayoutTo(new Rectangle(textMarkupToolbar.Bounds.X, textMarkupToolbar.Bounds.Y, textMarkupToolbar.Bounds.Width, 0), 200, Easing.Linear);
            });

            textMarkupToolbar.IsVisible = false;
            bottomMainToolbar.IsVisible = true;
        }

        private async void StrikethroughtButton_Clicked(object sender, EventArgs e)
        {
            imageAnnotationType.Source = "ic_strikethrough.png";

            pdfViewerControl.AnnotationMode = AnnotationMode.Strikethrough;
            this.annotationType = AnnotationType.Strikethrought;

            colorPicker.SelectedIndex = 5;

            InitializeComponentForTextMarkupAnnotation();
        }

        private async void UnderlineButton_Clicked(object sender, EventArgs e)
        {
            imageAnnotationType.Source = "ic_underline.png";

            pdfViewerControl.AnnotationMode = AnnotationMode.Underline;
            this.annotationType = AnnotationType.Underline;
            //todo -- set for red color
            colorPicker.SelectedIndex = 4;

            InitializeComponentForTextMarkupAnnotation();
        }

        private async void HightlightButton_Clicked(object sender, EventArgs e)
        {
            imageAnnotationType.Source = "ic_edit.png";

            pdfViewerControl.AnnotationMode = AnnotationMode.Highlight;
            this.annotationType = AnnotationType.Hightlight;

            colorPicker.SelectedIndex = 5;

            InitializeComponentForTextMarkupAnnotation();
        }

        private void InitializeComponentForTextMarkupAnnotation()
        {
            paletteButton.IsVisible = false;
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
                    break;
                case AnnotationType.FreeText:
                    pdfViewerControl.AnnotationMode = AnnotationMode.None;
                    annotationType = AnnotationType.None;
                    bottomMainToolbar.IsVisible = true;
                    break;
                case (AnnotationType.Arrow):
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
            stylePopup.ShowRelativeToView(paletteButton, RelativePosition.AlignTopRight, 0, -7);
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
            imageAnnotationType.Source = "ic_ui.png";

            pdfViewerControl.AnnotationMode = AnnotationMode.Circle;
            this.annotationType = AnnotationType.Circle;

            InitalizeCompenentsForShapeBar();
        }

        private void LineButton_Clicked(object sender, EventArgs e)
        {
            imageAnnotationType.Source = "ic_square.png";

            pdfViewerControl.AnnotationMode = AnnotationMode.Line;
            this.annotationType = AnnotationType.Line;

            InitalizeCompenentsForShapeBar();
        }

        private void ArrowButton_Clicked(object sender, EventArgs e)
        {
            imageAnnotationType.Source = "ic_directional.png";

            pdfViewerControl.AnnotationMode = AnnotationMode.Arrow;
            this.annotationType = AnnotationType.Arrow;

            InitalizeCompenentsForShapeBar();
        }

        private void RectangleButton_Clicked(object sender, EventArgs e)
        {
            imageAnnotationType.Source = "ic_math.png";

            pdfViewerControl.AnnotationMode = AnnotationMode.Rectangle;
            this.annotationType = AnnotationType.Rectangle;

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
            searchBar.IsVisible = true;
        }

        private void CancelSearchButton_Clicked(object sender, EventArgs e)
        {
            PdfViewerControl.CancelSearch();

            topMainBar.IsVisible = true;
            searchBar.IsVisible = false;
        }

        private void PdfViewerControl_SearchCompleted(object sender, TextSearchCompletedEventArgs args)
        {
            isNoMatchFound = args.NoMatchFound;
            isNoMoreOccurrenceFound = args.NoMoreOccurrence;

            activityIndicator.IsVisible = false;
            activityIndicator.IsRunning = false;

            if (isNoMatchFound == true)
            {
                //Show popup
                searchErrorPopupContent.NoOccurenceFound.IsVisible = true;
                searchErrorPopupContent.NoMoreOccurenceFound.IsVisible = false;
                errorSearchPopup.Show();
                isNoMatchFound = false;
            }
            else
            {
                if (isNoMoreOccurrenceFound == true)
                {
                    //Show popup
                    searchErrorPopupContent.NoOccurenceFound.IsVisible = false;
                    searchErrorPopupContent.NoMoreOccurenceFound.IsVisible = true;
                    errorSearchPopup.Show();
                    isNoMoreOccurrenceFound = false;
                }
            }
            search_started = false;
        }

        private void TextSearchEntry_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textSearchEntry.Text) && !string.IsNullOrEmpty(textSearchEntry.Text))
            {
                var searchText = textSearchEntry.Text;
                //Initiates text search.
                pdfViewerControl.SearchText(searchText);
                pdfViewerControl.SearchNext();
            }
            if (string.IsNullOrWhiteSpace(textSearchEntry.Text) || string.IsNullOrEmpty(textSearchEntry.Text))
            {
                pdfViewerControl.CancelSearch();
                search_started = false;
            }
            if (!search_started)
            {

                pdfViewerControl.SearchText(textSearchEntry.Text);
                pdfViewerControl.SearchNext();
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

            activityIndicator.IsVisible = false;
            activityIndicator.IsRunning = false;
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
            activityIndicator.IsVisible = true;
            activityIndicator.IsRunning = true;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}