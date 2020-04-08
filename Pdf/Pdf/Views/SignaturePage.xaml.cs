using Syncfusion.SfImageEditor.XForms;
using Syncfusion.SfRangeSlider.XForms;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class SignaturePage : ContentPage, INotifyPropertyChanged
    {
        private Color selectedColor;
        private ColorPicker colorPicker;
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
                OnPropertyChanged();
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send(this, "AllowLandscape");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send(this, "PreventLandscape"); 
            //during page close setting back to portrait 
        }

        public SignaturePage()
        {
            InitializeComponent();
            this.editor.ToolbarSettings.IsVisible = false;

            colorPicker = new ColorPicker();

            Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
            {
                SelectedColor = Color.Black;
                editor.AddShape(ShapeType.Path, new PenSettings() { Color = SelectedColor });
                return false;
            });

            redo.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    redo.Foreground = Color.FromHex("#b4b4b4");
                    Redo_Clicked();
                    await Task.Delay(100);
                    redo.Foreground = Color.FromHex("4e4e4e");
                })
            });

            undo.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    undo.Foreground = Color.FromHex("#b4b4b4");
                    Undo_Clicked();
                    await Task.Delay(100);
                    undo.Foreground = Color.FromHex("4e4e4e");
                })
            });

            cross.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    cross.Foreground = Color.FromHex("#b4b4b4");
                    await Navigation.PopAsync();
                    await Task.Delay(100);
                    cross.Foreground = Color.FromHex("4e4e4e");
                })
            });

            save.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    save.Foreground = Color.FromHex("#b4b4b4");
                    Save_Clicked();
                    await Task.Delay(100);
                    save.Foreground = Color.FromHex("4e4e4e");
                })
            });
        }

        public void Undo_Clicked()
        {
            editor.Undo();
        }

        public void Redo_Clicked()
        {
            editor.Redo();
        }

        public void Reset_Clicked(object sender, System.EventArgs e)
        {
            editor.Reset();
        }

        public void Save_Clicked()
        {
            editor.Save(".png");
        }

        public void Eraser_Clicked(object sender, System.EventArgs e)
        {
            editor.AddShape(Syncfusion.SfImageEditor.XForms.ShapeType.Path, new Syncfusion.SfImageEditor.XForms.PenSettings() { Color = Color.White, StrokeWidth = 20 });
        }

        private void editor_ImageSaving(object sender, Syncfusion.SfImageEditor.XForms.ImageSavingEventArgs args)
        {
            
        }

        private void editor_ImageSaved(object sender, Syncfusion.SfImageEditor.XForms.ImageSavedEventArgs args)
        {
            string savedLocation = args.Location; // You can get the saved image location with the help of this argument
        }

        private async void colorButtonClicked(object sender, EventArgs e)
        {
            SfPopupLayout popupLayout = new SfPopupLayout();
            popupLayout.PopupView.ShowHeader = false;
            popupLayout.PopupView.ShowFooter = false;

            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                StackLayout stackLayout1 = new StackLayout();
                stackLayout1.HorizontalOptions = LayoutOptions.Center;
                stackLayout1.VerticalOptions = LayoutOptions.Center;
                stackLayout1.BackgroundColor = Color.Red;
                stackLayout1.Orientation = StackOrientation.Horizontal;

                StackLayout stackLayout2 = new StackLayout();
                stackLayout2.HorizontalOptions = LayoutOptions.FillAndExpand;
                stackLayout2.VerticalOptions = LayoutOptions.FillAndExpand;
                stackLayout2.BackgroundColor = Color.White;
                stackLayout2.Orientation = StackOrientation.Vertical;

                SfRangeSlider rangeSlider = new SfRangeSlider();
                rangeSlider.VerticalOptions = LayoutOptions.Center;
                rangeSlider.Minimum = 0;
                rangeSlider.Maximum = 12;
                rangeSlider.RangeStart = 0;
                rangeSlider.TickPlacement = TickPlacement.Inline;
                rangeSlider.StepFrequency = 1;
                rangeSlider.ShowRange = true;
                rangeSlider.Orientation = Orientation.Horizontal;
                rangeSlider.ToolTipPrecision = 1;
                this.Content = rangeSlider;

                colorPicker.HeightRequest = 55;
                colorPicker.VerticalOptions = LayoutOptions.Center;

                stackLayout2.Children.Add(rangeSlider);
                stackLayout2.Children.Add(colorPicker);
                stackLayout1.Children.Add(stackLayout2);
                return stackLayout1;
            });

            popupLayout.PopupView.HeightRequest = 350;
            popupLayout.PopupView.WidthRequest = 400;
            popupLayout.PopupView.ContentTemplate = dataTemplate;

            popupLayout.Show();
        }

        #region On Property Changed

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}