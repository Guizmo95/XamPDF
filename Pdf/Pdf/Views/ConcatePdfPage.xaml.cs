using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConcatePdfPage : ContentPage
    {
        public ConcatePdfPage()
        {
            InitializeComponent();
        }

        private FileData fileData1;
        private FileData fileData2;
        public ConcatePdfPage(FileData fileData1)
        {
            InitializeComponent();
            this.fileData1 = fileData1;
        }

        private async void PickFileButton(object sender, EventArgs e)
        {
            try
            {
                fileData2 = await CrossFilePicker.Current.PickFile(new string[] { "application/pdf" });
                if (fileData2 == null)
                {
                    return;
                }
                string fileName = fileData2.FileName;
                string contents = System.Text.Encoding.UTF8.GetString(fileData2.DataArray);

                FileUploadHelper.UploadFile(fileData1, fileData2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}