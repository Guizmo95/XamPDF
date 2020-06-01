using Android.Content.Res;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompressDocumentPage : ContentPage, INotifyPropertyChanged
    {
        private string filePath;
        private int numberOfCheckboxChecked = 0;
        private bool canCompress;

        public int NumberOfCheckboxChecked
        {
            get
            {
                return numberOfCheckboxChecked;
            }
            set
            {
                numberOfCheckboxChecked = value;

                if (numberOfCheckboxChecked == 0)
                    CanCompress = false;
                else
                    CanCompress = true;

                OnPropertyChanged();
            }
        }
        public bool CanCompress
        {
            get
            {
                return canCompress;
            }
            set
            {
                canCompress = value;
                OnPropertyChanged();
            }
        }

        public CompressDocumentPage(string filePath)
        {
            InitializeComponent();

            BindingContext = this;
            this.filePath = filePath;
        }

        private void UpdateCompressButtonStatus(bool hasBeenChecked)
        {
            if(hasBeenChecked == true)
            {
                NumberOfCheckboxChecked += 1;
            }
            else
            {
                NumberOfCheckboxChecked -= 1;

            }
        }

        private void OptimizePageContentCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            UpdateCompressButtonStatus(e.Value);
        }

        private void RemoveMetadaCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            UpdateCompressButtonStatus(e.Value);
        }

        private void OptimzeFontCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            UpdateCompressButtonStatus(e.Value);
        }

        private void DisableIncrementalUpdateCheckox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            UpdateCompressButtonStatus(e.Value);
        }

        private void RemoveAnnotationCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            UpdateCompressButtonStatus(e.Value);
        }

        private void RemoveFormFieldsCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            UpdateCompressButtonStatus(e.Value);
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            UpdateCompressButtonStatus(e.Value);

            if(e.Value == true)
            {
                imageQualityLabel.IsEnabled = true;
                qualityPicker.IsEnabled = true;
            }
            else
            {
                imageQualityLabel.IsEnabled = false;
                qualityPicker.IsEnabled = false;
            }
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void CompressButton_Clicked(object sender, EventArgs e)
        {


            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}