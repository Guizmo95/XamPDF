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
    public class OverlayEndpoint:IOverlayEndpoint 
    {
        public async Task<string> UploadFilesForWatermark(List<FileInfo> filesInfo)
        {
            IAndroidFileHelper androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

            var content = new MultipartFormDataContent();

            filesInfo.ForEach(delegate (FileInfo fileInfo)
            {
                var bytesFile = androidFileHelper.LoadLocalFile(fileInfo.FullName);

                ByteArrayContent byteArrayContent = new ByteArrayContent(bytesFile);

                content.Add(byteArrayContent, fileInfo.Name, fileInfo.Name);
            });

            var httpClient = new HttpClient();

            var uploadServiceBaseAdress = "http://10.0.2.2:44560/PostFilesForWatermark/";

            using (HttpResponseMessage response = await httpClient.PostAsync(uploadServiceBaseAdress, content))
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

        public async Task<string> UploadFilesForStump(List<FileInfo> filesInfo)
        {
            IAndroidFileHelper androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

            var content = new MultipartFormDataContent();

            filesInfo.ForEach(delegate (FileInfo fileInfo)
            {
                var bytesFile = androidFileHelper.LoadLocalFile(fileInfo.FullName);

                ByteArrayContent byteArrayContent = new ByteArrayContent(bytesFile);

                content.Add(byteArrayContent, fileInfo.Name, fileInfo.Name);
            });

            var httpClient = new HttpClient();

            var uploadServiceBaseAdress = "http://10.0.2.2:44560/PostFilesForStump/";

            using (HttpResponseMessage response = await httpClient.PostAsync(uploadServiceBaseAdress, content))
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
