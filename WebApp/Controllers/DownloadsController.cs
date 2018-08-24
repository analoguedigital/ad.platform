using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class DownloadsController : ApiController
    {

        #region Properties

        public UnitOfWork UnitOfWork
        {
            get { return ServiceContext.UnitOfWork; }
        }

        public User CurrentUser
        {
            get { return ServiceContext.CurrentUser; }
        }

        #endregion

        // GET api/downloads/{id}
        [AllowAnonymous]
        [Route("api/downloads/{id}")]
        public HttpResponseMessage Get(string id)
        {
            var attachmentId = OneTimeAccessService.GetFileIdForTicket(id);
            if (attachmentId != Guid.Empty)
            {
                var attachment = UnitOfWork.AttachmentsRepository.Find(attachmentId);
                var filePath = HostingEnvironment.MapPath(attachment.Url);
                var fileContent = System.IO.File.ReadAllBytes(filePath);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new System.IO.FileStream(filePath, FileMode.Open, FileAccess.Read);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition.FileName = attachment.FileName;

                return response;
            }

            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        // GET api/downloads/file/{id}
        [Authorize]
        [Route("api/downloads/file/{id:guid}")]
        public IHttpActionResult GetFile(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var attachment = UnitOfWork.AttachmentsRepository.Find(id);
            if (attachment == null)
                return NotFound();

            var filePath = HostingEnvironment.MapPath(attachment.Url);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var oneTimeAccessId = string.Empty;
            var currentUser = ServiceContext.CurrentUser;
            var orgUser = ServiceContext.CurrentUser as OrgUser;

            if (currentUser is SuperUser)
                oneTimeAccessId = OneTimeAccessService.AddFileIdForTicket(id);
            else
            {
                var project = attachment.FormValue.FilledForm.Project;
                var template = attachment.FormValue.FilledForm.FormTemplate;

                var threadAssignment = UnitOfWork.ThreadAssignmentsRepository.AllAsNoTracking
                    .Where(x => x.FormTemplateId == template.Id && x.OrgUserId == CurrentUser.Id)
                    .SingleOrDefault();
                var projectAssignment = UnitOfWork.AssignmentsRepository.AllAsNoTracking
                    .Where(x => x.ProjectId == project.Id && x.OrgUserId == CurrentUser.Id)
                    .SingleOrDefault();

                if (projectAssignment == null && threadAssignment == null)
                    return Unauthorized();

                var authorized = false;
                if (projectAssignment != null)
                    authorized = projectAssignment.CanView;

                if (threadAssignment != null)
                    authorized = threadAssignment.CanView;

                if (!authorized)
                    return Unauthorized();

                oneTimeAccessId = OneTimeAccessService.AddFileIdForTicket(id);
            }

            var result = new DownloadRequestDTO { AccessId = oneTimeAccessId };

            return Ok(result);
        }
    }
}
