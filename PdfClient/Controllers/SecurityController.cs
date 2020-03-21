using PdfClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace PdfClient.Controllers
{
    public class SecurityController : ApiController
    {
        [Route("PostFilesForPassword")]
        public HttpResponseMessage Post([FromUri]string password)
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

                    string outputName = PdftkTools.SetPassword(fileName, password);

                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(outputName);
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
