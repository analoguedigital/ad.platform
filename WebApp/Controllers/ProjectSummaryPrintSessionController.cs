using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using WebApi.Filters;
using WebApi.Models;
using WebApi.Services;
using static LightMethods.Survey.Models.Entities.User;

namespace WebApi.Controllers
{
    [NeedsActiveSubscription]
    public class ProjectSummaryPrintSessionController : BaseApiController
    {
        // GET api/projectSummaryPrintSession/{id}
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var session = ProjectSummaryPrintSessionService.GetSession(id);
            if (session == null)
                return NotFound();

            return Ok(session);
        }

        // POST api/projectSummaryPrintSession
        public IHttpActionResult Post([FromBody] ProjectSummaryPrintSessionDTO model)
        {
            var updatedModel = ProjectSummaryPrintSessionService.AddOrUpdateSession(model);
            return Ok(updatedModel);
        }

        // GET api/projectSummaryPrintSession/downloadZip/{id}/{timeline}/{locations}/{piechart}
        [HttpGet]
        [Route("api/ProjectSummaryPrintSession/DownloadZip/{id:guid}/{timeline:bool}/{locations:bool}/{piechart:bool}/{latitude:double}/{longitude:double}/{zoomLevel:int}/{mapType}")]
        public HttpResponseMessage DownloadZip(Guid id, bool timeline, bool locations, bool piechart, double latitude, double longitude, int zoomLevel, string mapType)
        {
            if (id == Guid.Empty)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            var session = ProjectSummaryPrintSessionService.GetSession(id);
            if (session == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            if (CurrentUser is OrgUser)
            {
                if (CurrentOrgUser.Type == OrgUserTypesRepository.Administrator)
                {
                    var project = UnitOfWork.ProjectsRepository.Find(session.ProjectId);
                    if (CurrentOrgUser.OrganisationId != project.OrganisationId)
                        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
                else
                {
                    var subscriptionService = new SubscriptionService(UnitOfWork);
                    if (!subscriptionService.HasAccessToExportZip(CurrentOrgUser, session.ProjectId))
                        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }

            return ExportZipFile(session, id, timeline, locations, piechart, latitude, longitude, zoomLevel, mapType);
        }

        // GET api/projectSummaryPrintSession/downloadPdf/{id}/{timeline}/{locations}/{piechart}
        [HttpGet]
        [Route("api/ProjectSummaryPrintSession/DownloadPdf/{id:guid}/{timeline:bool}/{locations:bool}/{piechart:bool}/{latitude:double}/{longitude:double}/{zoomLevel:int}/{mapType}")]
        public HttpResponseMessage DownloadPdf(Guid id, bool timeline, bool locations, bool piechart, double latitude, double longitude, int zoomLevel, string mapType)
        {
            if (id == Guid.Empty)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            var session = ProjectSummaryPrintSessionService.GetSession(id);
            if (session == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            if (CurrentUser is OrgUser)
            {
                if (CurrentOrgUser.Type == OrgUserTypesRepository.Administrator)
                {
                    var project = UnitOfWork.ProjectsRepository.Find(session.ProjectId);
                    if (CurrentOrgUser.OrganisationId != project.OrganisationId)
                        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
                else
                {
                    var subscriptionService = new SubscriptionService(UnitOfWork);
                    if (!subscriptionService.HasAccessToExportPdf(CurrentOrgUser, session.ProjectId))
                        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }

            var rootIndex = WebHelpers.GetRootIndexPath();
            var authData = $"{{\"token\":\"{HttpContext.Current.Request.Headers["Authorization"].Substring(7)}\",\"email\":\"{CurrentUser.Email}\"}}";
            var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}?authData={authData}#!/projects/summary/print/{id.ToString()}?timeline={timeline}&locations={locations}&piechart={piechart}&lat={latitude}&lng={longitude}&zoomLevel={zoomLevel}&mapType={mapType}";

            var projectName = UnitOfWork.ProjectsRepository.Find(session.ProjectId).Name;
            var pdfFileName = $"{projectName}.pdf";
            var pdfData = ConvertHtmlToPdf(url);

            return CreatePdfResponseMessage(pdfData, projectName);
        }

        #region helpers

        private HttpResponseMessage ExportZipFile(ProjectSummaryPrintSessionDTO session, Guid id, bool timeline, bool locations, bool piechart, double latitude, double longitude, int zoomLevel, string mapType)
        {
            //if (CurrentOrgUser != null)
            //{
            //    var project = UnitOfWork.ProjectsRepository.Find(session.ProjectId);
            //    var assignment = project.Assignments.SingleOrDefault(a => a.OrgUserId == CurrentOrgUser.Id);
            //    if (assignment == null || !assignment.CanExportZip)
            //        return new HttpResponseMessage(HttpStatusCode.Forbidden);
            //}

            var rootIndex = WebHelpers.GetRootIndexPath();
            var authData = $"{{\"token\":\"{HttpContext.Current.Request.Headers["Authorization"].Substring(7)}\",\"email\":\"{CurrentUser.Email}\"}}";
            var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}?authData={authData}#!/projects/summary/print/{id.ToString()}?timeline={timeline}&locations={locations}&piechart={piechart}&lat={latitude}&lng={longitude}&zoomLevel={zoomLevel}&mapType={mapType}";

            var surveys = UnitOfWork.FilledFormsRepository.AllAsNoTracking
                .Where(s => session.SurveyIds.Contains(s.Id)).ToList();

            var rootFolder = HostingEnvironment.MapPath("/ExportTemp");
            if (Directory.Exists(rootFolder))
                Directory.Delete(rootFolder, true);
            Directory.CreateDirectory(rootFolder);

            foreach (var survey in surveys)
            {
                var attachments = UnitOfWork.AttachmentsRepository.AllAsNoTracking
                    .Where(a => a.FormValue.FilledFormId == survey.Id).ToList();

                foreach (var attch in attachments)
                {
                    string folderPath = string.Empty;
                    if (!string.IsNullOrEmpty(survey.Description))
                    {
                        var sanitizedFileName = WebHelpers.SanitizeFileName(survey.FormTemplate.Title);
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

            var zipData = System.IO.File.ReadAllBytes(zipFilePath);
            var response = CreateZipResponseMessage(zipData, zipFilename);

            Directory.Delete(rootFolder, true);
            Directory.Delete(zipRootPath, true);

            return response;
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

        private byte[] ConvertHtmlToPdf(string url)
        {
            var htmlToPdfConverter = new Winnovative.HtmlToPdfConverter();
            htmlToPdfConverter.TriggeringMode = Winnovative.TriggeringMode.ConversionDelay;
            htmlToPdfConverter.ConversionDelay = 5;
            htmlToPdfConverter.RenderedHtmlElementSelector = ".content";
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = Winnovative.PdfPageSize.A4;
            htmlToPdfConverter.DownloadAllResources = true;
            htmlToPdfConverter.EnableAccelerated2DCanvas = true;
            htmlToPdfConverter.EnablePersistentStorage = true;
            htmlToPdfConverter.PdfDocumentOptions.JpegCompressionEnabled = false;
            htmlToPdfConverter.PdfDocumentOptions.PdfCompressionLevel = Winnovative.PdfCompressionLevel.NoCompression;
            //htmlToPdfConverter.PdfDocumentOptions.TopMargin = 60;
            //htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 60;
            //htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 30;
            //htmlToPdfConverter.PdfDocumentOptions.RightMargin = 30;
            htmlToPdfConverter.PdfDocumentOptions.TopMargin = 20;
            htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 20;
            htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 20;
            htmlToPdfConverter.PdfDocumentOptions.RightMargin = 20;
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

        #endregion

        #region Download Account Data

        private IHttpActionResult DownloadAccountData()
        {
            var rootFolder = HostingEnvironment.MapPath("/ExportTemp");
            if (Directory.Exists(rootFolder))
                Directory.Delete(rootFolder, true);
            Directory.CreateDirectory(rootFolder);

            // generate JSON data
            WriteProfileJson(CurrentOrgUser, rootFolder);
            WriteThreadsJson(CurrentOrgUser, rootFolder);
            WriteSurveysJson(CurrentOrgUser, rootFolder);

            // extract attachments
            var project = UnitOfWork.ProjectsRepository.Find(CurrentOrgUser.CurrentProject.Id);
            ExtractAttachments(project.Id);

            // create the zip archive
            var zipRootPath = HostingEnvironment.MapPath("/ZipTemp");
            if (Directory.Exists(zipRootPath))
                Directory.Delete(zipRootPath, true);
            Directory.CreateDirectory(zipRootPath);

            var zipFilename = $"{project.Name}.zip";
            var zipFilePath = Path.Combine(zipRootPath, zipFilename);

            using (var zip = new Ionic.Zip.ZipFile())
            {
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                zip.CompressionMethod = Ionic.Zip.CompressionMethod.Deflate;
                zip.MaxOutputSegmentSize = 50 * 1024 * 1024;    // 50mb each.

                zip.AddDirectory(rootFolder);
                zip.Save(zipFilePath);
            }

            var accountDataExportPath = HostingEnvironment.MapPath("/_AccountDataExport");
            if (Directory.Exists(accountDataExportPath))
                Directory.Delete(accountDataExportPath, true);
            Directory.CreateDirectory(accountDataExportPath);

            var userFolderName = $"{CurrentOrgUser.Id}";
            var downloadFolderPath = HostingEnvironment.MapPath("/_AccountDataExport/" + userFolderName);
            if (Directory.Exists(downloadFolderPath))
                Directory.Delete(downloadFolderPath, true);
            Directory.CreateDirectory(downloadFolderPath);

            Dictionary<string, string> downloadLinks = new Dictionary<string, string>();
            var appUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}";

            var zipFiles = Directory.GetFiles(zipRootPath);
            foreach (var file in zipFiles)
            {
                var fi = new FileInfo(file);
                System.IO.File.Copy(file, Path.Combine(downloadFolderPath, fi.Name));

                var downloadLink = string.Format("{0}/_AccountDataExport/{1}/{2}", appUrl, userFolderName, fi.Name);
                downloadLinks.Add(downloadLink, fi.Name);
            }

            Directory.Delete(rootFolder, true);
            Directory.Delete(zipRootPath, true);

            // send the email containing download links
            GenerateAccountDataEmail(downloadLinks);

            return Ok();
        }

        private void ExtractAttachments(Guid projectId)
        {
            var surveys = UnitOfWork.FilledFormsRepository
                .AllAsNoTracking
                .Where(x => x.ProjectId == projectId && x.FormTemplate.Discriminator == FormTemplateDiscriminators.RegularThread)
                .ToList();

            foreach (var survey in surveys)
            {
                var attachments = UnitOfWork.AttachmentsRepository
                    .AllAsNoTracking
                    .Where(a => a.FormValue.FilledFormId == survey.Id)
                    .ToList();

                foreach (var attch in attachments)
                {
                    string folderPath = string.Empty;
                    if (!string.IsNullOrEmpty(survey.Description))
                    {
                        var sanitizedFileName = WebHelpers.SanitizeFileName(survey.FormTemplate.Title);
                        folderPath = HostingEnvironment.MapPath($"/ExportTemp/attachments/{survey.Serial}_{sanitizedFileName}");
                    }
                    else
                        folderPath = HostingEnvironment.MapPath($"/ExportTemp/attachments/{survey.Serial}");

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
        }

        private void WriteProfileJson(OrgUser user, string filePath)
        {
            var profileData = new JsonProfileData
            {
                Id = user.Id,
                EmailAddress = user.Email,
                JoinedOn = user.RegistrationDate,
                LastLogin = user.LastLogin,

                FirstName = user.FirstName,
                Surname = user.Surname,
                Gender = user.Gender,
                Birthdate = user.Birthdate,
                Address = user.Address,

                IsWebUser = user.IsWebUser,
                IsMobileUser = user.IsMobileUser,
                PhoneNumber = user.PhoneNumber,

                Organisation = user.Organisation.Name,

                Project = new JsonCurrentProjectData
                {
                    Id = user.CurrentProject.Id,
                    Number = user.CurrentProject.Number,
                    Name = user.CurrentProject.Name,
                    StartDate = user.CurrentProject.StartDate,
                    EndDate = user.CurrentProject.EndDate,
                    Notes = user.CurrentProject.Notes,
                    CreatedBy = user.CurrentProject.CreatedBy.Email
                }
            };

            var profileDataFilePath = Path.Combine(filePath, "profile.json");
            using (StreamWriter jsonFile = System.IO.File.CreateText(profileDataFilePath))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                };

                serializer.Serialize(jsonFile, profileData);

                jsonFile.Flush();
                jsonFile.Close();
            }
        }

        private void WriteThreadsJson(OrgUser user, string filePath)
        {
            var projectId = user.CurrentProjectId;
            var threads = UnitOfWork.FormTemplatesRepository
                .AllAsNoTracking
                .Where(x => x.ProjectId == projectId && x.Discriminator == FormTemplateDiscriminators.RegularThread)
                .ToList();
            var adviceThreads = UnitOfWork.FormTemplatesRepository
                .AllAsNoTracking
                .Where(x => x.ProjectId == projectId && x.Discriminator == FormTemplateDiscriminators.AdviceThread)
                .ToList();

            List<JsonThreadData> threadsData = new List<JsonThreadData>();
            foreach (var thread in threads)
            {
                threadsData.Add(new JsonThreadData
                {
                    Id = thread.Id,
                    Code = thread.Code,
                    Title = thread.Title,
                    Version = thread.Version,
                    Description = thread.Description,
                    IsPublished = thread.IsPublished,
                    Colour = thread.Colour,
                    Surveys = UnitOfWork.FilledFormsRepository.AllAsNoTracking.Count(x => x.FormTemplateId == thread.Id)
                });
            }

            List<JsonThreadData> adviceThreadsData = new List<JsonThreadData>();
            foreach (var adviceThread in adviceThreads)
            {
                adviceThreadsData.Add(new JsonThreadData
                {
                    Id = adviceThread.Id,
                    Code = adviceThread.Code,
                    Title = adviceThread.Title,
                    Version = adviceThread.Version,
                    Description = adviceThread.Description,
                    IsPublished = adviceThread.IsPublished,
                    Colour = adviceThread.Colour,
                    Surveys = UnitOfWork.FilledFormsRepository.AllAsNoTracking.Count(x => x.FormTemplateId == adviceThread.Id)
                });
            }

            var result = new JsonThreadsProfile
            {
                Threads = threadsData,
                AdviceThreads = adviceThreadsData
            };

            var outputPath = Path.Combine(filePath, "threads.json");
            using (StreamWriter jsonFile = System.IO.File.CreateText(outputPath))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                };

                serializer.Serialize(jsonFile, result);

                jsonFile.Flush();
                jsonFile.Close();
            }
        }

        private void WriteSurveysJson(OrgUser user, string filePath)
        {
            var projectId = user.CurrentProjectId;
            var surveys = UnitOfWork.FilledFormsRepository
                .AllAsNoTracking
                .Where(x => x.ProjectId == projectId && x.FormTemplate.Discriminator == FormTemplateDiscriminators.RegularThread)
                .GroupBy(x => x.FormTemplate)
                .ToList();

            List<JsonSurveyNode> result = new List<JsonSurveyNode>();

            foreach (var entry in surveys)
            {
                var node = new JsonSurveyNode
                {
                    Id = entry.Key.Id,
                    Title = entry.Key.Title
                };

                foreach (var record in entry.ToList())
                {
                    var survey = new JsonSurveyData
                    {
                        Id = record.Id,
                        Serial = record.Serial,
                        DateCreated = record.DateCreated,
                        DateUpdated = record.DateUpdated,
                        SurveyDate = record.SurveyDate,
                        FilledBy = record.FilledBy.Email,
                        IsRead = record.IsRead,
                        Description = record.Description
                    };

                    foreach (var loc in record.Locations)
                    {
                        survey.Locations.Add(new JsonSurveyLocationData
                        {
                            Latitude = loc.Latitude,
                            Longitude = loc.Longitude,
                            Address = loc.Address
                        });
                    }

                    node.Surveys.Add(survey);
                }

                result.Add(node);
            }

            var outputPath = Path.Combine(filePath, "surveys.json");
            using (StreamWriter jsonFile = System.IO.File.CreateText(outputPath))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                };

                serializer.Serialize(jsonFile, result);

                jsonFile.Flush();
                jsonFile.Close();
            }
        }

        private void GenerateAccountDataEmail(Dictionary<string, string> downloadLinks)
        {
            var emailBody = $@"
                <p>
                    You have requested to download your account data.<br>
                    Below are links to your download files:
                </p>
                <ul>";

            foreach (var link in downloadLinks)
                emailBody += $"<li><a href='{link.Key}'>{link.Value}</a></li>";

            emailBody += $@"</ul>
                <p>
                    Your data is available for the next 72 hours.<br>
                    Make sure to keep your data safe and secure.
                </p>
                <br>
                <p>
                    Best regards<br>
                    The OnRecord Team
                </p>";

            var email = new Email
            {
                To = CurrentOrgUser.Email,
                Subject = "Download your account data",
                Content = WebHelpers.GenerateEmailTemplate(emailBody, "Download Account Data")
            };

            UnitOfWork.EmailsRepository.InsertOrUpdate(email);
            UnitOfWork.Save();
        }

        #endregion Download Account Data

    }

    // JSON data objects
    public class JsonProfileData
    {
        public Guid Id { get; set; }

        public string EmailAddress { get; set; }

        public DateTime? JoinedOn { get; set; }

        public DateTime? LastLogin { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public GenderType? Gender { get; set; }

        public DateTime? Birthdate { get; set; }

        public string Address { get; set; }

        public bool IsWebUser { get; set; }

        public bool IsMobileUser { get; set; }

        public string PhoneNumber { get; set; }

        public string Organisation { get; set; }

        public JsonCurrentProjectData Project { get; set; }
    }

    public class JsonCurrentProjectData
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Notes { get; set; }

        public string CreatedBy { get; set; }
    }

    public class JsonThreadData
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Title { get; set; }

        public double Version { get; set; }

        public string Description { get; set; }

        public bool IsPublished { get; set; }

        public string Colour { get; set; }

        public int Surveys { get; set; }
    }

    public class JsonThreadsProfile
    {
        public List<JsonThreadData> Threads { get; set; }

        public List<JsonThreadData> AdviceThreads { get; set; }
    }

    public class JsonSurveyData
    {
        public Guid Id { get; set; }

        public int Serial { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime SurveyDate { get; set; }

        public string FilledBy { get; set; }

        public bool? IsRead { get; set; }

        public string Description { get; set; }

        public string SerialReferences { get; set; }

        public List<JsonSurveyLocationData> Locations { get; set; }

        public JsonSurveyData()
        {
            this.Locations = new List<JsonSurveyLocationData>();
        }
    }

    public class JsonSurveyLocationData
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Address { get; set; }
    }

    public class JsonSurveyNode
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public List<JsonSurveyData> Surveys { get; set; }

        public JsonSurveyNode()
        {
            this.Surveys = new List<JsonSurveyData>();
        }
    }

}
