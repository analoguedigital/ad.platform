using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
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

        public enum OrgUserListType
        {
            MobileAccounts = 0,
            WebAccounts = 1,
            AllAccounts = 2
        }

        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrgUserDTO>))]
        [Route("api/orgusers/{listType:int}/{organisationId?}")]
        public IHttpActionResult Get(OrgUserListType listType, Guid? organisationId = null)
        {
            var result = new List<OrgUserDTO>();

            if (this.CurrentOrgUser != null)
            {
                var users = Users.AllIncluding(u => u.Type)
                    .Where(u => u.OrganisationId == CurrentOrganisationId)
                    .OrderBy(u => u.Surname)
                    .ThenBy(u => u.FirstName)
                    .AsQueryable();

                if (listType != OrgUserListType.AllAccounts)
                {
                    var accType = (AccountType)(int)listType;
                    users = users.Where(x => x.AccountType == accType);
                }

                result = users
                    .ToList()
                    .Select(u => Mapper.Map<OrgUserDTO>(u))
                    .ToList();
            }
            else
            {
                if (organisationId.HasValue)
                {
                    var users = Users.AllIncluding(u => u.Type)
                        .Where(u => u.OrganisationId == organisationId)
                        .OrderBy(u => u.Surname)
                        .ThenBy(u => u.FirstName)
                        .AsQueryable();

                    if (listType != OrgUserListType.AllAccounts)
                    {
                        var accType = (AccountType)(int)listType;
                        users = users.Where(x => x.AccountType == accType);
                    }

                    result = users
                        .ToList()
                        .Select(u => Mapper.Map<OrgUserDTO>(u))
                        .ToList();
                }
                else
                {
                    var users = Users.AllIncluding(u => u.Type)
                      .OrderBy(u => u.Surname)
                      .ThenBy(u => u.FirstName)
                      .AsQueryable();

                    if (listType != OrgUserListType.AllAccounts)
                    {
                        var accType = (AccountType)(int)listType;
                        users = users.Where(x => x.AccountType == accType);
                    }

                    result = users
                        .ToList()
                        .Select(u => Mapper.Map<OrgUserDTO>(u))
                        .ToList();
                }
            }

            return Ok(result);
        }

        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrgUserDTO>))]
        [Route("api/orgusers/{id:guid}")]
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

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]OrgUserDTO value)
        {
            if (value.Password.IsEmpty())
                ModelState.AddModelError("Password", "Please provide password.");

            if (value.Password != value.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "'Password' and 'Confirm password' must be the same.");

            if (value.AccountType == AccountType.MobileAccount)
            {
                // the OrgUserType property is hidden in mobile-users' edit form.
                // so the Type is null at this point. fetch and populate.
                // TeamUser Type ID: 379c989a-9919-4338-a468-a7c20eb76e28

                var teamUserType = this.UnitOfWork.OrgUserTypesRepository.AllAsNoTracking
                    .Where(x => x.SystemName == "TeamUser")
                    .SingleOrDefault();

                value.Type = Mapper.Map<OrgUserTypeDTO>(teamUserType);
            }

            var orguser = Mapper.Map<OrgUser>(value);
            orguser.UserName = orguser.Email;

            if (this.CurrentUser is SuperUser)
            {
                orguser.OrganisationId = Guid.Parse(value.Organisation.Id);
                orguser.Organisation = null;
            }
            else if (this.CurrentUser is OrgUser)
                orguser.OrganisationId = this.CurrentOrgUser.OrganisationId.Value;

            // generate a random password
            var randomPassword = System.Web.Security.Membership.GeneratePassword(12, 1);
            var identityResult = ServiceContext.UserManager.CreateSync(orguser, randomPassword);

            if (!identityResult.Succeeded)
                throw new Exception(identityResult.Errors.ToString(". "));

            // assign roles by type.
            orguser.Type = UnitOfWork.OrgUserTypesRepository.Find(orguser.TypeId);
            UnitOfWork.UserManager.AssignRolesByUserType(orguser);

            var organisation = this.UnitOfWork.OrganisationRepository.Find(orguser.OrganisationId.Value);

            if (value.Type.Name.ToLower() == "administrator")
            {
                var projects = UnitOfWork.ProjectsRepository.AllAsNoTracking
                    .Where(p => p.OrganisationId == orguser.OrganisationId.Value);

                foreach (var item in projects)
                {
                    var orgUserAssignment = new Assignment
                    {
                        ProjectId = item.Id,
                        OrgUserId = orguser.Id,
                        CanView = true,
                        CanAdd = true,
                        CanEdit = true,
                        CanDelete = true,
                        CanExportPdf = true,
                        CanExportZip = true
                    };

                    UnitOfWork.AssignmentsRepository.InsertOrUpdate(orgUserAssignment);
                }

                UnitOfWork.Save();
            }

            // create a project for this user
            var project = new Project()
            {
                Name = $"{orguser.FirstName} {orguser.Surname}",
                StartDate = DateTimeService.UtcNow,
                OrganisationId = organisation.Id,
                CreatedById = orguser.Id
            };

            UnitOfWork.ProjectsRepository.InsertOrUpdate(project);
            UnitOfWork.Save();

            // assign this user to their project.
            var assignment = new Assignment()
            {
                ProjectId = project.Id,
                OrgUserId = orguser.Id,
                CanView = true,
                CanAdd = true,
                CanEdit = false,
                CanDelete = false,
                CanExportPdf = false,
                CanExportZip = false
            };

            UnitOfWork.AssignmentsRepository.InsertOrUpdate(assignment);

            // assign organisation admin to this project
            if (organisation.RootUser != null)
            {
                UnitOfWork.AssignmentsRepository.InsertOrUpdate(new Assignment
                {
                    ProjectId = project.Id,
                    OrgUserId = organisation.RootUserId.Value,
                    CanView = true,
                    CanAdd = true,
                    CanEdit = true,
                    CanDelete = true,
                    CanExportPdf = true,
                    CanExportZip = true
                });
            }

            UnitOfWork.Save();

            // set user's current project
            var _orgUser = UnitOfWork.OrgUsersRepository.Find(orguser.Id);
            _orgUser.CurrentProjectId = project.Id;

            UnitOfWork.OrgUsersRepository.InsertOrUpdate(_orgUser);
            UnitOfWork.Save();

            // send account confirmation email
            var code = await this.UserManager.GenerateEmailConfirmationTokenAsync(orguser.Id);
            var encodedCode = HttpUtility.UrlEncode(code);

            var rootIndex = GetRootIndexPath();
            var baseUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}";
            var callbackUrl = $"{baseUrl}#!/verify-email?userId={orguser.Id}&code={encodedCode}";

            var messageBody = GenerateAccountConfirmationEmail(callbackUrl, randomPassword);
            await UserManager.SendEmailAsync(orguser.Id, "Confirm your account", messageBody);

            return Ok();
        }

        [HttpPut]
        [Route("api/orgusers/{id:guid}")]
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

        [HttpDelete]
        [Route("api/orgusers/{id:guid}")]
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

        private string GenerateAccountConfirmationEmail(string callbackUrl, string randomPassword)
        {
            var path = HostingEnvironment.MapPath("~/EmailTemplates/email-confirmation.html");
            var emailTemplate = System.IO.File.ReadAllText(path, Encoding.UTF8);

            var messageHeaderKey = "{{MESSAGE_HEADING}}";
            var messageBodyKey = "{{MESSAGE_BODY}}";

            var content = @"<p>Complete your registration by verifying your email address. Click the link below to continue.</p>
                            <p><a href='" + callbackUrl + @"'>Verify Email Address</a></p><br>
                            <p>Your password is <strong>" + randomPassword + @"</strong>.</p>
                            <p>Make sure to change your password after you've signed in.</p>";

            emailTemplate = emailTemplate.Replace(messageHeaderKey, "Welcome to OnRecord");
            emailTemplate = emailTemplate.Replace(messageBodyKey, content);

            return emailTemplate;
        }

    }
}