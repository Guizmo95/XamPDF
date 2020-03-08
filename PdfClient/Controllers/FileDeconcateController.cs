using PdfClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace PdfClient.Controllers
{
    public class FileDeconcateController : ApiController
    {
        [Route("DeconcatePages")]
        public HttpResponseMessage Post([FromUri]List<int> pages)
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                var httpRequest = HttpContext.Current.Request;
                
                if(httpRequest.Files.Count != 0)
                {
                    var postedFile = httpRequest.Files[0];
                    var fileName = postedFile.FileName;
                    string date = DateTime.Now.ToString();
                    date = PdftkTools.CleanDate(date);
                    fileName = date + fileName;

                    var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);
                    postedFile.SaveAs(filePath);

                    List<string> outputNames = PdftkTools.DeconcatePages(fileName, pages);
                    response = Request.CreateResponse(HttpStatusCode.OK, outputNames);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return response;
        }

        //TODO LIST OF STRING RETURN
        public HttpResponseMessage GetFileConverted(List<string> fileName)
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
