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


namespace Pdf
{
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

            var uploadServiceBaseAdress = "http://10.0.2.2:44560/PostFilesForConcatePages?"+ pagesNumbersArg;

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


        public async Task<List<string>> UploadFilesForDeoncate(FileInfo fileInfo, List<int> pagesNumbers)
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

        //TODO CODE ENDPOIT FOR GET LIST OF FILES
    }
}


