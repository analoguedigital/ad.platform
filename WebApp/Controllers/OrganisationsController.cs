using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator,Platform administrator")]
    public class OrganisationsController : BaseApiController
    {

        private const string CACHE_KEY = "ORGANISATIONS";

        #region Properties

        private OrganisationRepository Organisations
        {
            get { return UnitOfWork.OrganisationRepository; }
        }

        private OrgUsersRepository OrgUsers
        {
            get { return UnitOfWork.OrgUsersRepository; }
        }

        #endregion

        #region CRUD

        // GET api/organisations
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrganisationDTO>))]
        public IHttpActionResult Get()
        {
            var cacheEntry = MemoryCacher.GetValue(CACHE_KEY);
            if (cacheEntry == null)
            {
                var organisations = Organisations.AllIncluding(u => u.RootUser)
                    .OrderBy(u => u.Name)
                    .ToList()
                    .Select(u => Mapper.Map<OrganisationDTO>(u))
                    .ToList();

                MemoryCacher.Add(CACHE_KEY, organisations, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(organisations);
            }
            else
            {
                var result = (List<OrganisationDTO>)cacheEntry;
                return new CachedResult<List<OrganisationDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/organisations/getList
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<OrganisationDTO>))]
        [Route("api/organisations/getlist")]
        [OverrideAuthorization()]
        [Authorize(Roles = "Organisation user,Restricted user")]
        public IHttpActionResult GetList()
        {
            //var isOrgAdmin = await ServiceContext.UserManager.IsInRoleAsync(CurrentOrgUser.Id, "Organisation administrator");
            if (CurrentOrgUser.AccountType != AccountType.MobileAccount)
                return BadRequest("organisations list is only available to mobile users");

            var cacheKey = $"{CACHE_KEY}_LIST";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var organisations = Organisations
                    .AllAsNoTracking
                    .Where(x => !x.Name.Equals("OnRecord"))
                    .OrderBy(x => x.Name)
                    .ToList()
                    .Select(x => Mapper.Map<OrganisationDTO>(x))
                    .ToList();

                MemoryCacher.Add(cacheKey, organisations, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(organisations);
            }
            else
            {
                var result = (List<OrganisationDTO>)cacheEntry;
                return new CachedResult<List<OrganisationDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/organisations/{id}
        [DeflateCompression]
        [ResponseType(typeof(OrganisationDTO))]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<OrganisationDTO>(new Organisation()));

            var cacheKey = $"{CACHE_KEY}_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var organisation = Organisations.Find(id);
                if (organisation == null)
                    return NotFound();

                var result = Mapper.Map<OrganisationDTO>(organisation);
                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (OrganisationDTO)cacheEntry;
                return new CachedResult<OrganisationDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/organisations
        public IHttpActionResult Post([FromBody]OrganisationDTO value)
        {
            var createOrganisation = new CreateOrganisation();
            createOrganisation.Name = value.Name;
            createOrganisation.RootUserEmail = value.RootUser.Email;
            createOrganisation.RootUserFirstName = value.RootUser.FirstName;
            createOrganisation.RootUserSurname = value.RootUser.Surname;
            createOrganisation.RootPassword = value.RootUser.Password;
            createOrganisation.RootConfirmPassword = value.RootUser.ConfirmPassword;
            createOrganisation.AddressLine1 = value.AddressLine1;
            createOrganisation.AddressLine2 = value.AddressLine2;
            createOrganisation.County = value.County;
            createOrganisation.Town = value.Town;
            createOrganisation.Postcode = value.Postcode;
            createOrganisation.TelNumber = value.TelNumber;
            createOrganisation.Description = value.Description;
            createOrganisation.Website = value.Website;
            createOrganisation.LogoUrl = value.LogoUrl;
            createOrganisation.TermsAndConditions = value.TermsAndConditions;
            createOrganisation.RequiresAgreement = value.RequiresAgreement;
            createOrganisation.DefaultCalendarId = CalendarsRepository.Gregorian.Id;
            createOrganisation.DefaultLanguageId = LanguagesRepository.English.Id;

            Organisations.CreateOrganisation(createOrganisation);

            try
            {
                UnitOfWork.Save();
                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/organisations/{id}
        public IHttpActionResult Put(Guid id, [FromBody]OrganisationDTO value)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var organisation = Organisations.Find(id);
            if (organisation == null)
                return NotFound();

            organisation.Name = value.Name;
            organisation.SubscriptionEnabled = value.SubscriptionEnabled;
            organisation.SubscriptionMonthlyRate = value.SubscriptionMonthlyRate;
            organisation.AddressLine1 = value.AddressLine1;
            organisation.AddressLine2 = value.AddressLine2;
            organisation.Town = value.Town;
            organisation.County = value.County;
            organisation.Postcode = value.Postcode;
            organisation.TelNumber = value.TelNumber;
            organisation.Description = value.Description;
            organisation.Website = value.Website;
            organisation.LogoUrl = value.LogoUrl;
            organisation.TermsAndConditions = value.TermsAndConditions;
            organisation.RequiresAgreement = value.RequiresAgreement;

            try
            {
                Organisations.InsertOrUpdate(organisation);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DEL api/organisations/{id}
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            try
            {
                Organisations.Delete(id);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (DbUpdateException)
            {
                return BadRequest("this organisation cannot be deleted");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion CRUD

        #region User Management

        // POST api/organisations/{id}/assign
        [HttpPost]
        [Route("api/organisations/{id:guid}/assign")]
        public IHttpActionResult AssignUsers(Guid id, OrganisationAssignmentDTO model)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var org = Organisations.Find(id);
            if (org == null)
                return NotFound();

            var subscriptionService = new SubscriptionService(UnitOfWork);
            subscriptionService.MoveUsersToOrganisation(org, model.OrgUsers);

            try
            {
                // TODO: notify the user by email.
                // notify orgUser about joining organisation
                // notify orgAdmin about new user

                UnitOfWork.Save();
                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DEL api/organisations/{id}/revoke/{userId}
        [HttpDelete]
        [Route("api/organisations/{id:guid}/revoke/{userId:guid}")]
        [OverrideAuthorization()]
        [Authorize(Roles = "System administrator,Platform administrator,Organisation administrator,Organisation user")]
        public IHttpActionResult RevokeUser(Guid id, Guid userId)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            if (userId == Guid.Empty)
                return BadRequest("user id is empty");

            var org = Organisations.Find(id);
            if (org == null)
                return NotFound();

            var orgUser = UnitOfWork.OrgUsersRepository.Find(userId);
            if (orgUser == null)
                return NotFound();

            // root users cannot be removed from an organization!
            if (orgUser.IsRootUser)
                return BadRequest("Root users cannot be removed from organizations!");

            var subscriptionService = new SubscriptionService(UnitOfWork);
            subscriptionService.RemoveUserFromOrganization(org, orgUser);

            try
            {
                // send email notifications
                NotifyUserAboutLeavingOrganisation(org.Name, orgUser.Email);
                NotifyOrgAdminAboutUserLeaving(org, orgUser.UserName);

                UnitOfWork.Save();
                MemoryCacher.DeleteStartingWith(CACHE_KEY);
                MemoryCacher.DeleteStartingWith("ORG_USERS");

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion User Management

        #region Helpers

        private void NotifyUserAboutLeavingOrganisation(string organisationName, string userEmail)
        {
            var userEmailContent = @"<p>You have left the <strong>" + organisationName + @"</strong> organization.</p>
                            <p>Your personal case has been moved back to OnRecord. And they don't have access to your files anymore, except for any assignments you might have created.</p>";

            var email = new Email
            {
                To = userEmail,
                Subject = $"Left organization - {organisationName}",
                Content = WebHelpers.GenerateEmailTemplate(userEmailContent, "You have left an organization")
            };

            UnitOfWork.EmailsRepository.InsertOrUpdate(email);
        }

        private void NotifyOrgAdminAboutUserLeaving(Organisation organisation, string userName)
        {
            if (organisation.RootUser != null)
            {
                var adminEmailContent = @"<p>A user has left your organization: <strong>" + userName + @"</strong>.</p>
                            <p>And their personal case has been moved back to OnRecord.</p>";

                var adminEmail = new Email
                {
                    To = organisation.RootUser.Email,
                    Subject = $"User left organization - {userName}",
                    Content = WebHelpers.GenerateEmailTemplate(adminEmailContent, "A user has left your organization")
                };

                UnitOfWork.EmailsRepository.InsertOrUpdate(adminEmail);
            }
        }

        #endregion Helpers

    }
}