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
        private readonly NavigationDrawerToolsMenu navigationDrawerToolsMenu;
        private readonly FreeTextButtonClickedBottomBar freeTextButtonClickedBottomBar;
        private readonly FontSizeSlider fontSizeSlider;
        private readonly EditFreeTextBar editFreeTextBar;
        private readonly InkBottomBar inkBottomBar;
        private readonly ThicknessBar thicknessBar;
        private readonly EditInkBar editInkBar;
        #endregion

        private FreeTextAnnotation selectedFreeTextAnnotation;
        private InkAnnotation selectedInkAnnotation;
       

        #region Property
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
                    ValidInkButton.Foreground = Color.Black;
                }
                else
                {
                    if (value == false)
                    {
                        UndoButton.Foreground = Color.White;
                        ValidInkButton.Foreground = Color.White;
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
        #endregion

        public PdfViewer(Stream fileStream)
        {
            InitializeComponent();

            #region Instanciation
            this.fileStream = fileStream;

            this.navigationDrawerToolsMenu = new NavigationDrawerToolsMenu();

            this.rotatorPage = new RotatorPage(RotatorMode.ColorPicker);

            this.freeTextButtonClickedBottomBar = new FreeTextButtonClickedBottomBar();
            this.freeTextButtonClickedBottomBar.BindingContext = this;

            this.fontSizeSlider = new FontSizeSlider();
            this.fontSizeSlider.BindingContext = this;

            this.editFreeTextBar = new EditFreeTextBar();
            this.editFreeTextBar.BindingContext = this;

            this.inkBottomBar = new InkBottomBar();
            this.inkBottomBar.BindingContext = this;

            this.thicknessBar = new ThicknessBar();

            this.editInkBar = new EditInkBar();
            this.editInkBar.BindingContext = this;
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
            pdfViewerControl.FreeTextAnnotationDeselected += PdfViewerControl_FreeTextAnnotationDeselected;
            pdfViewerControl.CanRedoInkModified += PdfViewerControl_CanRedoInkModified;
            pdfViewerControl.CanUndoInkModified += PdfViewerControl_CanUndoInkModified;

            pdfViewerControl.InkSelected += PdfViewerControl_InkSelected;
            pdfViewerControl.InkDeselected += PdfViewerControl_InkDeselected;

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

            ValidInkButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if (CanUndoInk == true)
                    {
                        ValidInkButton.Foreground = Color.FromHex("#b4b4b4");
                        SaveInk();
                        await Task.Delay(100);

                        CanRedoInk = false;
                        CanUndoInk = false;
                        StatusColor = Color.Transparent;
                    }
                })
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
            rotatorPage.IsVisible = false;
            mainStackLayout.Children.Add(rotatorPage);
            fontSizeSlider.IsVisible = false;
            mainStackLayout.Children.Insert(3, fontSizeSlider);
            freeTextButtonClickedBottomBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, freeTextButtonClickedBottomBar);
            editFreeTextBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, editFreeTextBar);
            inkBottomBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, inkBottomBar);
            thicknessBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, thicknessBar);
            editInkBar.IsVisible = false;
            mainStackLayout.Children.Insert(2, editInkBar);

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


        private void PdfViewerControl_FreeTextAnnotationDeselected(object sender, FreeTextAnnotationDeselectedEventArgs args)
        {
            if (editFreeTextBar.IsVisible == true)
                editFreeTextBar.IsVisible = false;

            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            freeTextButtonClickedBottomBar.IsVisible = true;
        }

        private void PdfViewerControl_FreeTextAnnotationSelected(object sender, FreeTextAnnotationSelectedEventArgs args)
        {
            if(freeTextButtonClickedBottomBar.IsVisible == true)
                freeTextButtonClickedBottomBar.IsVisible = false;
            
            if(bottomToolbar.IsVisible == true)
                bottomToolbar.IsVisible = false;

            editFreeTextBar.IsVisible = true;

            //Cast the sender object to FreeTextAnnotation
            selectedFreeTextAnnotation = sender as FreeTextAnnotation;
        }

        private void PdfViewerControl_FreeTextPopupDisappeared(object sender, FreeTextPopupDisappearedEventArgs args)
        {

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
            pdfViewerControl.EndInkSession(true);
        }

        private void PdfViewerControl_InkSelected(object sender, InkSelectedEventArgs args)
        {
            //Casts the sender object as Ink annotation.
            selectedInkAnnotation = sender as InkAnnotation;

            if (inkBottomBar.IsVisible == true)
                inkBottomBar.IsVisible = false;

            editInkBar.IsVisible = true;
        }

        private void PdfViewerControl_InkDeselected(object sender, InkDeselectedEventArgs args)
        {
            BackButton_Clicked();
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
            #region Free Text button events 
            //TextButtonClickedBottomBar events
            freeTextButtonClickedBottomBar.ColorButtonClicked -= ColorTextButton_Clicked;
            freeTextButtonClickedBottomBar.FontSizeButtonClicked -= FontSizeButton_Clicked;
            freeTextButtonClickedBottomBar.BackButtonClicked -= BackTextButton_Clicked;
            freeTextButtonClickedBottomBar.FreeTextButtonClicked -= FreeTextButton_Clicked;
            #endregion

            #region Free Text edit button events
            //EditFreeTextBar events
            editFreeTextBar.TrashCanButtonClicked -= TrashCanButton_Clicked;
            editFreeTextBar.FontSizeButtonClicked -= FontSizeButton_Clicked;
            editFreeTextBar.ColorButtonClicked -= ColorTextButton_Clicked;
            editFreeTextBar.BackButtonClicked -= BackButtonInFreeTextEditor_Clicked;
            editFreeTextBar.ChangeTextInFreeTextSelectedButtonClicked -= ChangeFreeTextSelected;

            MessagingCenter.Unsubscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor");
            #endregion

            if (freeTextButtonClickedBottomBar.IsVisible == true)
                freeTextButtonClickedBottomBar.IsVisible = false;

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

                freeTextButtonClickedBottomBar.IsVisible = true;
            }
        }

        private void BackButtonInFreeTextEditor_Clicked()
        {
            if(editFreeTextBar.IsVisible == true)
                editFreeTextBar.IsVisible = false;

            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            freeTextButtonClickedBottomBar.IsVisible = true;

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
            #region Ink bottom bar events 
            inkBottomBar.ColorButtonClicked += ColorInkButton_Clicked;
            inkBottomBar.BackButtonClicked += BackPenButton_Clicked;
            inkBottomBar.PenSizeButtonClicked += InkThicknessButton_Clicked;
            inkBottomBar.PenStatusButtonClicked += PenStatusButton_Clicked;
            #endregion

            #region Ink bottom bar edit events 
            editInkBar.ColorButtonClicked += ColorInkButton_Clicked;
            editInkBar.TrashButtonClicked += TrashButton_Clicked;
            editInkBar.InkSizeButtonClicked += InkThicknessButton_Clicked;
            editInkBar.BackButtonClicked += BackButton_Clicked;
            #endregion

            if (bottomToolbar.IsVisible == true)
                bottomToolbar.IsVisible = false;

            if (inkBottomBar.IsVisible == false)
                inkBottomBar.IsVisible = true;

            navigationDrawer.ToggleDrawer();

            ValidInkButton.IsVisible = true;
            UndoButton.IsVisible = true;
            RedoButton.IsVisible = true;

            this.StatusColor = Color.FromHex("b4b4b4");

            pdfViewerControl.AnnotationMode = AnnotationMode.Ink;
        }

        private void TextButton_Clicked()
        {
            #region Free Text button events 
            //TextButtonClickedBottomBar events
            freeTextButtonClickedBottomBar.ColorButtonClicked += ColorTextButton_Clicked;
            freeTextButtonClickedBottomBar.FontSizeButtonClicked += FontSizeButton_Clicked;
            freeTextButtonClickedBottomBar.BackButtonClicked += BackTextButton_Clicked;
            freeTextButtonClickedBottomBar.FreeTextButtonClicked += FreeTextButton_Clicked;
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

            if (bottomToolbar.IsVisible == true)
                bottomToolbar.IsVisible = false;

            if (freeTextButtonClickedBottomBar.IsVisible == false)
                freeTextButtonClickedBottomBar.IsVisible = true;

            this.StatusColor = Color.FromHex("b4b4b4");

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

        private void BackPenButton_Clicked()
        {
            #region Ink bottom bar events 
            inkBottomBar.ColorButtonClicked -= ColorInkButton_Clicked;
            inkBottomBar.BackButtonClicked -= BackPenButton_Clicked;
            inkBottomBar.PenSizeButtonClicked -= InkThicknessButton_Clicked;
            inkBottomBar.PenStatusButtonClicked -= PenStatusButton_Clicked;
            #endregion

            #region Ink bottom bar edit events 
            editInkBar.ColorButtonClicked -= ColorInkButton_Clicked;
            editInkBar.TrashButtonClicked -= TrashButton_Clicked;
            editInkBar.InkSizeButtonClicked -= InkThicknessButton_Clicked;
            editInkBar.BackButtonClicked -= BackButton_Clicked;
            #endregion

            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            if (inkBottomBar.IsVisible == true)
                inkBottomBar.IsVisible = false;

            if (thicknessBar.IsVisible == true)
                thicknessBar.IsVisible = false;

            if (bottomToolbar.IsVisible == false)
                bottomToolbar.IsVisible = true;

            ValidInkButton.IsVisible = false;
            UndoButton.IsVisible = false;
            RedoButton.IsVisible = false;

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

        #endregion

        #region Ink edit bottom bar

        private void TrashButton_Clicked()
        {
            if (selectedInkAnnotation != null)
                //Removes the selected annotation from the PDF viewer.
                pdfViewerControl.RemoveAnnotation(selectedInkAnnotation);

            editInkBar.IsVisible = false;

            inkBottomBar.IsVisible = true;
        }

        private void BackButton_Clicked()
        {
            editInkBar.IsVisible = false;

            if (rotatorPage.IsVisible == true)
                rotatorPage.IsVisible = false;

            if (thicknessBar.IsVisible == true)
                thicknessBar.IsVisible = false;

            if(selectedInkAnnotation != null)
                //Trick for deselect and re-select ink annotation 
                pdfViewerControl.AnnotationMode = AnnotationMode.Ink;

            this.StatusColor = Color.FromHex("b4b4b4");

            inkBottomBar.IsVisible = true;
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