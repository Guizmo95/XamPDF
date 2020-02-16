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
    [Route("api/[controller]")]
    public class FileConcateController : ApiController
    {

        [HttpPost]
        [Route("PostFiles")]
        public  HttpResponseMessage Post()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                var httpRequest = HttpContext.Current.Request;
                List<string> filesNames = new List<string>();
                
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        var fileName = postedFile.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault().Replace(" ", "");
                        string date = DateTime.Now.ToString();
                        date = ConvertFileTools.CleanDate(date);
                        fileName = date + fileName;
                        filesNames.Add(fileName);

                        var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);
                        postedFile.SaveAs(filePath);
                    }

                    

                    string outputName = ConvertFileTools.ConcateFiles(filesNames);

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



        [HttpGet]
        [Route("GetFile/{fileName}")]
        public HttpResponseMessage GetFileConverted(string fileName)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            if (String.IsNullOrEmpty(fileName))
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/" + fileName);
            byte[] pdf = File.ReadAllBytes(filePath);

            response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(pdf);
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            return response;
        }
    }
}