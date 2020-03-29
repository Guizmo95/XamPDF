using Pdf.Helpers;
using Pdf.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pdf.Api
{
    public class DeconcateEndpoint:IDeconcateEndpoint 
    {
        //DONT WORK
        public async Task<string> UploadFilesForDeconcate(FileInfo fileInfo, List<int> pagesNumbers, IProgress<UploadBytesProgress> progessReporter)
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
                        UploadBytesProgress args = new UploadBytesProgress("http://10.0.2.2:44560/DeconcatePages?", (int)sent, (int)total);
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

                    var uploadServiceBaseAdress = "http://10.0.2.2:44560/DeconcatePages?" + pagesNumbersArg;

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
