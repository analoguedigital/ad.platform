using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;

namespace WebApi.Controllers
{
    public class SubscriptionsController : BaseApiController
    {
        private SubscriptionService SubscriptionService { get; set; }

        public SubscriptionsController()
        {
            this.SubscriptionService = new SubscriptionService(this.CurrentOrgUser, this.UnitOfWork);
        }

        [DeflateCompression]
        [Route("api/subscriptions")]
        [ResponseType(typeof(IEnumerable<SubscriptionDTO>))]
        public IHttpActionResult Get()
        {
            var subscriptions = this.SubscriptionService.GetUserSubscriptions();
            var result = subscriptions.Select(s => Mapper.Map<SubscriptionDTO>(s));

            return Ok(result);
        }

        [DeflateCompression]
        [Route("api/subscriptions/getLatest")]
        [ResponseType(typeof(LatestSubscriptionDTO))]
        public IHttpActionResult GetLatest()
        {
            var latestSubscription = this.SubscriptionService.GetLatest();
            var result = new LatestSubscriptionDTO { Date = latestSubscription };

            return Ok(result);
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
                return BadRequest("Purchasing subscriptions is only available to Mobile Users");

            var payment = new PaymentRecord
            {
                Date = DateTimeService.UtcNow,
                Amount = plan.Price,
                Note = $"Purchased subscription - {plan.Name}",
                Reference = string.Empty,
                OrgUserId = this.CurrentUser.Id
            };

            UnitOfWork.PaymentsRepository.InsertOrUpdate(payment);

            var subscriptions = this.UnitOfWork.SubscriptionsRepository.AllAsNoTracking.Where(s => s.OrgUserId == this.CurrentOrgUser.Id);
            var lastSubscription = subscriptions.Max(s => s.EndDate) ?? DateTimeService.UtcNow;

            for (var index = 0; index < plan.Length; index++)
            {
                var subscription = new Subscription
                {
                    IsActive = true,
                    Type = SubscriptionType.Paid,
                    StartDate = lastSubscription.AddMonths(index),
                    EndDate = lastSubscription.AddMonths(index).AddMonths(1),
                    Note = $"Paid subscription - {plan.Name}",
                    PaymentRecord = payment,
                    OrgUserId = this.CurrentOrgUser.Id,
                    SubscriptionPlanId = plan.Id,
                };

                UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);
            }

            try
            {
                this.CurrentOrgUser.IsSubscribed = true;
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
            if (string.IsNullOrEmpty(token))
                return BadRequest();

            var invitation = UnitOfWork.OrgInvitationsRepository.AllAsNoTracking
                .Where(x => x.Token == token)
                .SingleOrDefault();

            if (invitation == null)
                return NotFound();

            if (invitation.Used + 1 > invitation.Limit || !invitation.IsActive)
                return BadRequest("This invitation has been closed");

            var subscription = new Subscription
            {
                IsActive = true,
                Type = SubscriptionType.Organisation,
                StartDate = DateTimeService.UtcNow,
                EndDate = null,
                Note = $"Joined organisation - {invitation.Organisation.Name}",
                OrgUserId = this.CurrentOrgUser.Id
            };

            UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);

            invitation.Used += 1;
            UnitOfWork.OrgInvitationsRepository.InsertOrUpdate(invitation);

            // move this OrgUser and their personal case to the new organisation.
            var orgUser = this.UnitOfWork.OrgUsersRepository.Find(this.CurrentOrgUser.Id);

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
                    else
                    {
                        // the root user has already been assigned to this project.
                    }
                }
            }

            try
            {
                this.CurrentOrgUser.IsSubscribed = true;
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
