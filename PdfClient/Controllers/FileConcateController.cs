using Newtonsoft.Json;
using Pdf.Enumerations;
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
    [Route("api/[controller]")]
    public class FileConcateController : ApiController
    {

        [Route("PostFilesForConcateDocs")]
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

                    var lastFileInRequest = filesNames.Last();
                    while (!File.Exists(HttpContext.Current.Server.MapPath("~/Uploads/" + lastFileInRequest)))
                    {
                        Thread.Sleep(1000);
                    }

                    string outputName = PdftkTools.ConcateFiles(filesNames);

                    response = Request.CreateResponse(HttpStatusCode.OK, outputName);
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

        [Route("PostFilesForConcatePages")]
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

                    string outputName = PdftkTools.ConcatePages(fileName, pages);

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


        //TODO - MOVE THIS TO OTHER CLASS
        //[Route("GetFile/{fileName}")]
        //public HttpResponseMessage GetFileConverted(string fileName)
        //{
        //    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);

        //    if (String.IsNullOrEmpty(fileName))
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);

        //    string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/" + fileName);
        //    byte[] pdf = File.ReadAllBytes(filePath);

        //    response.Content = new ByteArrayContent(pdf);
        //    response.Content.Headers.ContentLength = pdf.Length;
        //    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
        //    response.Content.Headers.ContentDisposition.FileName = fileName;
        //    response.StatusCode = HttpStatusCode.OK;
        //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        //    return response;
        //}

        [Route("GetFile/{fileName}")]
        public HttpResponseMessage Get(string fileName)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/" + fileName);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            return result;
        }

    }
}