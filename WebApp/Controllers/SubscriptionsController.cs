using AutoMapper;
using LightMethods.Survey.Models.DAL;
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
        SubscriptionsRepository Subscriptions { get { return UnitOfWork.SubscriptionsRepository; } }

        [Route("api/subscriptions/{userId:guid}")]
        [ResponseType(typeof(IEnumerable<SubscriptionDTO>))]
        public IHttpActionResult Get(Guid userId)
        {
            var subscriptions = this.Subscriptions.AllAsNoTracking
                .Where(s => s.OrgUserId == userId)
                .ToList();
            var result = subscriptions.Select(s => Mapper.Map<SubscriptionDTO>(s));

            return Ok(result);
        }

        [Route("api/subscriptions/getExpiry/{userId:guid}")]
        [ResponseType(typeof(SubscriptionExpiryDTO))]
        public IHttpActionResult GetExpiry(Guid userId)
        {
            var result = new SubscriptionExpiryDTO();
            var subscriptions = this.Subscriptions.AllAsNoTracking
                .Where(s => s.OrgUserId == userId)
                .ToList();

            var lastSubscription = subscriptions.LastOrDefault();
            if (lastSubscription != null)
                result.ExpiryDate = lastSubscription.EndDate;

            return Ok(result);
        }
    }
}
