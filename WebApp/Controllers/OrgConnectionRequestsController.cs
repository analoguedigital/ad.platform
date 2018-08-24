using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Http;

namespace WebApi.Controllers
{
    [RoutePrefix("api/orgConnectionRequests")]
    public class OrgConnectionRequestsController : BaseApiController
    {

        // GET api/orgConnectionRequests
        public IHttpActionResult Get(Guid? organisationId = null)
        {
            var result = new List<OrgConnectionRequestDTO>();

            if (CurrentUser is SuperUser)
            {
                var connectionRequests = UnitOfWork.OrgConnectionRequestsRepository.AllAsNoTracking;

                if (organisationId.HasValue)
                    connectionRequests = connectionRequests.Where(x => x.OrganisationId == organisationId.Value);

                result = connectionRequests.ToList()
                    .Select(x => Mapper.Map<OrgConnectionRequestDTO>(x))
                    .ToList();
            }
            else if (CurrentUser is OrgUser)
            {
                var connectionRequests = UnitOfWork.OrgConnectionRequestsRepository.AllAsNoTracking
                    .Where(x => x.OrganisationId == this.CurrentOrgUser.Organisation.Id);

                result = connectionRequests.ToList()
                    .Select(x => Mapper.Map<OrgConnectionRequestDTO>(x))
                    .ToList();
            }

            return Ok(result);
        }

        // GET api/orgConnectionRequests/{id}
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var connectionRequest = UnitOfWork.OrgConnectionRequestsRepository.Find(id);
            if (connectionRequest == null)
                return NotFound();

            return Ok(Mapper.Map<OrgConnectionRequestDTO>(connectionRequest));
        }

        // POST api/orgConnectionRequests
        [HttpPost]
        [Route("{organisationId:guid}")]
        public IHttpActionResult Post(Guid organisationId)
        {
            if (organisationId == null || organisationId == Guid.Empty)
                return BadRequest();

            if (this.CurrentUser is SuperUser)
                return BadRequest("Connection requests can only be made by Organisation Users");

            if (this.CurrentOrgUser.AccountType != AccountType.MobileAccount)
                return BadRequest("Connection requests can only be made by Mobile Users");

            if (this.CurrentOrgUser.Organisation.Id == organisationId)
                return BadRequest("You are already connected to this organisation");

            if (UnitOfWork.OrgConnectionRequestsRepository.AllAsNoTracking
                .Count(x => x.OrgUserId == this.CurrentOrgUser.Id && x.OrganisationId == organisationId) > 0)
            {
                return BadRequest("You have already made a request to connect to this organisation");
            }

            try
            {
                var organisation = UnitOfWork.OrganisationRepository.Find(organisationId);

                var connectionRequest = new OrgConnectionRequest
                {
                    OrgUserId = this.CurrentOrgUser.Id,
                    OrganisationId = organisationId
                };

                UnitOfWork.OrgConnectionRequestsRepository.InsertOrUpdate(connectionRequest);

                var onRecord = UnitOfWork.OrganisationRepository.AllAsNoTracking
                    .Where(x => x.Name == "OnRecord").SingleOrDefault();
                if (onRecord.RootUserId.HasValue)
                {
                    var onRecordAdmin = UnitOfWork.OrgUsersRepository.AllAsNoTracking
                        .Where(x => x.Id == onRecord.RootUserId.Value)
                        .SingleOrDefault();

                    var rootIndex = WebHelpers.GetRootIndexPath();
                    var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}#!/organisations/connection-requests/";

                    var content = @"<p>User name: <b>" + this.CurrentOrgUser.UserName + @"</b></p>
                            <p>Organisation: <b>" + organisation.Name + @"</b></p>
                            <p><br></p>
                            <p>View <a href='" + url + @"'>connection requests</a> on the dashboard.</p>";

                    var email = new Email
                    {
                        To = onRecordAdmin.Email,
                        Subject = $"A user has requested to join an organization",
                        Content = WebHelpers.GenerateEmailTemplate(content, "A user has requested to join an organization")
                    };

                    UnitOfWork.EmailsRepository.InsertOrUpdate(email);
                }

                if (organisation.RootUser != null)
                {
                    var orgAdmin = this.UnitOfWork.OrgUsersRepository.AllAsNoTracking
                        .Where(x => x.Id == organisation.RootUser.Id)
                        .SingleOrDefault();

                    var rootIndex = WebHelpers.GetRootIndexPath();
                    var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}#!/organisations/connection-requests/";

                    var content = @"<p>User name: <b>" + this.CurrentOrgUser.UserName + @"</b></p>
                            <p><br></p>
                            <p>View <a href='" + url + @"'>connection requests</a> on the dashboard.</p>";

                    var orgAdminEmail = new Email
                    {
                        To = orgAdmin.Email,
                        Subject = $"A user has requested to join your organization",
                        Content = WebHelpers.GenerateEmailTemplate(content, "A user has request to join your organization")
                    };

                    UnitOfWork.EmailsRepository.InsertOrUpdate(orgAdminEmail);
                }

                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DEL api/orgConnectionRequests/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                UnitOfWork.OrgConnectionRequestsRepository.Delete(id);
                UnitOfWork.OrgConnectionRequestsRepository.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/orgConnectionRequests/{id}/approve
        [HttpPost]
        [Route("{id:guid}/approve")]
        public IHttpActionResult Approve(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            var connectionRequest = UnitOfWork.OrgConnectionRequestsRepository.Find(id);
            if (connectionRequest == null)
                return NotFound();

            var organisation = UnitOfWork.OrganisationRepository.Find(connectionRequest.Organisation.Id);
            var orgUser = UnitOfWork.OrgUsersRepository.Find(connectionRequest.OrgUser.Id);

            // update the connection request.
            connectionRequest.IsApproved = true;
            connectionRequest.ApprovalDate = DateTimeService.UtcNow;

            UnitOfWork.OrgConnectionRequestsRepository.InsertOrUpdate(connectionRequest);

            var subscription = new Subscription
            {
                IsActive = true,
                Type = UserSubscriptionType.Organisation,
                StartDate = DateTimeService.UtcNow,
                EndDate = null,
                Note = $"Joined organisation - {organisation.Name}",
                OrgUserId = orgUser.Id
            };

            UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);

            // remove this user from any teams in current organisation.
            var records = UnitOfWork.OrgTeamUsersRepository.AllAsNoTracking
                .Where(x => x.OrgUserId == orgUser.Id && x.OrganisationTeam.OrganisationId == organisation.Id)
                .ToList();

            foreach (var item in records)
                UnitOfWork.OrgTeamUsersRepository.Delete(item);

            orgUser.OrganisationId = organisation.Id;    // update user's organisation

            if (orgUser.CurrentProject != null) // update user's current project, if exists
            {
                if (orgUser.CurrentProject.CreatedById == orgUser.Id)
                {
                    var project = UnitOfWork.ProjectsRepository.Find(orgUser.CurrentProject.Id);
                    project.OrganisationId = organisation.Id;    // update project's organisation
                    UnitOfWork.ProjectsRepository.InsertOrUpdate(project);

                    // update threads under this project
                    var threads = UnitOfWork.FormTemplatesRepository.AllAsNoTracking
                        .Where(t => t.ProjectId == project.Id)
                        .ToList();

                    // update form templates' organisation
                    foreach (var form in threads)
                    {
                        form.OrganisationId = organisation.Id;
                        UnitOfWork.FormTemplatesRepository.InsertOrUpdate(form);
                    }

                    var rootAdminAssignment = UnitOfWork.AssignmentsRepository.AllAsNoTracking
                        .Where(x => x.OrgUserId == organisation.RootUserId.Value && x.ProjectId == orgUser.CurrentProjectId.Value)
                        .SingleOrDefault();

                    if (rootAdminAssignment == null)
                    {
                        // assign Org root user to the project
                        UnitOfWork.AssignmentsRepository.InsertOrUpdate(new Assignment
                        {
                            ProjectId = orgUser.CurrentProjectId.Value,
                            OrgUserId = organisation.RootUserId.Value,
                            CanAdd = true,
                            CanEdit = true,
                            CanDelete = true,
                            CanView = true,
                            CanExportPdf = true,
                            CanExportZip = true
                        });
                    }
                }
            }

            // cancel last subscription, if any.
            var lastSubscription = this.UnitOfWork.SubscriptionsRepository.AllAsNoTracking
               .Where(x => x.OrgUserId == orgUser.Id && x.IsActive)
               .OrderByDescending(x => x.DateCreated)
               .FirstOrDefault();

            if (lastSubscription != null)
            {
                if (lastSubscription.Type == UserSubscriptionType.Organisation)
                {
                    lastSubscription.EndDate = DateTimeService.UtcNow;
                    lastSubscription.IsActive = false;

                    this.UnitOfWork.SubscriptionsRepository.InsertOrUpdate(lastSubscription);
                }
                else
                {
                    var paymentRecord = this.UnitOfWork.PaymentsRepository.Find(lastSubscription.PaymentRecord.Id);
                    foreach (var record in paymentRecord.Subscriptions)
                    {
                        record.IsActive = false;
                    }

                    this.UnitOfWork.PaymentsRepository.InsertOrUpdate(paymentRecord);
                }
            }

            // grant export access to the user
            if (orgUser.CurrentProjectId.HasValue)
            {
                var orgUserAssignment = orgUser.Assignments.Where(x => x.ProjectId == orgUser.CurrentProject.Id).SingleOrDefault();
                if (orgUserAssignment != null)
                {
                    orgUserAssignment.CanExportPdf = true;
                    orgUserAssignment.CanExportZip = true;

                    UnitOfWork.AssignmentsRepository.InsertOrUpdate(orgUserAssignment);
                }
            }

            try
            {
                orgUser.IsSubscribed = true;

                var content = @"<p>You have joined the <strong>" + organisation.Name + @"</strong> organization.</p>
                            <p>Your personal case and its threads are now filed under this organization.</p>
                            <p>If you like to opt-out and disconnect from this organization, please contact your administrator.</p>";

                var email = new Email
                {
                    To = orgUser.Email,
                    Subject = $"Joined organization - {organisation.Name}",
                    Content = WebHelpers.GenerateEmailTemplate(content, "You have joined an organization")
                };

                UnitOfWork.EmailsRepository.InsertOrUpdate(email);

                if (organisation.RootUserId.HasValue)
                {
                    var orgAdmin = this.UnitOfWork.OrgUsersRepository.AllAsNoTracking
                        .Where(x => x.Id == organisation.RootUserId.Value)
                        .SingleOrDefault();

                    var rootIndex = WebHelpers.GetRootIndexPath();
                    var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}#!/users/mobile/";

                    var emailBody = @"<p>A new user has joined your organisation: <strong>" + orgUser.UserName + @"</strong>.</p>
                            <p>The user's personal case is now filed under your organisation and you have access to it.</p>
                            <p>You can remove this user whenever you like, and put them back under OnRecord.</p>
                            <p><br></p>
                            <p>View the <a href='" + url + @"'>directory of mobile users</a> on the dashboard.</p>";

                    var orgAdminEmail = new Email
                    {
                        To = orgAdmin.Email,
                        Subject = $"User joined organization - {orgUser.UserName}",
                        Content = WebHelpers.GenerateEmailTemplate(emailBody, "A user has joined your organization")
                    };

                    UnitOfWork.EmailsRepository.InsertOrUpdate(orgAdminEmail);
                }

                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
