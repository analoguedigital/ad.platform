using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class ProjectSummaryPrintSessionController : BaseApiController
    {
        public IHttpActionResult Get(Guid id)
        {
            var session = ProjectSummaryPrintSessionService.GetSession(id);

            if (session == null)
                return NotFound();

            return Ok(session);
        }

        public IHttpActionResult Post([FromBody] ProjectSummaryPrintSessionDTO model)
        {
            var updatedModel = ProjectSummaryPrintSessionService.AddOrUpdateSession(model);
            return Ok(updatedModel);
        }

        [HttpGet]
        [Route("api/ProjectSummaryPrintSession/Download/{id:Guid}")]
        public HttpResponseMessage Download(Guid id)
        {
            var session = ProjectSummaryPrintSessionService.GetSession(id);

            if (session == null)
                return null;

            var authData = $"{{\"token\":\"{HttpContext.Current.Request.Headers["Authorization"].Substring(7)}\",\"email\":\"{CurrentUser.Email}\"}}";
            var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/wwwroot/index.html?authData={authData}#!/projects/summary/print/{id.ToString()}";

            var filename = UnitOfWork.ProjectsRepository.Find(session.ProjectId).Name + ".pdf";
            return CreatePdfResponseMessage(ConvertHtmlToPdf(url), filename);
        }

        private byte[] ConvertHtmlToPdf(string url)
        {
            var htmlToPdfConverter = new Winnovative.HtmlToPdfConverter();
            htmlToPdfConverter.ConversionDelay = 3;
            htmlToPdfConverter.MediaType = "print";
            htmlToPdfConverter.PdfDocumentOptions.TopMargin = 60;
            htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 60;
            htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 30;
            htmlToPdfConverter.PdfDocumentOptions.RightMargin = 30;
            return htmlToPdfConverter.ConvertUrl(url);
        }

        private HttpResponseMessage CreatePdfResponseMessage(byte[] data, string fileName)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(data);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = fileName;
            result.Content.Headers.Add("x-filename", fileName);
            return result;
        }
    }

    public class ProjectSummaryPrintSessionService
    {
        private static Dictionary<Guid, ProjectSummaryPrintSessionDTO> Storage = new Dictionary<Guid, ProjectSummaryPrintSessionDTO>();

        public static ProjectSummaryPrintSessionDTO AddOrUpdateSession(ProjectSummaryPrintSessionDTO session)
        {
            if (session.Id == Guid.Empty)
                session.Id = Guid.NewGuid();

            Storage[session.Id] = session;
            return session;
        }

        public static ProjectSummaryPrintSessionDTO UpdateSession(Guid id, ProjectSummaryPrintSessionDTO session)
        {
            Storage[id] = session;
            return session;
        }

        public static ProjectSummaryPrintSessionDTO GetSession(Guid id)
        {
            if (Storage.ContainsKey(id))
                return Storage[id];

            return null;
        }
    }
}
