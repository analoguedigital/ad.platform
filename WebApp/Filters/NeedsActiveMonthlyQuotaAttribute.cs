using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Enums;
using LightMethods.Survey.Models.Services;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApi.Filters
{
    public class NeedsActiveMonthlyQuotaAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var unitOfWork = ServiceContext.UnitOfWork;
            var currentUser = ServiceContext.CurrentUser;

            if (currentUser is OrgUser && ((OrgUser)currentUser).AccountType == AccountType.MobileAccount)
            {
                var subscriptionService = new SubscriptionService(unitOfWork);
                var expiryDate = subscriptionService.GetLatest(currentUser.Id);

                var fixedQuota = GetFixedMonthlyQuota();
                //var lastMonth = DateTimeService.UtcNow.AddMonths(-1);
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastMonthRecords = unitOfWork.FilledFormsRepository
                    .AllAsNoTracking
                    .Count(x => x.FilledById == currentUser.Id && x.DateCreated >= startDate);

                if (expiryDate == null)
                {
                    // unsubscribed user. check fixed quota and if exceeded, return unauthorized.
                    if (lastMonthRecords >= fixedQuota)
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                }
                else
                {
                    var subscription = subscriptionService.GetLastSubscription(currentUser.Id);
                    if (subscription.Type == UserSubscriptionType.Paid && subscription.SubscriptionPlan.IsLimited)
                    {
                        var quota = subscription.SubscriptionPlan.MonthlyQuota.Value;
                        if (lastMonthRecords >= quota)
                            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                    }

                    if (subscription.Type == UserSubscriptionType.Voucher)
                    {
                        // check fixed quota.
                        if (lastMonthRecords >= fixedQuota)
                            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                    }
                }
            }

            base.OnActionExecuting(actionContext);
        }

        private int GetFixedMonthlyQuota()
        {
            var quota = ConfigurationManager.AppSettings["FixedMonthlyQuota"];
            if (!string.IsNullOrEmpty(quota))
                return int.Parse(quota);

            return 150;  // default hard-coded value
        }
    }
}