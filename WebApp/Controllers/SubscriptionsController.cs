using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Enums;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Organisation user,Restricted user")]
    public class SubscriptionsController : BaseApiController
    {

        #region Properties

        private SubscriptionService SubscriptionService { get; set; }

        #endregion

        #region C-tor

        public SubscriptionsController()
        {
            SubscriptionService = new SubscriptionService(UnitOfWork);
        }

        #endregion

        // GET api/subscriptions
        [DeflateCompression]
        [Route("api/subscriptions")]
        [ResponseType(typeof(IEnumerable<SubscriptionEntryDTO>))]
        public IHttpActionResult Get()
        {
            var subscriptions = SubscriptionService.GetUserSubscriptions(CurrentOrgUser.Id);
            return Ok(subscriptions);
        }

        // GET api/subscriptions/user/{id}
        [DeflateCompression]
        [Route("api/subscriptions/user/{id:guid}")]
        [ResponseType(typeof(IEnumerable<SubscriptionEntryDTO>))]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Platform administrator,Organisation user")]
        public IHttpActionResult Get(Guid id)
        {
            var subscriptions = SubscriptionService.GetUserSubscriptions(id);
            return Ok(subscriptions);
        }

        // GET api/subscriptions/getLatest
        [DeflateCompression]
        [Route("api/subscriptions/getLatest")]
        [ResponseType(typeof(LatestSubscriptionDTO))]
        public IHttpActionResult GetLatest()
        {
            var latestSubscription = SubscriptionService.GetLatest(CurrentOrgUser);
            var result = new LatestSubscriptionDTO { Date = latestSubscription };

            return Ok(result);
        }

        // GET api/subscriptions/last
        [DeflateCompression]
        [Route("api/subscriptions/last")]
        [ResponseType(typeof(SubscriptionDTO))]
        public IHttpActionResult GetLastSubscription()
        {
            var lastSubscription = SubscriptionService.GetLastSubscription(CurrentOrgUser.Id);
            return Ok(lastSubscription);
        }

        // POST api/subscriptions/buy/{id}
        [HttpPost]
        [Route("api/subscriptions/buy/{id:guid}")]
        public IHttpActionResult Buy(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var plan = UnitOfWork.SubscriptionPlansRepository.Find(id);
            if (plan == null)
                return NotFound();

            if (CurrentOrgUser.AccountType != AccountType.MobileAccount)
                return BadRequest("paid plans are only available to mobile users");

            SubscriptionService.PurchaseSubscription(plan, CurrentOrgUser);

            try
            {
                NotifyUserAboutPurchase(plan);
                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST api/subscriptions/joinOrganisation/{token}
        [HttpPost]
        [Route("api/subscriptions/joinorganisation/{token}")]
        public IHttpActionResult JoinOrganisation(string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("invitation token is empty");

            var invitation = UnitOfWork.OrgInvitationsRepository
                .AllAsNoTracking
                .Where(x => x.Token == token)
                .SingleOrDefault();

            if (invitation == null)
                return NotFound();

            if (invitation.Used + 1 > invitation.Limit || !invitation.IsActive)
                return BadRequest("invitation token has been closed");

            var orgUser = UnitOfWork.OrgUsersRepository.Find(CurrentOrgUser.Id);

            if (orgUser.Organisation.Id == invitation.Organisation.Id)
                return BadRequest("you are already connected to this organization");

            SubscriptionService.MoveUserToOrganization(invitation.Organisation, orgUser);

            invitation.Used += 1;
            UnitOfWork.OrgInvitationsRepository.InsertOrUpdate(invitation);

            try
            {
                NotifyUserAboutOrganisationSubscription(invitation.Organisation.Name);
                NotifyOrgAdminAboutNewSubscriber(invitation.Organisation);

                UnitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/subscriptions/quota
        [Route("api/subscriptions/quota")]
        public IHttpActionResult GetQuota()
        {
            // OrgAdmins and WebAccounts don't need an active subscription.
            // the UI shouldn't query this data, and if requested directly
            // from the API we should return OK or another data structure.
            if (CurrentOrgUser.AccountType != AccountType.MobileAccount)
                return Ok();

            var quota = SubscriptionService.GetMonthlyQuota(CurrentOrgUser.Id);
            return Ok(quota);
        }

        #region Admin Methods

        // POST api/subscriptions/close
        [HttpPost]
        [Route("api/subscriptions/close")]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Platform administrator")]
        public IHttpActionResult CloseSubscription([FromBody] UpdateSubscriptionEntryDTO value)
        {
            if (value.Type == UserSubscriptionType.Organisation)
            {
                var subscription = UnitOfWork.SubscriptionsRepository.Find(value.RecordId);
                if (subscription == null)
                    return NotFound();

                if (!subscription.IsActive)
                    return BadRequest("Subscription entry already closed");

                subscription.EndDate = DateTimeService.UtcNow;
                subscription.IsActive = false;

                UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);
            }
            else
            {
                var payment = UnitOfWork.PaymentsRepository.Find(value.RecordId);
                if (payment == null)
                    return NotFound();

                if (!payment.Subscriptions.Any(x => x.IsActive == true))
                    return BadRequest("Subscription entry already closed");

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

        // POST api/subscriptions/remove
        [HttpPost]
        [Route("api/subscriptions/remove")]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Platform administrator")]
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

        // POST api/subscriptions/joinOnRecord/{userId}
        [HttpPost]
        [Route("api/subscriptions/joinOnRecord/{userId:guid}")]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Platform administrator")]
        public IHttpActionResult JoinOnRecord(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("user id is empty");

            var orgUser = UnitOfWork.OrgUsersRepository.Find(userId);
            if (orgUser == null)
                return NotFound();

            if (orgUser.AccountType != AccountType.MobileAccount)
                return BadRequest("only mobile users can join OnRecord");

            SubscriptionService.SubscribeUserToOnRecord(orgUser);

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

        #endregion Admin Methods

        #region Helpers

        private void NotifyUserAboutPurchase(SubscriptionPlan plan)
        {
            var content = @"<p>Subscription purchased: <strong>" + plan.Name + @"</strong></p>
                            <p>Description: " + plan.Description + @"</p>
                            <p>Price: " + plan.Price + @" GBP</p>
                            <p>Length: " + plan.Length + @" month(s)</p>
                            <p>Is Limited: " + plan.IsLimited + @"</p>
                            <p>Monthly Quota: " + plan.MonthlyQuota.ToString() + @"</p>
                            <p>PDF Export: " + plan.PdfExport + @"</p>
                            <p>Zip Export: " + plan.ZipExport + @"</p>";

            var email = new Email
            {
                To = CurrentOrgUser.Email,
                Subject = $"You have purchased a subscription as follows - {plan.Name}",
                Content = WebHelpers.GenerateEmailTemplate(content, "Subscription Purchased")
            };

            UnitOfWork.EmailsRepository.InsertOrUpdate(email);
        }

        private void NotifyUserAboutOrganisationSubscription(string organisationName)
        {
            var content = @"<p>You have joined <strong>" + organisationName + @"</strong>.</p>
                            <p>Your records are now available to " + organisationName + @".</p>
                            <p>You can end their access to your records at any time by going to the mobile app, then going to the Connect to an Organisation section and unlinking using the Unlink button at the bottom of the screen. Your records will still be safely stored and accessible to you as usual.</p>";

            var email = new Email
            {
                To = CurrentOrgUser.Email,
                Subject = $"You have joined {organisationName}",
                Content = WebHelpers.GenerateEmailTemplate(content, "You have successfully joined an organisation")
            };

            UnitOfWork.EmailsRepository.InsertOrUpdate(email);
        }

        private void NotifyOrgAdminAboutNewSubscriber(Organisation organisation)
        {
            if (organisation.RootUser != null)
            {
                var orgAdmin = UnitOfWork.OrgUsersRepository
                    .AllAsNoTracking
                    .Where(x => x.Id == organisation.RootUser.Id)
                    .SingleOrDefault();

                // generate the email body.
                var rootIndex = WebHelpers.GetRootIndexPath();
                var url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/{rootIndex}#!/users/mobile/";

                var emailBody = @"<p>A new client, <strong>" + CurrentOrgUser.UserName + @"</strong>, has joined your organisation and their records are now available to you.</p>
                            <p>You can end your access to any client's records by logging into the OnRecord platform, going to 'Organisation Management' and then 'Clients', finding their user name and clicking on 'Remove'.</p>
                            <p><br></p>
                            <p>Go to <a href='" + url + @"'>Clients</a> in the Organisation Management section of the platform menu.</p>";

                var orgAdminEmail = new Email
                {
                    To = orgAdmin.Email,
                    Subject = $"A new client, {CurrentOrgUser.UserName}, has joined",
                    Content = WebHelpers.GenerateEmailTemplate(emailBody, "A New Client Has Joined Your Organisation")
                };

                UnitOfWork.EmailsRepository.InsertOrUpdate(orgAdminEmail);
            }
        }

        #endregion

    }
}
