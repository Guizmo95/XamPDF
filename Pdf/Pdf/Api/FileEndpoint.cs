using Pdf.Droid;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;


namespace Pdf
{
    public class FileEndpoint
    {
        //TODO - Gerer fichiers de mm nom
        public async Task<string> UploadFiles(List<FileInfo> filesInfo)
        {
            IAndroidFileHelper androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

            var content = new MultipartFormDataContent();
            //FIX
            //filesInfo.ForEach(delegate (FileInfo fileInfo)
            //{
            //    var bytesFile = androidFileHelper.LoadLocalFile(fileInfo.FullName);

            //    using (ByteArrayContent byteArrayContent = new ByteArrayContent(bytesFile))
            //    {
            //        byteArrayContent.
            //        content.Add(byteArrayContent, "\"file\"", $"\"{fileInfo.FullName}\"");
            //    }
            //});

            var httpClient = new HttpClient();

            var uploadServiceBaseAdress = "http://10.0.2.2:50547/PostFiles";

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

        //TODO -- CREATE WEB API CLIENT
        public async Task<byte[]> GetFileConcated(string fileName)
        {
            HttpClient httpClient = new HttpClient();

            var uploadServiceBaseAdress = "http://10.0.2.2:50547/GetFile/";
            byte[] result;

            using (HttpResponseMessage response = await httpClient.GetAsync(uploadServiceBaseAdress + fileName))
            {
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsByteArrayAsync();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}


