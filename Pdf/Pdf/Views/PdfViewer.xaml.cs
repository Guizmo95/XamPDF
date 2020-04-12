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
        private readonly PdfViewerModel pdfViewerModel;
        private readonly RotatorPage rotatorPage;
        private readonly NavigationDrawerToolsMenu navigationDrawerToolsMenu;
        private readonly TextButtonClickedBottomBar textButtonClickedBottomBar;
        private readonly FontSizeSlider fontSizeSlider;
        private readonly EditFreeTextBar editFreeTextBar;
        private PenBottomBar penBottomBar;
        private FreeTextAnnotation selectedFreeTextAnnotation;
        

        private Color selectedColor = Color.Black;
        private int fontSize = 6;
        private Color statusColor = Color.FromHex("b4b4b4");
        private bool canUndoInk = false;
        private bool canRedoInk = false;

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
                    UndoButton.Foreground = Color.Black;
                else
                {
                    if (value == false)
                        UndoButton.Foreground = Color.White;
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

            this.editFreeTextBar = new EditFreeTextBar();
            this.editFreeTextBar.BindingContext = this;

            this.penBottomBar = new PenBottomBar();
            this.penBottomBar.BindingContext = this;
            #endregion

            #region Pen bottom bar events 
            penBottomBar.ColorButtonClicked += ColorPenButton_Clicked;
            penBottomBar.BackButtonClicked += BackPenButton_Clicked;
            penBottomBar.PenSizeButtonClicked += PenSizeButton_Clicked;
            penBottomBar.PenStatusButtonClicked += PenStatusButton_Clicked;
            #endregion

            #region Free Text button events 
            //TextButtonClickedBottomBar events
            textButtonClickedBottomBar.ColorButtonClicked += ColorTextButton_Clicked;
            textButtonClickedBottomBar.FontSizeButtonClicked += FontSizeButton_Clicked;
            textButtonClickedBottomBar.BackButtonClicked += BackTextButton_Clicked;
            textButtonClickedBottomBar.FreeTextButtonClicked += FreeTextButton_Clicked;

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

            #region Custom navigation drawer tool bar events
            navigationDrawerToolsMenu.SignaturePadButton += async () => await SignaturePadButton_Clicked();
            navigationDrawerToolsMenu.StampButtonClicked += () => StampButton_Clicked();
            navigationDrawerToolsMenu.TextButtonClicked += () => TextButton_Clicked();
            navigationDrawerToolsMenu.PenButtonClicked += () => PenButton_Clicked();
            #endregion

            #region Pdf viewer events
            pdfViewerControl.FreeTextAnnotationAdded += PdfViewerControl_FreeTextAnnotationAdded;
            pdfViewerControl.FreeTextPopupDisappeared += PdfViewerControl_FreeTextPopupDisappeared;
            pdfViewerControl.FreeTextAnnotationSelected += PdfViewerControl_FreeTextAnnotationSelected;
            pdfViewerControl.CanRedoInkModified += PdfViewerControl_CanRedoInkModified;
            pdfViewerControl.CanUndoInkModified += PdfViewerControl_CanUndoInkModified;
            #endregion

            #region Add content view but set unvisible 
            rotatorPage.IsVisible = false;
            mainStackLayout.Children.Add(rotatorPage);
            fontSizeSlider.IsVisible = false;
            mainStackLayout.Children.Insert(3, fontSizeSlider);
            textButtonClickedBottomBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, textButtonClickedBottomBar);
            editFreeTextBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, editFreeTextBar);
            penBottomBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, penBottomBar);

            ValidInkButton.IsVisible = false;
            UndoButton.IsVisible = false;
            RedoButton.IsVisible = false;
            #endregion

            BindingContext = pdfViewerModel = new PdfViewerModel(fileStream);

            pdfViewerControl.Toolbar.Enabled = true;

            ToolsButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
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

        #region Pdf viewer events methods
        private void PdfViewerControl_FreeTextAnnotationSelected(object sender, FreeTextAnnotationSelectedEventArgs args)
        {
            if(textButtonClickedBottomBar.IsVisible == true)
                textButtonClickedBottomBar.IsVisible = false;
            
            if(bottomToolbar.IsVisible == true)
                bottomToolbar.IsVisible = false;

            editFreeTextBar.IsVisible = true;

            //Cast the sender object to FreeTextAnnotation
            selectedFreeTextAnnotation = sender as FreeTextAnnotation;
        }

        private void PdfViewerControl_FreeTextPopupDisappeared(object sender, FreeTextPopupDisappearedEventArgs args)
        {
            if (args.PopupResult == FreeTextPopupResult.Cancel)
            {
                //Bug - can't re-enabled the annotation free text then we disabled because we dont have the choice 
                FreeTextButton_Clicked();
            }
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
        #endregion

        #region TextButtonClickedBottomBar events methods
        private void FontSizeButton_Clicked()
        {
            if (fontSizeSlider.IsVisible == true)
            {
                fontSizeSlider.IsVisible = false;

                if(rotatorPage.IsVisible == true)
                    rotatorPage.IsVisible = false;
            }
            else
            {
                fontSizeSlider.IsVisible = true;
            }
        }

        private void ColorTextButton_Clicked()
        {
            if (rotatorPage.IsVisible == true)
            {
                rotatorPage.IsVisible = false;

                if(fontSizeSlider.IsVisible == true)
                    fontSizeSlider.IsVisible = false;
            }
            else
            {
                rotatorPage.IsVisible = true;
            }
        }

        private void BackTextButton_Clicked()
        {
            if(textButtonClickedBottomBar.IsVisible == true)
                textButtonClickedBottomBar.IsVisible = false;

            if(fontSizeSlider.IsVisible == true)
                fontSizeSlider.IsVisible = false;

            if(rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;


            bottomToolbar.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.None;
        }

        private void FreeTextButton_Clicked()
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

                editFreeTextBar.IsVisible = false;

                textButtonClickedBottomBar.IsVisible = true;
            }
        }

        private void BackButtonInFreeTextEditor_Clicked()
        {
            if(editFreeTextBar.IsVisible == true)
                editFreeTextBar.IsVisible = false;

            if(bottomToolbar.IsVisible == false)
                bottomToolbar.IsVisible = true;

            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            //Trick for deselect free text
            pdfViewerControl.AnnotationMode = AnnotationMode.FreeText;
            pdfViewerControl.AnnotationMode = AnnotationMode.None;

        }

        //TODO -- LOOK FOR CHANGE TEXT OF FREE TEXT SELECTED
        private void ChangeFreeTextSelected()
        {
            

        }
        #endregion

        #region Tools menu events methods

        private void PenButton_Clicked()
        {
            if (bottomToolbar.IsVisible == true)
                bottomToolbar.IsVisible = false;

            if (penBottomBar.IsVisible == false)
                penBottomBar.IsVisible = true;

            navigationDrawer.ToggleDrawer();

            ValidInkButton.IsVisible = true;
            UndoButton.IsVisible = true;
            RedoButton.IsVisible = true;

            pdfViewerControl.AnnotationMode = AnnotationMode.Ink;
        }

        private void TextButton_Clicked()
        {
            if (bottomToolbar.IsVisible == true)
                bottomToolbar.IsVisible = false;

            if (textButtonClickedBottomBar.IsVisible == false)
                textButtonClickedBottomBar.IsVisible = true;

            navigationDrawer.ToggleDrawer();

            pdfViewerControl.AnnotationMode = AnnotationMode.FreeText;
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

        #region Pen bottom bar events methods
        private void ColorPenButton_Clicked()
        {
            if (rotatorPage.IsVisible == true)
            {
                rotatorPage.IsVisible = false;
            }
            else
            {
                rotatorPage.IsVisible = true;
            }
        }

        private void BackPenButton_Clicked()
        {
            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            if (penBottomBar.IsVisible == true)
                penBottomBar.IsVisible = false;

            if (bottomToolbar.IsVisible == false)
                bottomToolbar.IsVisible = true;

            //Trick for deselect ink annotation 
            pdfViewerControl.AnnotationMode = AnnotationMode.Ink;
            pdfViewerControl.AnnotationMode = AnnotationMode.None;
        }

        private void PenStatusButton_Clicked()
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

        private void PenSizeButton_Clicked()
        {

        }

        #endregion

        #region Fire events manually 
        public static void FireEvent(object onMe, string invokeMe, params object[] eventParams)
        {
            MulticastDelegate eventDelagate =
                  (MulticastDelegate)onMe.GetType().GetField(invokeMe,
                   System.Reflection.BindingFlags.Instance |
                   System.Reflection.BindingFlags.NonPublic).GetValue(onMe);

            Delegate[] delegates = eventDelagate.GetInvocationList();

            foreach (Delegate dlg in delegates)
            {
                dlg.Method.Invoke(dlg.Target, eventParams);
            }
        }

        #endregion

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}