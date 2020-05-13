using Pdf.Interfaces;
using SignaturePad.Forms;
using Syncfusion.SfImageEditor.XForms;
using Syncfusion.SfRangeSlider.XForms;
using Syncfusion.XForms.PopupLayout;
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
    public partial class SignaturePage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void HandlePopDelegate(string parameter);
        public event HandlePopDelegate DidFinishPoping;

        private Color selectedColor = Color.Black;

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

                signatureView.StrokeColor = value;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send(this, "AllowLandscape");

            MessagingCenter.Subscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor", (sender, helper) =>
            {
                this.SelectedColor = helper;
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send(this, "PreventLandscape");
            //during page close setting back to portrait 

            MessagingCenter.Unsubscribe<ColorPicker, Xamarin.Forms.Color>(this, "selectedColor");
        }

        public SignaturePage()
        {
            InitializeComponent();
        }

        #region On Property Changed

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private async void SaveImage_Clicked(object sender, EventArgs e)
        {
            try
            {
                Stream image = await signatureView.GetImageStreamAsync(SignatureImageFormat.Png);

                var downlaodPath = DependencyService.Get<IAndroidFileHelper>().GetDownloadPath();
                var filePath = Path.Combine(downlaodPath, "image");

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    image.CopyTo(fileStream);
                }

                await Navigation.PopAsync();
                DidFinishPoping(filePath);
            }
            catch (Exception ex)
            {
                DependencyService.Get<IToastMessage>().LongAlert("No signature added");
                await Navigation.PopAsync();
                DidFinishPoping(null);
            }
        }
    }
}