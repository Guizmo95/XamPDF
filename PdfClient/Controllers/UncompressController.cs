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
    public class UncompressController : ApiController
    {
        [Route("PostFilesForUncompressDocs")]
        public HttpResponseMessage Post()
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

                        var fileName = postedFile.FileName;
                        string date = DateTime.Now.ToString();
                        date = PdftkTools.CleanDate(date);
                        fileName = date + fileName;
                        filesNames.Add(fileName);

                        var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);
                        postedFile.SaveAs(filePath);
                    }

                    List<string> outputFilesNames = new List<string>();
                    filesNames.ForEach(delegate (string fileName)
                    {
                        outputFilesNames.Add(PdftkTools.UncompressFile(fileName));
                    });

                    response = Request.CreateResponse(HttpStatusCode.OK, outputFilesNames);

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
