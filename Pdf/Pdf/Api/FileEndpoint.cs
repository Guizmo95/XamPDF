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
        public async Task<string> UploadFile(FileData fileData1, FileData fileData2)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StreamContent(fileData1.GetStream()), "\"file\"", $"\"{fileData1.FilePath}\"");
            content.Add(new StreamContent(fileData1.GetStream()), "\"file\"", $"\"{fileData2.FilePath}\"");

            var httpClient = new HttpClient();

            var uploadServiceBaseAdress = "http://10.0.2.2:51549/PostFiles";

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

            var uploadServiceBaseAdress = "http://10.0.2.2:51549/GetFile/";
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


