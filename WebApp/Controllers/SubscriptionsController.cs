using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class SubscriptionsController : BaseApiController
    {
        private SubscriptionService SubscriptionService { get; set; }

        public SubscriptionsController()
        {
            this.SubscriptionService = new SubscriptionService(this.CurrentOrgUser, this.UnitOfWork);
        }

        [Route("api/subscriptions")]
        [ResponseType(typeof(IEnumerable<SubscriptionDTO>))]
        public IHttpActionResult Get()
        {
            var subscriptions = this.SubscriptionService.GetUserSubscriptions();
            var result = subscriptions.Select(s => Mapper.Map<SubscriptionDTO>(s));

            return Ok(result);
        }

        [Route("api/subscriptions/getLatest")]
        [ResponseType(typeof(LatestSubscriptionDTO))]
        public IHttpActionResult GetLatest()
        {
            var latestSubscription = this.SubscriptionService.GetLatest();
            return Ok(new LatestSubscriptionDTO { Date = latestSubscription });
        }
    }
}
