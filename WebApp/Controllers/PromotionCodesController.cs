using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
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
            if (result == SubscriptionService.RedeemCodeStatus.OK)
                return Ok();

            return NotFound();
        }
    }
}
