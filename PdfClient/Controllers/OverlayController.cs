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
    public class OverlayController : ApiController
    {
        [Route("PostFilesForWatermark")]
        public HttpResponseMessage PostForWatermark()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                var httpRequest = HttpContext.Current.Request;
                List<string> filesNames = new List<string>();

                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        var fileName = postedFile.FileName;
                        string date = DateTime.Now.ToString();
                        date = PdftkTools.CleanDate(date);
                        fileName = date + fileName;
                        filesNames.Add(fileName);

                        var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);
                        postedFile.SaveAs(filePath);
                    }
                    string outputName = PdftkTools.AddWatermark(filesNames);

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

        [Route("PostFilesForStump")]
        public HttpResponseMessage PostForStamp()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                var httpRequest = HttpContext.Current.Request;
                List<string> filesNames = new List<string>();

                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        var fileName = postedFile.FileName;
                        string date = DateTime.Now.ToString();
                        date = PdftkTools.CleanDate(date);
                        fileName = date + fileName;
                        filesNames.Add(fileName);

                        var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);
                        postedFile.SaveAs(filePath);
                    }
                    string outputName = PdftkTools.AddStump(filesNames);

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
