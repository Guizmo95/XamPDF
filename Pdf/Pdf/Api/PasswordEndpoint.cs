using Pdf.Helpers;
using Pdf.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pdf.Api
{
    public class PasswordEndpoint:IPasswordEndpoint 
    {
        //TODO CHANGE THIS 
        public async Task<string> UploadFilesForPassword(FileInfo fileInfo, string password)
        {
            IAndroidFileHelper androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

            var content = new MultipartFormDataContent();

            var bytesFile = androidFileHelper.LoadLocalFile(fileInfo.FullName);

            ByteArrayContent byteArrayContent = new ByteArrayContent(bytesFile);

            content.Add(byteArrayContent, fileInfo.Name, fileInfo.Name);

            var httpClient = new HttpClient();

            var uploadServiceBaseAdress = "http://10.0.2.2:44560/PostFilesForPassword?password" + password;

            var request = new HttpRequestMessage(HttpMethod.Post, uploadServiceBaseAdress);

            var progressContent = new ProgressableStreamContent(content, 4096,
            (sent, total) =>
            {
                Console.WriteLine("Uploading {0}/{1}", sent, total);
            });

            request.Content = progressContent;

            using (HttpResponseMessage response = await httpClient.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    string fileName = await response.Content.ReadAsStringAsync();

                    return fileName;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
