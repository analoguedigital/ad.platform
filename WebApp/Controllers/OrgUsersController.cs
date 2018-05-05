using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;

namespace WebApi.Controllers
{
    public class OrgUsersController : BaseApiController
    {
        private OrgUsersRepository Users
        {
            get { return UnitOfWork.OrgUsersRepository; }
        }

        private OrganisationRepository Organisations
        {
            get { return UnitOfWork.OrganisationRepository; }
        }

        private OrgUserTypesRepository Types
        {
            get { return UnitOfWork.OrgUserTypesRepository; }
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set { _userManager = value; }
        }

        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrgUserDTO>))]
        public IHttpActionResult Get(Guid? organisationId = null)
        {
            var result = new List<OrgUserDTO>();

            if (this.CurrentOrgUser != null)
            {
                var users = Users.AllIncluding(u => u.Type)
                    .Where(u => u.OrganisationId == CurrentOrganisationId)
                    .OrderBy(u => u.Surname)
                    .ThenBy(u => u.FirstName)
                    .ToList()
                    .Select(u => Mapper.Map<OrgUserDTO>(u)).ToList();
                result = users;
            }
            else
            {
                if (organisationId.HasValue)
                {
                    var users = Users.AllIncluding(u => u.Type)
                        .Where(u => u.OrganisationId == organisationId)
                        .OrderBy(u => u.Surname)
                        .ThenBy(u => u.FirstName)
                        .ToList()
                        .Select(u => Mapper.Map<OrgUserDTO>(u)).ToList();
                    result = users;
                }
                else
                {
                    var users = Users.AllIncluding(u => u.Type)
                      .OrderBy(u => u.Surname)
                      .ThenBy(u => u.FirstName)
                      .ToList()
                      .Select(u => Mapper.Map<OrgUserDTO>(u)).ToList();
                    result = users;
                }
            }

            return Ok(result);
        }

        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrgUserDTO>))]
        public IHttpActionResult Get(Guid id)
        {
            var user = Users.Find(id);

            var orgUser = user as OrgUser;
            var result = Mapper.Map<OrgUserDTO>(user);

            if (orgUser != null)
            {
                var assignments = orgUser.Assignments.Select(a => Mapper.Map<ProjectAssignmentDTO>(a)).ToList();
                result.Assignments = assignments;
            }

            return Ok(result);
        }

        public async Task<IHttpActionResult> Post([FromBody]OrgUserDTO value)
        {
            if (value.Password.IsEmpty())
                ModelState.AddModelError("Password", "Please provide password.");

            if (value.Password != value.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "'Password' and 'Confirm password' must be the same.");

            var orguser = Mapper.Map<OrgUser>(value);
            orguser.UserName = orguser.Email;
            orguser.OrganisationId = CurrentOrgUser.OrganisationId.Value;
            orguser.AccountType = AccountType.WebAccount;

            var identityResult = ServiceContext.UserManager.CreateSync(orguser, value.Password);

            if (!identityResult.Succeeded)
                throw new Exception(identityResult.Errors.ToString(". "));

            orguser.Type = UnitOfWork.OrgUserTypesRepository.Find(orguser.TypeId);
            UnitOfWork.UserManager.AssignRolesByUserType(orguser);

            if (value.Type.Name.ToLower() == "administrator")
            {
                var projects = UnitOfWork.ProjectsRepository.AllAsNoTracking
                    .Where(p => p.OrganisationId == this.CurrentOrgUser.OrganisationId);

                foreach (var project in projects)
                {
                    var assignment = new Assignment
                    {
                        ProjectId = project.Id,
                        OrgUserId = orguser.Id,
                        CanView = true,
                        CanAdd = true,
                        CanEdit = true,
                        CanDelete = true,
                        CanExportPdf = true,
                        CanExportZip = true
                    };

                    UnitOfWork.AssignmentsRepository.InsertOrUpdate(assignment);
                }

                UnitOfWork.Save();
            }

            // send account confirmation email
            var code = await this.UserManager.GenerateEmailConfirmationTokenAsync(orguser.Id);
            var encodedCode = HttpUtility.UrlEncode(code);

            var rootIndex = GetRootIndexPath();
            var baseUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}";
            var callbackUrl = $"{baseUrl}#!/verify-email?userId={orguser.Id}&code={encodedCode}";

            var messageBody = GenerateAccountConfirmationEmail(callbackUrl);
            await UserManager.SendEmailAsync(orguser.Id, "Confirm your account", messageBody);

            return Ok();
        }

        public IHttpActionResult Put(Guid id, [FromBody]OrgUserDTO value)
        {
            var orguser = Users.Find(id);
            if (orguser == null)
                return NotFound();

            orguser.Email = value.Email;
            orguser.EmailConfirmed = value.EmailConfirmed;
            orguser.FirstName = value.FirstName;
            orguser.Surname = value.Surname;
            orguser.TypeId = value.Type.Id;
            orguser.IsWebUser = value.IsWebUser;
            orguser.IsMobileUser = value.IsMobileUser;
            orguser.Gender = value.Gender;
            orguser.Birthdate = value.Birthdate;
            orguser.Address = value.Address;

            if (!orguser.PhoneNumberConfirmed)
                orguser.PhoneNumber = string.IsNullOrEmpty(value.PhoneNumber) ? null : value.PhoneNumber;

            if (this.CurrentUser is SuperUser)
            {
                if (value.CurrentProject != null)
                {
                    if (Guid.Parse(value.CurrentProject.Organisation.Id) != orguser.Organisation.Id)
                        return BadRequest("The selected current project does not belong to this user's organisation");

                    orguser.CurrentProjectId = value.CurrentProject.Id;
                }
            }

            var result = UnitOfWork.UserManager.UpdateSync(orguser);
            if (result.Succeeded)
                return Ok();
            else
                return BadRequest(result.Errors.ToString(", "));
        }

        public IHttpActionResult Delete(Guid id)
        {
            if (id == CurrentUser.Id)
                throw new InvalidOperationException("Current user cannot be deleted!");

            var orguser = Users.Find(id);
            if (orguser == null)
                return NotFound();

            if (orguser.IsRootUser)
                return BadRequest();

            try
            {
                Users.Delete(id);
                UnitOfWork.Save();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Could not delete this user!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        #region helpers

        // this needs to be refactored to a global static helper.
        private string GetRootIndexPath()
        {
            var rootIndexPath = ConfigurationManager.AppSettings["RootIndexPath"];
            if (!string.IsNullOrEmpty(rootIndexPath))
                return rootIndexPath;

            return "wwwroot/index.html";
        }

        #endregion

        private string GenerateAccountConfirmationEmail(string callbackUrl)
        {
            var path = HostingEnvironment.MapPath("~/EmailTemplates/email-confirmation.html");
            var emailTemplate = System.IO.File.ReadAllText(path, Encoding.UTF8);

            var messageHeaderKey = "{{MESSAGE_HEADING}}";
            var messageBodyKey = "{{MESSAGE_BODY}}";

            var content = @"<p>Your new account has been created. To complete your registration please confirm your email address by clicking the link below.</p>
                            <p><a href='" + callbackUrl + @"'>Verify Email</a></p>";

            emailTemplate = emailTemplate.Replace(messageHeaderKey, "Complete your registration");
            emailTemplate = emailTemplate.Replace(messageBodyKey, content);

            return emailTemplate;
        }

    }
}