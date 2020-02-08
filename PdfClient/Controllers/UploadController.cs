using PdfClient.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PdfClient.Controllers
{
    [Route("api/Files/Upload")]
    public class UploadController : ApiController
    {

        [HttpPost]
        public  HttpResponseMessage Post()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                var httpRequest = HttpContext.Current.Request;
                
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        var fileName = postedFile.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault().Replace(" ", "");
                        string date = DateTime.Now.ToString();
                        date = ConvertFileTools.CleanDate(date);
                        fileName = date + fileName;

                        var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);
                        postedFile.SaveAs(filePath);
                    }

                    var fileName1 = httpRequest.Files[0].FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault().Replace(" ", "");
                    var fileName2 = httpRequest.Files[1].FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault().Replace(" ", "");

                    string outputName = ConvertFileTools.ConcateFiles(fileName1, fileName2);

                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(outputName);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                    return response;
                }
            }
            catch (Exception ex)
            {
                return response;
            }

            return response;
        }

        [HttpGet]

        //TODO - FIX CONTROLLER ERROR
        public HttpResponseMessage GetFileConverted(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/");

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream((filePath + fileName), FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            return response;

        }
    }
}