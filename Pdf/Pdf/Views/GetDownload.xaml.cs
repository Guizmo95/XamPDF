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

        private readonly string fileName;
        private readonly List<string> filesNames;

        public GetDownload(string fileName)
        {
            InitializeComponent();
            this.fileName = fileName;
        }

        public GetDownload(List<string> filesNames)
        {
            InitializeComponent();
            this.filesNames = filesNames;
        }

        private async void Download(object sender, EventArgs e)
        {
            try {
                //TODO DOWNLOAD FOR LIST
                if(filesNames.Count == 0)
                {
                    var file = await fileEndpoint.GetFileConcated(fileName);
                    androidWritter.SaveFile(fileName, file);
                }
                else
                {
                    filesNames.ForEach(async delegate (string fileName)
                    {
                        var file = await fileEndpoint.GetFileConcated(fileName);
                        androidWritter.SaveFile(fileName, file);
                    });
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}