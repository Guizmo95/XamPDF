using Syncfusion.SfImageEditor.XForms;
using Syncfusion.SfRangeSlider.XForms;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        RotatorPage rotatorPage;
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void HandlePopDelegate(string parameter);
        public event HandlePopDelegate DidFinishPoping;

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
            rotatorPage = new RotatorPage(Enumerations.RotatorMode.ColorPicker);

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
                    DidFinishPoping(null);
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
            editor.Save(".png", new Size(913, 764)) ;
        }

        public void Eraser_Clicked(object sender, System.EventArgs e)
        {
            editor.AddShape(Syncfusion.SfImageEditor.XForms.ShapeType.Path, new Syncfusion.SfImageEditor.XForms.PenSettings() { Color = Color.White, StrokeWidth = 20 });
        }

        private void editor_ImageSaving(object sender, Syncfusion.SfImageEditor.XForms.ImageSavingEventArgs args)
        {
            
        }

        private async void editor_ImageSaved(object sender, Syncfusion.SfImageEditor.XForms.ImageSavedEventArgs args)
        {
            // You can get the saved image location with the help of this argument
            string savedLocation = args.Location;

            await Navigation.PopAsync();
            DidFinishPoping(savedLocation);
        }

        private async void colorButtonClicked(object sender, EventArgs e)
        {
            rotatorPage.HeightRequest = 65;
            mainStackLayout.Children.Insert(3, rotatorPage);

            // get reference to the layout to animate
            //var layout = rotatorPage;

            ////setup information for animation
            //Action<double> callback = input => { layout.HeightRequest = input; }; // update the height of the layout with this callback
            //double startingHeight = layout.Height; // the layout's height when we begin animation
            //double endingHeight = 150; // final desired height of the layout
            //uint rate = 16; // pace at which aniation proceeds
            //uint length = 250; // one second animation
            //Easing easing = Easing.CubicOut; // There are a couple easing types, just tried this one for effect

            //// now start animation with all the setup information
            //layout.Animate("visibl", callback, startingHeight, endingHeight, rate, length, easing);

            //bottomToolbar.HasShadow = true;
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