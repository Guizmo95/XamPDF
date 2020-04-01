using Newtonsoft.Json;
using Pdf.Helpers;
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
        public async Task<string> UploadFilesForSummary(FileInfo fileInfo, List<SummaryModel> summaries, IProgress<UploadBytesProgress> progessReporter)
        {
            string res = null;
            IAndroidFileHelper androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

            using (var client = new HttpClient())
            {
                using (var multiForm = new MultipartFormDataContent())
                {
                    var bytesFile = androidFileHelper.LoadLocalFile(fileInfo.FullName);
                    var json = JsonConvert.SerializeObject(summaries, Formatting.Indented);
                    ByteArrayContent byteArrayContent = new ByteArrayContent(bytesFile);

                    multiForm.Add(byteArrayContent, fileInfo.Name, fileInfo.Name);
                    multiForm.Add(new StringContent(json, Encoding.UTF8, "application/json"), "list");

                    var progressContent = new ProgressableStreamContent(multiForm, 4096, (sent, total) =>
                    {
                        UploadBytesProgress args = new UploadBytesProgress("http://10.0.2.2:44560/PostFilesForSummary?", (int)sent, (int)total);
                        progessReporter.Report(args);
                    });

                    var uploadServiceBaseAdress = "http://10.0.2.2:44560/PostFilesForSummary/";

                    using (HttpResponseMessage response = await client.PostAsync(uploadServiceBaseAdress, progressContent))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            res = await response.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            throw new Exception(response.ReasonPhrase);
                        }
                    }
                    return res;
                }
            }
        }
    }
}
