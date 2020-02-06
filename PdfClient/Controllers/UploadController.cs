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
        public async Task<string> Post()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var fileName = postedFile.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault();
                        var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName + DateTime.Now );
                        postedFile.SaveAs(filePath);

                    }

                    var fileName1 = httpRequest.Files[0].FileName;
                    var fileName2 = httpRequest.Files[1].FileName;

                    string outputName = ConvertFileTools.ConcateFiles(fileName1, fileName2);
                    return outputName;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "nofiles";

        }

        [HttpGet]
        public HttpResponseMessage GetFilesConverted(string fileName)
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