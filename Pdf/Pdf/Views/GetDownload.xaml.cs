using Pdf.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetDownload : ContentPage
    {
        private FileEndpoint fileEndpoint = new FileEndpoint();

        IAndroidFileHelper androidWritter =  DependencyService.Get<IAndroidFileHelper>();

        private string fileName;

        public GetDownload(string fileName)
        {
            InitializeComponent();
            this.fileName = fileName;
        }

        private async void Download(object sender, EventArgs e)
        {
            try {
                //TODO -- SOLVER ERROR 
                var file = await fileEndpoint.GetFileConcated(fileName);
                androidWritter.SaveFile(fileName, file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}