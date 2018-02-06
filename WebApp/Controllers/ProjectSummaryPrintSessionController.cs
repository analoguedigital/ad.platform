using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Hosting;
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
        [Route("api/ProjectSummaryPrintSession/DownloadZip/{id:guid}/{timeline:bool}/{locations:bool}/{piechart:bool}")]
        public HttpResponseMessage DownloadZip(Guid id, bool timeline, bool locations, bool piechart)
        {
            var session = ProjectSummaryPrintSessionService.GetSession(id);
            if (session == null)
                return null;

            return ExportZipFile(session, id, timeline, locations, piechart);
        }

        private string GetRootIndexPath()
        {
            var rootIndexPath = ConfigurationManager.AppSettings["RootIndexPath"];
            if (!string.IsNullOrEmpty(rootIndexPath))
                return rootIndexPath;

            return "wwwroot/index.html";
        }

        [HttpGet]
        [Route("api/ProjectSummaryPrintSession/DownloadPdf/{id:guid}/{timeline:bool}/{locations:bool}/{piechart:bool}")]
        public HttpResponseMessage DownloadPdf(Guid id, bool timeline, bool locations, bool piechart)
        {
            var session = ProjectSummaryPrintSessionService.GetSession(id);
            if (session == null)
                return null;

            var rootIndex = GetRootIndexPath();
            var authData = $"{{\"token\":\"{HttpContext.Current.Request.Headers["Authorization"].Substring(7)}\",\"email\":\"{CurrentUser.Email}\"}}";
            var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}?authData={authData}#!/projects/summary/print/{id.ToString()}?timeline={timeline}&locations={locations}&piechart={piechart}";

            var projectName = UnitOfWork.ProjectsRepository.Find(session.ProjectId).Name;
            var pdfFileName = $"{projectName}.pdf";

            var pdfData = ConvertHtmlToPdf(url);
            return CreatePdfResponseMessage(pdfData, projectName);
        }

        private HttpResponseMessage ExportZipFile(ProjectSummaryPrintSessionDTO session, Guid id, bool timeline, bool locations, bool piechart)
        {
            var rootIndex = GetRootIndexPath();
            var authData = $"{{\"token\":\"{HttpContext.Current.Request.Headers["Authorization"].Substring(7)}\",\"email\":\"{CurrentUser.Email}\"}}";
            var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}?authData={authData}#!/projects/summary/print/{id.ToString()}?timeline={timeline}&locations={locations}&piechart={piechart}";

            var surveys = this.UnitOfWork.FilledFormsRepository.AllAsNoTracking
                .Where(s => session.SurveyIds.Contains(s.Id)).ToList();

            var rootFolder = HostingEnvironment.MapPath("/ExportTemp");
            if (Directory.Exists(rootFolder))
                Directory.Delete(rootFolder, true);
            Directory.CreateDirectory(rootFolder);

            foreach (var survey in surveys)
            {
                var attachments = this.UnitOfWork.AttachmentsRepository.AllAsNoTracking
                    .Where(a => a.FormValue.FilledFormId == survey.Id).ToList();

                foreach (var attch in attachments)
                {
                    string folderPath = string.Empty;
                    if (!string.IsNullOrEmpty(survey.Description))
                    {
                        var sanitizedFileName = SanitizeFileName(survey.FormTemplate.Title);
                        folderPath = HostingEnvironment.MapPath($"/ExportTemp/{survey.Serial}_{sanitizedFileName}");
                    }
                    else
                        folderPath = HostingEnvironment.MapPath($"/ExportTemp/{survey.Serial}");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    try
                    {
                        var filePath = HostingEnvironment.MapPath(attch.Url);
                        var fileInfo = new FileInfo(filePath);
                        var dest = Path.Combine(folderPath, attch.FileName);
                        fileInfo.CopyTo(dest);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }

            var projectName = UnitOfWork.ProjectsRepository.Find(session.ProjectId).Name;
            var pdfFileName = $"{projectName}.pdf";

            var pdfData = ConvertHtmlToPdf(url);
            var pdfFilePath = Path.Combine(rootFolder, pdfFileName);
            var fs = new FileStream(pdfFilePath, FileMode.Create);
            fs.Write(pdfData, 0, pdfData.Length);
            fs.Flush();
            fs.Close();

            var zipRootPath = HostingEnvironment.MapPath("/ZipTemp");
            if (Directory.Exists(zipRootPath))
                Directory.Delete(zipRootPath, true);
            Directory.CreateDirectory(zipRootPath);

            var zipFilename = $"{projectName}.zip";
            var zipFilePath = Path.Combine(zipRootPath, zipFilename);
            ZipFile.CreateFromDirectory(rootFolder, zipFilePath);

            var zipData = File.ReadAllBytes(zipFilePath);
            var response = CreateZipResponseMessage(zipData, zipFilename);

            Directory.Delete(rootFolder, true);
            Directory.Delete(zipRootPath, true);

            return response;
        }

        private byte[] ConvertHtmlToPdf(string url)
        {
            var htmlToPdfConverter = new Winnovative.HtmlToPdfConverter();
            htmlToPdfConverter.ConversionDelay = 3;
            htmlToPdfConverter.RenderedHtmlElementSelector = ".content";
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = Winnovative.PdfPageSize.A4;
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
            result.Content.Headers.ContentLength = data.Length;

            return result;
        }

        private HttpResponseMessage CreateZipResponseMessage(byte[] data, string fileName)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(data);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = fileName;
            result.Content.Headers.Add("x-filename", fileName);

            return result;
        }

        private string SanitizeFileName(string filename)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(filename, invalidRegStr, "_");
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
