using Newtonsoft.Json;
using Pdf.Models;
using PdfClient.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
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

    public class AddSummaryController : ApiController
    {
        [Route("PostFilesForSummary")]
        public HttpResponseMessage Post()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                var httpRequest = HttpContext.Current.Request;

                var jsonContent = httpRequest.Form["list"];
                List<SummaryModel> summaries = JsonConvert.DeserializeObject<List<SummaryModel>>(jsonContent);

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

                    string outputName = PdftkTools.AddSummary(fileName, summaries);

                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(outputName);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                    return response;
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
