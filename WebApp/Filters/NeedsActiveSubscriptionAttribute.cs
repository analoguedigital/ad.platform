using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApi.Filters
{
    public class NeedsActiveSubscriptionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var unitOfWork = ServiceContext.UnitOfWork;
            var currentUser = ServiceContext.CurrentUser;

            if (currentUser is OrgUser && ((OrgUser)currentUser).AccountType == AccountType.MobileAccount)
            {
                var subscriptionService = new SubscriptionService(unitOfWork);
                var latestSubscription = subscriptionService.GetLatest(currentUser.Id);
                if (latestSubscription == null)
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            }

            base.OnActionExecuting(actionContext);
        }
    }
}