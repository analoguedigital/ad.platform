using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Services
{
    public class SubscriptionService
    {
        public OrgUser User { set; get; }
        private UnitOfWork UOW { set; get; }
        public enum RedeemCodeStatus
        {
            NotFound,
            AlreadyRedeemed,
            SubscriptionDisabled,
            SubscriptionRateNotSet,
            OK
        }

        public SubscriptionService(OrgUser user, UnitOfWork uow)
        {
            this.User = user;
            this.UOW = uow;
        }

        public IEnumerable<Subscription> GetUserSubscriptions()
        {
            var subscriptions = this.UOW.SubscriptionsRepository.AllAsNoTracking
                .Where(s => s.OrgUserId == this.User.Id);
            return subscriptions.ToList();
        }

        public DateTime? GetLatest()
        {
            return this.GetLatest(this.User.Id);
        }

        public DateTime? GetLatest(Guid userId)
        {
            var subscriptions = this.UOW.SubscriptionsRepository.AllAsNoTracking
                .Where(s => s.OrgUserId == userId);

            if (subscriptions.Any())
                return subscriptions.Max(s => s.EndDate);

            return null;
        }

        public bool HasValidSubscription(Guid userId)
        {
            var latestSubscription = this.GetLatest(userId);
            if (latestSubscription.HasValue && latestSubscription.Value > DateTime.Now)
                return true;

            return false;
        }

        public RedeemCodeStatus RedeemCode(string code)
        {
            var promotionCode = this.UOW.PromotionCodesRepository.AllAsNoTracking
                .Where(pc => pc.OrganisationId == this.User.OrganisationId && pc.Code == code)
                .SingleOrDefault();

            if (promotionCode == null)
                return RedeemCodeStatus.NotFound;
            if (promotionCode.IsRedeemed)
                return RedeemCodeStatus.AlreadyRedeemed;

            if (!this.User.Organisation.SubscriptionEnabled)
                return RedeemCodeStatus.SubscriptionDisabled;

            if (!this.User.Organisation.SubscriptionMonthlyRate.HasValue)
                return RedeemCodeStatus.SubscriptionRateNotSet;

            // register payment record
            var payment = new PaymentRecord
            {
                Date = DateTime.Now,
                Amount = promotionCode.Amount,
                Note = "Promotion Code Redeemed",
                PromotionCode = promotionCode,
                Reference = string.Empty,
                OrgUserId = this.User.Id
            };
            this.UOW.PaymentsRepository.InsertOrUpdate(payment);

            // update promotion code
            promotionCode.IsRedeemed = true;
            promotionCode.PaymentRecordId = payment.Id;
            this.UOW.PromotionCodesRepository.InsertOrUpdate(promotionCode);

            this.AddSusbcriptions(payment);
            this.UOW.Save();

            return RedeemCodeStatus.OK;
        }

        private void AddSusbcriptions(PaymentRecord payment)
        {
            var monthlyRate = this.User.Organisation.SubscriptionMonthlyRate;
            var subscriptionCount = Math.Floor(payment.Amount / monthlyRate.Value);
            var latestSubscription = this.GetLatest() ?? DateTime.Now;

            for (var index = 0; index < subscriptionCount; index++)
            {
                var subscription = new Subscription
                {
                    StartDate = latestSubscription.AddMonths(index),
                    EndDate = latestSubscription.AddMonths(index).AddMonths(1),
                    Note = "Subscribed with promotion code",
                    PaymentRecord = payment,
                    OrgUserId = this.User.Id
                };
                this.UOW.SubscriptionsRepository.InsertOrUpdate(subscription);
            }
        }
    }
}
