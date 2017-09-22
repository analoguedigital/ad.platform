using LightMethods.Survey.Models.Services;
using System.Net;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class PromotionCodesController : BaseApiController
    {
        private SubscriptionService SubscriptionService { get; set; }

        public PromotionCodesController()
        {
            this.SubscriptionService = new SubscriptionService(this.CurrentOrgUser, this.UnitOfWork);
        }

        [HttpPost]
        [Route("api/promotionCodes/redeem/{code}")]
        public IHttpActionResult Redeem(string code)
        {
            var result = this.SubscriptionService.RedeemCode(code);
            switch (result)
            {
                case SubscriptionService.RedeemCodeStatus.SubscriptionDisabled:
                    return Content(HttpStatusCode.Forbidden, "Subscriptions are disabled. Contact your administrator.");
                case SubscriptionService.RedeemCodeStatus.SubscriptionRateNotSet:
                    return Content(HttpStatusCode.Forbidden, "Subscription Rate is not set. Contact your administrator.");
                case SubscriptionService.RedeemCodeStatus.OK:
                    return Ok();
                default:
                    return NotFound();
            }   
        }
    }
}
