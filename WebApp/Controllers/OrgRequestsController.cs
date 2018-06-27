using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Http;

namespace WebApi.Controllers
{
    [RoutePrefix("api/orgRequests")]
    public class OrgRequestsController : BaseApiController
    {
        public IHttpActionResult Get()
        {
            var orgRequests = UnitOfWork.OrgRequestsRepository
                .AllAsNoTracking
                .OrderByDescending(x => x.DateCreated);

            var result = orgRequests
                .ToList()
                .Select(x => Mapper.Map<OrgRequestDTO>(x))
                .ToList();

            return Ok(result);
        }

        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var orgRequest = UnitOfWork.OrgRequestsRepository.Find(id);
            if (orgRequest == null)
                return NotFound();

            return Ok(Mapper.Map<OrgRequestDTO>(orgRequest));
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody]OrgRequestDTO model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return BadRequest("Organisation name is required");

            if (this.CurrentUser is SuperUser)
                return BadRequest("Organisation requests can only be made by Organisation Users");

            if (this.CurrentOrgUser.AccountType != AccountType.MobileAccount)
                return BadRequest("Organisation requests can only be made by Mobile Users");

            var existingOrg = UnitOfWork.OrganisationRepository.AllAsNoTracking
                .Where(x => x.Name == model.Name)
                .FirstOrDefault();

            if (existingOrg != null)
                return BadRequest("An organisation with this name already exists!");

            if (UnitOfWork.OrgRequestsRepository.AllAsNoTracking
                .Count(x => x.OrgUserId == this.CurrentOrgUser.Id && x.Name == model.Name) > 0)
            {
                return BadRequest("You have already requested this organisation");
            }

            try
            {
                var orgRequest = new OrgRequest
                {
                    Name = model.Name,
                    Address = model.Address,
                    ContactName = model.ContactName,
                    Email = model.Email,
                    TelNumber = model.TelNumber,
                    Postcode = model.Postcode,
                    OrgUserId = this.CurrentOrgUser.Id
                };

                UnitOfWork.OrgRequestsRepository.InsertOrUpdate(orgRequest);

                var onRecord = UnitOfWork.OrganisationRepository.AllAsNoTracking
                    .Where(x => x.Name == "OnRecord").SingleOrDefault();

                if (onRecord.RootUserId.HasValue)
                {
                    var onRecordAdmin = UnitOfWork.OrgUsersRepository.AllAsNoTracking
                        .Where(x => x.Id == onRecord.RootUserId.Value)
                        .SingleOrDefault();

                    var email = new Email
                    {
                        To = onRecordAdmin.Email,
                        Subject = $"A user has requested an organization",
                        Content = GenerateOnRecordOrgRequestEmail(model, this.CurrentOrgUser)
                    };

                    UnitOfWork.EmailsRepository.InsertOrUpdate(email);
                }

                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                UnitOfWork.OrgRequestsRepository.Delete(id);
                UnitOfWork.OrgRequestsRepository.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GenerateOnRecordOrgRequestEmail(OrgRequestDTO model, OrgUser orgUser)
        {
            var path = HostingEnvironment.MapPath("~/EmailTemplates/org-connection-request.html");
            var emailTemplate = System.IO.File.ReadAllText(path, Encoding.UTF8);

            var messageHeaderKey = "{{MESSAGE_HEADING}}";
            var messageBodyKey = "{{MESSAGE_BODY}}";

            var rootIndex = GetRootIndexPath();
            var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}#!/organisations/requests/";

            var content = @"<p>User name: <b>" + orgUser.UserName + @"</b></p>
                            <p>Organisation: <b>" + model.Name + @"</b></p>
                            <p><br></p>
                            <p>View <a href='" + url + @"'>organization requests</a> on the dashboard.</p>";

            emailTemplate = emailTemplate.Replace(messageHeaderKey, "A User Has Requested an Organization");
            emailTemplate = emailTemplate.Replace(messageBodyKey, content);

            return emailTemplate;
        }

        private string GetRootIndexPath()
        {
            var rootIndexPath = ConfigurationManager.AppSettings["RootIndexPath"];
            if (!string.IsNullOrEmpty(rootIndexPath))
                return rootIndexPath;

            return "wwwroot/index.html";
        }

    }
}
