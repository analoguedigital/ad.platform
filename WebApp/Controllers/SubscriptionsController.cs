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
using System.Web.Http.Description;
using WebApi.Filters;

namespace WebApi.Controllers
{
    public class UpdateSubscriptionEntryDTO
    {
        public Guid RecordId { get; set; }

        public UserSubscriptionType Type { get; set; }
    }

    public class SubscriptionsController : BaseApiController
    {
        private SubscriptionService SubscriptionService { get; set; }

        public SubscriptionsController()
        {
            this.SubscriptionService = new SubscriptionService(this.CurrentOrgUser, this.UnitOfWork);
        }

        [DeflateCompression]
        [Route("api/subscriptions")]
        [ResponseType(typeof(IEnumerable<SubscriptionEntryDTO>))]
        public IHttpActionResult Get()
        {
            if (this.CurrentUser is SuperUser)
                return Ok();

            var subscriptions = this.SubscriptionService.GetUserSubscriptions(this.CurrentOrgUser.Id);
            return Ok(subscriptions);
        }

        [DeflateCompression]
        [Route("api/subscriptions/user/{id:guid}")]
        [ResponseType(typeof(IEnumerable<SubscriptionEntryDTO>))]
        public IHttpActionResult Get(Guid id)
        {
            var subscriptions = this.SubscriptionService.GetUserSubscriptions(id);
            return Ok(subscriptions);
        }

        [DeflateCompression]
        [Route("api/subscriptions/getLatest")]
        [ResponseType(typeof(LatestSubscriptionDTO))]
        public IHttpActionResult GetLatest()
        {
            if (this.CurrentUser is SuperUser)
                return Ok();

            var latestSubscription = this.SubscriptionService.GetLatest();
            var result = new LatestSubscriptionDTO { Date = latestSubscription };

            return Ok(result);
        }

        [DeflateCompression]
        [Route("api/subscriptions/last")]
        [ResponseType(typeof(SubscriptionDTO))]
        public IHttpActionResult GetLastSubscription()
        {
            if (this.CurrentUser is SuperUser)
                return Ok();

            var lastSubscription = this.SubscriptionService.GetLastSubscription(this.CurrentOrgUser.Id);

            return Ok(lastSubscription);
        }

        [HttpPost]
        [Route("api/subscriptions/buy/{id:guid}")]
        public IHttpActionResult Buy(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            var plan = UnitOfWork.SubscriptionPlansRepository.Find(id);
            if (plan == null)
                return NotFound();

            if (this.CurrentOrgUser == null || this.CurrentOrgUser.AccountType != AccountType.MobileAccount)
                return BadRequest("Paid plans are only available to mobile users");

            var payment = new PaymentRecord
            {
                Date = DateTimeService.UtcNow,
                Amount = plan.Price,
                Note = $"Purchased subscription - {plan.Name}",
                Reference = string.Empty,
                OrgUserId = this.CurrentOrgUser.Id
            };

            UnitOfWork.PaymentsRepository.InsertOrUpdate(payment);

            var subscriptions = this.UnitOfWork.SubscriptionsRepository.AllAsNoTracking.Where(s => s.OrgUserId == this.CurrentOrgUser.Id);
            var startDate = DateTimeService.UtcNow;

            for (var index = 0; index < plan.Length; index++)
            {
                var subscription = new Subscription
                {
                    IsActive = true,
                    Type = UserSubscriptionType.Paid,
                    StartDate = startDate.AddMonths(index),
                    EndDate = startDate.AddMonths(index).AddMonths(1),
                    Note = $"Paid subscription - {plan.Name}",
                    PaymentRecord = payment,
                    OrgUserId = this.CurrentOrgUser.Id,
                    SubscriptionPlanId = plan.Id,
                };

                UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);
            }

            // update export permissions based on purchased plan
            var orgUser = UnitOfWork.OrgUsersRepository.Find(this.CurrentOrgUser.Id);
            if (orgUser.CurrentProjectId.HasValue)
            {
                var orgUserAssignment = orgUser.Assignments.Where(x => x.ProjectId == orgUser.CurrentProject.Id).SingleOrDefault();
                if (orgUserAssignment != null)
                {
                    orgUserAssignment.CanExportPdf = plan.PdfExport;
                    orgUserAssignment.CanExportZip = plan.ZipExport;

                    UnitOfWork.AssignmentsRepository.InsertOrUpdate(orgUserAssignment);
                }
            }

            // cancel last subscription, if any.
            var lastSubscription = this.UnitOfWork.SubscriptionsRepository.AllAsNoTracking
                .Where(x => x.OrgUserId == this.CurrentOrgUser.Id && x.IsActive)
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

            try
            {
                this.CurrentOrgUser.IsSubscribed = true;

                var email = new Email
                {
                    To = this.CurrentOrgUser.Email,
                    Subject = $"Subscription purchase - {plan.Name}",
                    Content = GeneratePaidPlanEmail(plan)
                };

                UnitOfWork.EmailsRepository.InsertOrUpdate(email);
                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/subscriptions/joinorganisation/{token}")]
        public IHttpActionResult JoinOrganisation(string token)
        {
            if (CurrentUser is SuperUser)
                return BadRequest("Organisation invitations are only available to mobile users");

            if (string.IsNullOrEmpty(token))
                return BadRequest();

            var invitation = UnitOfWork.OrgInvitationsRepository.AllAsNoTracking
                .Where(x => x.Token == token)
                .SingleOrDefault();

            if (invitation == null)
                return NotFound();

            if (invitation.Used + 1 > invitation.Limit || !invitation.IsActive)
                return BadRequest("This invitation has been closed");

            // move this OrgUser and their personal case to the new organisation.
            var orgUser = this.UnitOfWork.OrgUsersRepository.Find(this.CurrentOrgUser.Id);

            if (orgUser.Organisation.Id == invitation.Organisation.Id)
                return BadRequest("You're already connected to this organization");

            var subscription = new Subscription
            {
                IsActive = true,
                Type = UserSubscriptionType.Organisation,
                StartDate = DateTimeService.UtcNow,
                EndDate = null,
                Note = $"Joined organisation - {invitation.Organisation.Name}",
                OrgUserId = this.CurrentOrgUser.Id,
                OrganisationId = invitation.Organisation.Id
            };

            UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);

            invitation.Used += 1;
            UnitOfWork.OrgInvitationsRepository.InsertOrUpdate(invitation);

            // remove this user from any teams in current organisation.
            var records = UnitOfWork.OrgTeamUsersRepository.AllAsNoTracking
                .Where(x => x.OrgUserId == orgUser.Id && x.OrganisationTeam.OrganisationId == orgUser.OrganisationId)
                .ToList();

            foreach (var item in records)
                UnitOfWork.OrgTeamUsersRepository.Delete(item);

            orgUser.OrganisationId = invitation.Organisation.Id;    // update user's organisation

            if (orgUser.CurrentProject != null) // update user's current project, if exists
            {
                if (orgUser.CurrentProject.CreatedById == orgUser.Id)
                {
                    var project = UnitOfWork.ProjectsRepository.Find(orgUser.CurrentProject.Id);
                    project.OrganisationId = invitation.Organisation.Id;    // update project's organisation

                    // update threads under this project
                    var threads = UnitOfWork.FormTemplatesRepository.AllAsNoTracking
                        .Where(t => t.ProjectId == project.Id)
                        .ToList();

                    // update form templates' organisation
                    foreach (var form in threads)
                    {
                        form.OrganisationId = invitation.Organisation.Id;
                        UnitOfWork.FormTemplatesRepository.InsertOrUpdate(form);
                    }

                    var rootAdminAssignment = UnitOfWork.AssignmentsRepository.AllAsNoTracking
                        .Where(x => x.OrgUserId == invitation.Organisation.RootUserId.Value && x.ProjectId == orgUser.CurrentProjectId.Value)
                        .SingleOrDefault();

                    if (rootAdminAssignment == null)
                    {
                        // assign Org root user to the project
                        UnitOfWork.AssignmentsRepository.InsertOrUpdate(new Assignment
                        {
                            ProjectId = orgUser.CurrentProjectId.Value,
                            OrgUserId = invitation.Organisation.RootUserId.Value,
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
               .Where(x => x.OrgUserId == this.CurrentOrgUser.Id && x.IsActive)
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
                this.CurrentOrgUser.IsSubscribed = true;

                var email = new Email
                {
                    To = this.CurrentOrgUser.Email,
                    Subject = $"Joined organization - {invitation.Organisation.Name}",
                    Content = GenerateOrgSubscriptionEmail(invitation)
                };

                UnitOfWork.EmailsRepository.InsertOrUpdate(email);

                if (invitation.Organisation.RootUser != null)
                {
                    var orgAdmin = this.UnitOfWork.OrgUsersRepository.AllAsNoTracking
                        .Where(x => x.Id == invitation.Organisation.RootUser.Id)
                        .SingleOrDefault();

                    var orgAdminEmail = new Email
                    {
                        To = orgAdmin.Email,
                        Subject = $"User joined organization - {this.CurrentOrgUser.UserName}",
                        Content = GenerateOrgSubscriptionAdminEmail(invitation, this.CurrentOrgUser)
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

        [Route("api/subscriptions/quota")]
        public IHttpActionResult GetQuota()
        {
            if (this.CurrentUser is SuperUser)
                return Ok();

            var quota = this.SubscriptionService.GetMonthlyQuota(this.CurrentOrgUser.Id);
            return Ok(quota);
        }

        [HttpPost]
        [Route("api/subscriptions/close")]
        public IHttpActionResult CloseSubscription([FromBody] UpdateSubscriptionEntryDTO value)
        {
            if (value.Type == UserSubscriptionType.Organisation)
            {
                var subscription = UnitOfWork.SubscriptionsRepository.Find(value.RecordId);
                if (subscription == null)
                    return NotFound();

                subscription.EndDate = DateTimeService.UtcNow;
                subscription.IsActive = false;

                UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);
            }
            else
            {
                var payment = UnitOfWork.PaymentsRepository.Find(value.RecordId);
                if (payment == null)
                    return NotFound();

                foreach (var subscription in payment.Subscriptions)
                    subscription.IsActive = false;

                UnitOfWork.PaymentsRepository.InsertOrUpdate(payment);
            }

            try
            {
                UnitOfWork.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/subscriptions/remove")]
        public IHttpActionResult RemoveSubscription([FromBody] UpdateSubscriptionEntryDTO value)
        {
            if (value.Type == UserSubscriptionType.Organisation)
                UnitOfWork.SubscriptionsRepository.Delete(value.RecordId);
            else
                UnitOfWork.PaymentsRepository.Delete(value.RecordId);

            try
            {
                UnitOfWork.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = "System administrator,Platform administrator")]
        [Route("api/subscriptions/joinOnRecord/{userId:guid}")]
        public IHttpActionResult JoinOnRecord(Guid userId)
        {
            if (userId == null || userId == Guid.Empty)
                return BadRequest();

            var orgUser = UnitOfWork.OrgUsersRepository.Find(userId);
            if (orgUser == null)
                return NotFound();

            if (orgUser.AccountType != AccountType.MobileAccount)
                return BadRequest("Only mobile users can join OnRecord");

            var onRecord = UnitOfWork.OrganisationRepository.AllAsNoTracking
                .Where(x => x.Name == "OnRecord")
                .SingleOrDefault();

            var subscription = new Subscription
            {
                IsActive = true,
                Type = UserSubscriptionType.Organisation,
                StartDate = DateTimeService.UtcNow,
                EndDate = null,
                Note = $"Joined organisation - OnRecord",
                OrgUserId = userId,
                OrganisationId = onRecord.Id
            };
            
            UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);

            try
            {
                UnitOfWork.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #region Helpers

        private string GeneratePaidPlanEmail(SubscriptionPlan plan)
        {
            var path = HostingEnvironment.MapPath("~/EmailTemplates/subscribed-paid-plan.html");
            var emailTemplate = System.IO.File.ReadAllText(path, Encoding.UTF8);

            var messageHeaderKey = "{{MESSAGE_HEADING}}";
            var messageBodyKey = "{{MESSAGE_BODY}}";
            var content = @"<p>Subscription purchased: <strong>" + plan.Name + @"</strong></p>
                            <p>Description: " + plan.Description + @"</p>
                            <p>Price: " + plan.Price + @" GBP</p>
                            <p>Length: " + plan.Length + @" month(s)</p>
                            <p>Is Limited: " + plan.IsLimited + @"</p>
                            <p>Monthly Quota: " + plan.MonthlyQuota.ToString() + @"</p>
                            <p>PDF Export: " + plan.PdfExport + @"</p>
                            <p>Zip Export: " + plan.ZipExport + @"</p>";

            emailTemplate = emailTemplate.Replace(messageHeaderKey, plan.Name);
            emailTemplate = emailTemplate.Replace(messageBodyKey, content);

            return emailTemplate;
        }

        private string GenerateOrgSubscriptionEmail(OrganisationInvitation invitation)
        {
            var path = HostingEnvironment.MapPath("~/EmailTemplates/subscribed-paid-plan.html");
            var emailTemplate = System.IO.File.ReadAllText(path, Encoding.UTF8);

            var messageHeaderKey = "{{MESSAGE_HEADING}}";
            var messageBodyKey = "{{MESSAGE_BODY}}";

            var content = @"<p>You have joined the <strong>" + invitation.Organisation.Name + @"</strong> organization.</p>
                            <p>Your personal case and its threads are now filed under this organization.</p>
                            <p>If you like to opt-out and disconnect from this organization, please contact your administrator.</p>";

            emailTemplate = emailTemplate.Replace(messageHeaderKey, "You've joined an organisation");
            emailTemplate = emailTemplate.Replace(messageBodyKey, content);

            return emailTemplate;
        }

        private string GenerateOrgSubscriptionAdminEmail(OrganisationInvitation invitation, OrgUser orgUser)
        {
            var path = HostingEnvironment.MapPath("~/EmailTemplates/subscribed-paid-plan.html");
            var emailTemplate = System.IO.File.ReadAllText(path, Encoding.UTF8);

            var messageHeaderKey = "{{MESSAGE_HEADING}}";
            var messageBodyKey = "{{MESSAGE_BODY}}";

            var rootIndex = GetRootIndexPath();
            var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}#!/users/mobile/";

            var content = @"<p>A new user has joined your organisation: <strong>" + orgUser.UserName + @"</strong>.</p>
                            <p>The user's personal case is now filed under your organisation and you have access to it.</p>
                            <p>You can remove this user whenever you like, and put them back under OnRecord.</p>
                            <p><br></p>
                            <p>View the <a href='" + url + @"'>directory of mobile users</a> on the dashboard.</p>";

            emailTemplate = emailTemplate.Replace(messageHeaderKey, "A User Has Joined Your Organisation");
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

        #endregion

    }
}
