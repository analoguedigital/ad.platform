using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [RoutePrefix("api/orgConnectionRequests")]
    [Authorize(Roles = "System administrator,Platform administrator,Organisation administrator,Authorized staff")]
    public class OrgConnectionRequestsController : BaseApiController
    {

        #region Properties

        private const string CACHE_KEY = "CONNECTION_REQUESTS";

        #endregion Properties

        #region CRUD

        // GET api/orgConnectionRequests
        [ResponseType(typeof(IEnumerable<OrgConnectionRequestDTO>))]
        public IHttpActionResult Get(Guid? organisationId = null)
        {
            if (CurrentUser is OrgUser)
            {
                var cacheKey = $"{CACHE_KEY}_{CurrentOrgUser.OrganisationId}";
                var cacheEntry = MemoryCacher.GetValue(cacheKey);

                if (cacheEntry == null)
                {
                    var connectionRequests = UnitOfWork.OrgConnectionRequestsRepository
                        .AllAsNoTracking
                        .Where(x => x.OrganisationId == CurrentOrgUser.Organisation.Id)
                        .ToList()
                        .Select(x => Mapper.Map<OrgConnectionRequestDTO>(x))
                        .ToList();

                    MemoryCacher.Add(cacheKey, connectionRequests, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(connectionRequests);
                }
                else
                {
                    var result = (List<OrgConnectionRequestDTO>)cacheEntry;
                    return new CachedResult<List<OrgConnectionRequestDTO>>(result, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is super user
            var _cacheKey = organisationId.HasValue ? $"{CACHE_KEY}_{organisationId.Value}" : CACHE_KEY;
            var _cacheEntry = MemoryCacher.GetValue(_cacheKey);

            if (_cacheEntry == null)
            {
                var connRequests = UnitOfWork.OrgConnectionRequestsRepository.AllAsNoTracking;

                if (organisationId.HasValue)
                    connRequests = connRequests.Where(x => x.OrganisationId == organisationId.Value);

                var retVal = connRequests
                    .ToList()
                    .Select(x => Mapper.Map<OrgConnectionRequestDTO>(x))
                    .ToList();

                MemoryCacher.Add(_cacheKey, retVal, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(retVal);
            }
            else
            {
                var retVal = (List<OrgConnectionRequestDTO>)_cacheEntry;
                return new CachedResult<List<OrgConnectionRequestDTO>>(retVal, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/orgConnectionRequests/{id}
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var cacheKey = $"{CACHE_KEY}_{id}";
            var cacheEntry = MemoryCacher.GetValue(cacheKey);

            if (cacheEntry == null)
            {
                var connectionRequest = UnitOfWork.OrgConnectionRequestsRepository.Find(id);
                if (connectionRequest == null)
                    return NotFound();

                var result = Mapper.Map<OrgConnectionRequestDTO>(connectionRequest);
                MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (OrgConnectionRequestDTO)cacheEntry;
                return new CachedResult<OrgConnectionRequestDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/orgConnectionRequests
        [HttpPost]
        [Route("{organisationId:guid}")]
        [OverrideAuthorization]
        [Authorize(Roles = "Organisation user,Restricted user")]
        public IHttpActionResult Post(Guid organisationId)
        {
            if (CurrentOrgUser.AccountType != AccountType.MobileAccount)
                return BadRequest("connection requests can only be made by mobile users");

            if (organisationId == Guid.Empty)
                return BadRequest("organisation id is empty");

            if (CurrentOrgUser.Organisation.Id == organisationId)
                return BadRequest("you are already connected to this organisation");

            var requestCount = UnitOfWork.OrgConnectionRequestsRepository
                .AllAsNoTracking
                .Count(x => x.OrgUserId == CurrentOrgUser.Id && x.OrganisationId == organisationId && !x.IsApproved);

            if (requestCount > 0)
                return BadRequest("you have already requested to connect to this organisation");

            var organisation = UnitOfWork.OrganisationRepository.Find(organisationId);

            var connectionRequest = new OrgConnectionRequest
            {
                OrgUserId = CurrentOrgUser.Id,
                OrganisationId = organisationId
            };

            UnitOfWork.OrgConnectionRequestsRepository.InsertOrUpdate(connectionRequest);

            NotifyOnRecordAboutNewRequest(organisation.Name);
            NotifyOrgAdminAboutNewRequest(organisation);
            NotifyEmailRecipientsAboutNewRequest(organisation.Name);

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

        // DEL api/orgConnectionRequests/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var request = UnitOfWork.OrgConnectionRequestsRepository.Find(id);
            if (request == null)
                return NotFound();

            // Repo.FindIncluding() doesn't seem to work, params return null.
            // this isn't ideal at all, we should be able to fetch our data
            // in one go, one db-hit. investigate the base repository implementation.
            var organization = UnitOfWork.OrganisationRepository.Find(request.OrganisationId);
            var orgUser = UnitOfWork.OrgUsersRepository.Find(request.OrgUserId);

            UnitOfWork.OrgConnectionRequestsRepository.Delete(id);

            if (!request.IsApproved)
                NotifyUserAboutDeclinedRequest(organization.Name, orgUser.Email);

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

        // POST api/orgConnectionRequests/{id}/approve
        [HttpPost]
        [Route("{id:guid}/approve")]
        public IHttpActionResult Approve(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var connectionRequest = UnitOfWork.OrgConnectionRequestsRepository.Find(id);
            if (connectionRequest == null)
                return NotFound();

            var organisation = UnitOfWork.OrganisationRepository.Find(connectionRequest.Organisation.Id);
            var orgUser = UnitOfWork.OrgUsersRepository.Find(connectionRequest.OrgUser.Id);

            var subscriptionService = new SubscriptionService(UnitOfWork);

            // update the connection request.
            connectionRequest.IsApproved = true;
            connectionRequest.ApprovalDate = DateTimeService.UtcNow;
            UnitOfWork.OrgConnectionRequestsRepository.InsertOrUpdate(connectionRequest);

            // move the user to this organization
            subscriptionService.MoveUserToOrganization(organisation, orgUser);

            // insert notification emails
            NotifyUserAboutApprovedRequest(organisation.Name, orgUser.Email);
            NotifyOrgAdminAboutApprovedRequest(organisation, orgUser);

            try
            {
                UnitOfWork.Save();

                // invalidate connection requests from cache
                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                // invalidate projects from cache
                MemoryCacher.DeleteStartingWith("PROJECTS_ADMIN");
                MemoryCacher.DeleteStartingWith("PROJECTS_ORG_ADMIN");

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion CRUD

        #region Helpers

        private void NotifyOnRecordAboutNewRequest(string organisationName)
        {
            var onRecord = UnitOfWork.OrganisationRepository
                  .AllAsNoTracking
                  .Where(x => x.Name == "OnRecord")
                  .SingleOrDefault();

            if (onRecord.RootUserId.HasValue)
            {
                var onRecordAdmin = UnitOfWork.OrgUsersRepository
                    .AllAsNoTracking
                    .Where(x => x.Id == onRecord.RootUserId.Value)
                    .SingleOrDefault();

                var rootIndex = WebHelpers.GetRootIndexPath();
                var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}#!/organisations/connection-requests/";

                var content = @"<p>The client's name is: <b>" + CurrentOrgUser.UserName + @"</b></p>
                            <p>Organisation: <b>" + organisationName + @"</b></p>
                            <p><br></p>
                            <p>Go to <a href='" + url + @"'>connection requests</a> on the platform menu.</p>";

                var email = new Email
                {
                    To = onRecordAdmin.Email,
                    Subject = $"Someone has requested to join {organisationName}",
                    Content = WebHelpers.GenerateEmailTemplate(content, "Someone has requested to join an organisation")
                };

                UnitOfWork.EmailsRepository.InsertOrUpdate(email);
            }
        }

        private void NotifyOrgAdminAboutNewRequest(Organisation organisation)
        {
            if (organisation.RootUser != null)
            {
                var orgAdmin = UnitOfWork.OrgUsersRepository
                    .AllAsNoTracking
                    .Where(x => x.Id == organisation.RootUser.Id)
                    .SingleOrDefault();

                var rootIndex = WebHelpers.GetRootIndexPath();
                var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}#!/organisations/connection-requests/";

                var content = @"<p>The person's username is: <b>" + CurrentOrgUser.UserName + @"</b></p>
                            <p><br></p>
                            <p>Go to <a href='" + url + @"'>connection requests</a> on the platform menu.</p>";

                var orgAdminEmail = new Email
                {
                    To = orgAdmin.Email,
                    Subject = $"Someone has requested to join your organisation",
                    Content = WebHelpers.GenerateEmailTemplate(content, "Someone has requested to join your organisation")
                };

                UnitOfWork.EmailsRepository.InsertOrUpdate(orgAdminEmail);
            }
        }

        private void NotifyEmailRecipientsAboutNewRequest(string organisationName)
        {
            var recipients = UnitOfWork.EmailRecipientsRepository
                .AllAsNoTracking
                .Where(x => x.OrgConnectionRequests == true)
                .ToList();

            var rootIndex = WebHelpers.GetRootIndexPath();
            var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}#!/organisations/connection-requests/";

            var emailContent = @"<p>The person's user name is: <b>" + CurrentOrgUser.UserName + @"</b></p>
                            <p>Organisation: <b>" + organisationName + @"</b></p>
                            <p><br></p>
                            <p>Go to <a href='" + url + @"'>connection requests</a> on the platform menu.</p>";

            foreach (var recipient in recipients)
            {
                var recipientEmail = new Email
                {
                    To = recipient.OrgUser.Email,
                    Subject = $"Someone has requested to join {organisationName}",
                    Content = WebHelpers.GenerateEmailTemplate(emailContent, "Someone has requested to to join")
                };

                UnitOfWork.EmailsRepository.InsertOrUpdate(recipientEmail);
            }
        }

        private void NotifyUserAboutApprovedRequest(string organisationName, string userEmail)
        {
            var content = @"<p>You have joined <strong>" + organisationName + @"</strong>.</p>
                            <p>Your records are now available to " + organisationName + @".</p>
                            <p>You can end their access to your records at any time by going to the mobile app, then going to the Connect to an Organisation section and unlinking using the Unlink button at the bottom of the screen. Your records will still be safely stored and accessible to you as usual.</p>";

            var email = new Email
            {
                To = userEmail,
                Subject = $"You have joined {organisationName}",
                Content = WebHelpers.GenerateEmailTemplate(content, "You have joined an organisation")
            };

            UnitOfWork.EmailsRepository.InsertOrUpdate(email);
        }

        private void NotifyOrgAdminAboutApprovedRequest(Organisation organisation, OrgUser orgUser)
        {
            if (organisation.RootUserId.HasValue)
            {
                var orgAdmin = UnitOfWork.OrgUsersRepository
                    .AllAsNoTracking
                    .Where(x => x.Id == organisation.RootUserId.Value)
                    .SingleOrDefault();

                var rootIndex = WebHelpers.GetRootIndexPath();
                var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}#!/users/mobile/";

                var emailBody = @"<p>A new client <strong>" + orgUser.UserName + @"</strong>, has joined your organisation and their records are now available to you.</p>
                            <p>You can end your access to any client's records via the OnRecord platform, by going to 'Organisation Management' and then 'Clients', finding their user name and clicking on 'Remove'.</p>
                            <p><br></p>
                            <p>Go to <a href='" + url + @"'>Clients</a> in the Organisation Management section of the platform menu.</p>";

                var orgAdminEmail = new Email
                {
                    To = orgAdmin.Email,
                    Subject = $"A new client, {orgUser.UserName}, has joined {organisation.Name}",
                    Content = WebHelpers.GenerateEmailTemplate(emailBody, "A new client has joined your organisation")
                };

                UnitOfWork.EmailsRepository.InsertOrUpdate(orgAdminEmail);
            }
        }

        private void NotifyUserAboutDeclinedRequest(string organisationName, string userEmail)
        {
            var content = @"<p>Your request to join the <strong>" + organisationName + @"</strong> organization has been declined.</p>
                            <p>If you need help or want to learn more, please contact us at admin@analogue.digital.</p>";

            var email = new Email
            {
                To = userEmail,
                Subject = $"Connection request declined by {organisationName}",
                Content = WebHelpers.GenerateEmailTemplate(content, "Your connection request has been declined")
            };

            UnitOfWork.EmailsRepository.InsertOrUpdate(email);
        }

        #endregion Helpers

    }
}
