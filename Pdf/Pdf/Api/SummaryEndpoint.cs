using Newtonsoft.Json;
using Pdf.Interfaces;
using Pdf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pdf.Api
{
    public class SummaryEndpoint:ISummaryEndpoint 
    {
        public async Task<string> UploadFilesForSummary(FileInfo fileInfo, List<SummaryModel> summaries)
        {
            IAndroidFileHelper androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

            var content = new MultipartFormDataContent();

            var bytesFile = androidFileHelper.LoadLocalFile(fileInfo.FullName);
            var json = JsonConvert.SerializeObject(summaries, Formatting.Indented);

            ByteArrayContent byteArrayContent = new ByteArrayContent(bytesFile);

            content.Add(byteArrayContent, fileInfo.Name, fileInfo.Name);
            content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "list");

            var httpClient = new HttpClient();

            var uploadServiceBaseAdress = "http://10.0.2.2:44560/PostFilesForSummary/";

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
