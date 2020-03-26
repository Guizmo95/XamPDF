using Pdf.Enumerations;
using Pdf.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using System.Net.Http.Formatting;
using Pdf.Models;
using System.Web;
using Newtonsoft.Json;
using Pdf.Helpers;
using System.Net;

namespace Pdf
{
    //TODO -- REWORK FOR UPLOAD BAR
    public class FileEndpoint
    {
        //TODO - Gerer fichiers de mm nom
        public async Task<string> UploadFilesForConcate(List<FileInfo> filesInfo)
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

            var uploadServiceBaseAdress = "http://10.0.2.2:44560/PostFilesForConcateDocs/";

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

        public async Task<string> UploadFilesForConcate(FileInfo fileInfo, List<int> pagesNumbers)
        {
            IAndroidFileHelper androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

            var content = new MultipartFormDataContent();

            var bytesFile = androidFileHelper.LoadLocalFile(fileInfo.FullName);

            ByteArrayContent byteArrayContent = new ByteArrayContent(bytesFile);

            content.Add(byteArrayContent, fileInfo.Name, fileInfo.Name);

            var httpClient = new HttpClient();

            string pagesNumbersArg = "";

            int i = 0;
            pagesNumbers.ForEach(delegate (int number)
            {
                if (pagesNumbers.Last() != number)
                    pagesNumbersArg += "pages[" + i + "]=" + number + "&";
                else
                {
                    pagesNumbersArg += "pages[" + i + "]=" + number;
                }
                i++;
            });

            var uploadServiceBaseAdress = "http://10.0.2.2:44560/PostFilesForConcatePages?" + pagesNumbersArg;

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


        public async Task<List<string>> UploadFilesForDeconcate(FileInfo fileInfo, List<int> pagesNumbers)
        {
            IAndroidFileHelper androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

            var content = new MultipartFormDataContent();

            var bytesFile = androidFileHelper.LoadLocalFile(fileInfo.FullName);

            ByteArrayContent byteArrayContent = new ByteArrayContent(bytesFile);

            content.Add(byteArrayContent, fileInfo.Name, fileInfo.Name);

            var httpClient = new HttpClient();

            string pagesNumbersArg = "";

            int i = 0;
            pagesNumbers.ForEach(delegate (int number)
            {
                if (pagesNumbers.Last() != number)
                    pagesNumbersArg += "pages[" + i + "]=" + number + "&";
                else
                {
                    pagesNumbersArg += "pages[" + i + "]=" + number;
                }
                i++;
            });

            var uploadServiceBaseAdress = "http://10.0.2.2:44560/DeconcatePages?" + pagesNumbersArg;

            using (HttpResponseMessage response = await httpClient.PostAsync(uploadServiceBaseAdress, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    List<string> filesNames = await response.Content.ReadAsAsync<List<string>>();

                    return filesNames;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<string> UploadFilesForRemovePages(FileInfo fileInfo, List<int> pagesNumbers, IProgress<UploadBytesProgress> progessReporter)
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
                        UploadBytesProgress args = new UploadBytesProgress("http://10.0.2.2:44560/RemovePages?", (int)sent, (int)total);
                        progessReporter.Report(args);
                    });

                    string pagesNumbersArg = "";

                    int i = 0;
                    pagesNumbers.ForEach(delegate (int number)
                    {
                        if (pagesNumbers.Last() != number)
                            pagesNumbersArg += "pages[" + i + "]=" + number + "&";
                        else
                        {
                            pagesNumbersArg += "pages[" + i + "]=" + number;
                        }
                        i++;
                    });

                    var uploadServiceBaseAdress = "http://10.0.2.2:44560/RemovePages?" + pagesNumbersArg;

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

        //public async Task<string> UploadFilesForRemovePages(FileInfo fileInfo, List<int> pagesNumbers)
        //{
        //    IAndroidFileHelper androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

        //    var content = new MultipartFormDataContent();

        //    var bytesFile = androidFileHelper.LoadLocalFile(fileInfo.FullName);

        //    ByteArrayContent byteArrayContent = new ByteArrayContent(bytesFile);

        //    content.Add(byteArrayContent, fileInfo.Name, fileInfo.Name);

        //    var httpClient = new HttpClient();

        //    string pagesNumbersArg = "";

        //    int i = 0;
        //    pagesNumbers.ForEach(delegate (int number)
        //    {
        //        if (pagesNumbers.Last() != number)
        //            pagesNumbersArg += "pages[" + i + "]=" + number + "&";
        //        else
        //        {
        //            pagesNumbersArg += "pages[" + i + "]=" + number;
        //        }
        //        i++;
        //    });

        //    var uploadServiceBaseAdress = "http://10.0.2.2:44560/RemovePages?" + pagesNumbersArg;

        //    using (HttpResponseMessage response = await httpClient.PostAsync(uploadServiceBaseAdress, content))
        //    {
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string fileName = await response.Content.ReadAsStringAsync();

        //            return fileName;
        //        }
        //        else
        //        {
        //            throw new Exception(response.ReasonPhrase);
        //        }
        //    }
        //}

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

        public async Task<string> UploadFiles(FileInfo fileInfo, IProgress<UploadBytesProgress> progessReporter)
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
                        UploadBytesProgress args = new UploadBytesProgress("SERVEUR URL FOR UPLOAD", (int)sent, (int)total);
                        progessReporter.Report(args);
                    });


                    var uploadServiceBaseAdress = "SERVEUR URL FOR UPLOAD";

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

        //TODO -- CREATE WEB API CLIENT
        public async Task<byte[]> GetFileConcated(string fileName)
        {
            HttpClient httpClient = new HttpClient();

            var uploadServiceBaseAdress = "http://10.0.2.2:44560/GetFile/";
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


