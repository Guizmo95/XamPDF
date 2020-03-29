using Pdf.Helpers;
using Pdf.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pdf.Api
{
    public class UncompressEndpoint:IUncompressEndpoint 
    {
        //WORK
        public async Task<string> UploadFilesForUncompress(FileInfo fileInfo, IProgress<UploadBytesProgress> progessReporter)
        {
            string res = null;
            IAndroidFileHelper androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

            using (var client = new HttpClient())
            {
                using (var multiForm = new MultipartFormDataContent())
                {
                    var bytesFile = androidFileHelper.LoadLocalFile(fileInfo.FullName);
                    ByteArrayContent byteArrayContent = new ByteArrayContent(bytesFile);

                    multiForm.Add(byteArrayContent, fileInfo.Name, fileInfo.Name);

                    var progressContent = new ProgressableStreamContent(multiForm, 4096, (sent, total) =>
                    {
                        UploadBytesProgress args = new UploadBytesProgress("http://10.0.2.2:44560/PostFilesForUncompressDocs/", (int)sent, (int)total);
                        progessReporter.Report(args);
                    });


                    var uploadServiceBaseAdress = "http://10.0.2.2:44560/PostFilesForUncompressDocs/";

                    var response = await client.PostAsync(uploadServiceBaseAdress, progressContent);
                    Console.WriteLine(response.StatusCode);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        res = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(res);

                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }

                    return res;
                }
            }
        }
    }
}
