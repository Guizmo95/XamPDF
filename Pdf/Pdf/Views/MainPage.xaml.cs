using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pdf
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void PickFileButton(object sender, EventArgs e)
        {
            try
            {
                FileData file = await CrossFilePicker.Current.PickFile(new string[] { "application/pdf" });
                if (file == null)
                    return; //user canceked file picking
                string fileName = file.FileName;
                string contents = System.Text.Encoding.UTF8.GetString(file.DataArray);

                await Navigation.PushAsync(new MenuPage(file));
            }

            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }
    }
    }

