using PdfClient.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PdfClient.Controllers
{
    public class FileDeconcateController : ApiController
    {
        //TODO -- CHECK IF THIS WORK
        [Route("DeconcatePages")]
        public HttpResponseMessage Post([FromUri]List<int> pages)
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count != 0)
                {
                    var postedFile = httpRequest.Files[0];
                    var fileName = postedFile.FileName;
                    string date = DateTime.Now.ToString();
                    date = PdftkTools.CleanDate(date);
                    fileName = date + fileName;

                    var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);

                    postedFile.SaveAs(filePath);

                    while (!File.Exists(filePath))
                    {
                        Thread.Sleep(1000);
                    }

                    List<string> outputs = PdftkTools.DeconcatePages(fileName, pages);

                    string zipName = Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + ".zip";

                    string lastFilesPath = HttpContext.Current.Server.MapPath("~/Uploads/" + outputs.Last());

                    while (!File.Exists(lastFilesPath))
                    {
                        Thread.Sleep(1000);
                    }

                    ZipHelper.CreateZipFromFiles(zipName, outputs);

                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(zipName);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return response;
        }
    }
}
