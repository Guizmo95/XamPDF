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
    public class OverlayEndpoint:IOverlayEndpoint 
    {
        public async Task<string> UploadFilesForWatermark(List<FileInfo> filesInfo, IProgress<UploadBytesProgress> progessReporter)
        {
            string res = null;
            IAndroidFileHelper androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

            using (var client = new HttpClient())
            {
                using (var multiForm = new MultipartFormDataContent())
                {
                    filesInfo.ForEach(delegate (FileInfo fileInfo)
                    {
                        var bytesFile = androidFileHelper.LoadLocalFile(fileInfo.FullName);

                        ByteArrayContent byteArrayContent = new ByteArrayContent(bytesFile);

                        multiForm.Add(byteArrayContent, fileInfo.Name, fileInfo.Name);
                    });

                    var progressContent = new ProgressableStreamContent(multiForm, 4096, (sent, total) =>
                    {
                        UploadBytesProgress args = new UploadBytesProgress("http://10.0.2.2:44560/PostFilesForWatermark/", (int)sent, (int)total);
                        progessReporter.Report(args);
                    });

                    var uploadServiceBaseAdress = "http://10.0.2.2:44560/PostFilesForWatermark/";

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

        public async Task<string> UploadFilesForStump(List<FileInfo> filesInfo, IProgress<UploadBytesProgress> progessReporter)
        {
            string res = null;
            IAndroidFileHelper androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

            using (var client = new HttpClient())
            {
                using (var multiForm = new MultipartFormDataContent())
                {
                    filesInfo.ForEach(delegate (FileInfo fileInfo)
                    {
                        var bytesFile = androidFileHelper.LoadLocalFile(fileInfo.FullName);

                        ByteArrayContent byteArrayContent = new ByteArrayContent(bytesFile);

                        multiForm.Add(byteArrayContent, fileInfo.Name, fileInfo.Name);
                    });

                    var progressContent = new ProgressableStreamContent(multiForm, 4096, (sent, total) =>
                    {
                        UploadBytesProgress args = new UploadBytesProgress("http://10.0.2.2:44560/PostFilesForStump/", (int)sent, (int)total);
                        progessReporter.Report(args);
                    });

                    var uploadServiceBaseAdress = "http://10.0.2.2:44560/PostFilesForStump/";

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
