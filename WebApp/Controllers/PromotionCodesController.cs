using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
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
        PromotionCodesRepository PromotionCodes { get { return UnitOfWork.PromotionCodesRepository; } }
        SubscriptionsRepository Subscriptions { get { return UnitOfWork.SubscriptionsRepository; } }
        PaymentsRepository Payments { get { return UnitOfWork.PaymentsRepository; } }

        [HttpPost]
        [Route("api/promotionCodes/redeemCode/{userId:guid}/{code}")]
        [ResponseType(typeof(RedeemCodeResponse))]
        public IHttpActionResult RedeemCode(Guid userId, string code)
        {
            var promotionCode = this.PromotionCodes.AllAsNoTracking
                .Where(pc => pc.Code == code)
                .SingleOrDefault();

            if (promotionCode == null)
                return Ok(new RedeemCodeResponse { Success = false, Message = "Promotion Code Not Found!" });

            if (promotionCode.IsRedeemed)
                return Ok(new RedeemCodeResponse { Success = false, Message = "This code has already been redeemed!" });

            // register payment record
            var payment = new PaymentRecord
            {
                Date = DateTime.Now,
                Amount = promotionCode.Amount,
                Note = "Promotion Code Redeemed",
                PromotionCode = promotionCode,
                Reference = string.Empty,
                OrgUserId = userId
            };
            this.Payments.InsertOrUpdate(payment);

            // register subscription
            var subscription = new Subscription
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Note = "Subscription added with Promo. Code",
                PaymentRecord = payment,
                OrgUserId = userId
            };
            this.Subscriptions.InsertOrUpdate(subscription);

            // update promotion code
            promotionCode.IsRedeemed = true;
            promotionCode.PaymentRecordId = payment.Id;
            this.PromotionCodes.InsertOrUpdate(promotionCode);

            this.UnitOfWork.Save();

            return Ok(new RedeemCodeResponse { Success = true, Message = "Promotion Code Redeemed Successfully." });
        }
    }

    public class RedeemCodeResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
