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
    public class RemovePagesController : ApiController
    {

        [Route("RemovePages")]
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

                    string outputName = PdftkTools.RemovePages(fileName, pages);
                    response = Request.CreateResponse(HttpStatusCode.OK, outputName);
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
