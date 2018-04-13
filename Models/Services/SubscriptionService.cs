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
            var subscriptions = this.UOW.SubscriptionsRepository.AllAsNoTracking.Where(s => s.OrgUserId == this.User.Id);
            return subscriptions.ToList();
        }

        public DateTime? GetLatest()
        {
            return this.GetLatest(this.User.Id);
        }

        public DateTime? GetLatest(Guid userId)
        {
            var subscriptions = this.UOW.SubscriptionsRepository.AllAsNoTracking.Where(s => s.OrgUserId == userId);
            if (subscriptions.Any())
            {
                var lastSubscription = subscriptions.OrderByDescending(x => x.DateCreated).Take(1).SingleOrDefault();
                if (lastSubscription.Type == SubscriptionType.Paid)
                    return subscriptions.Max(s => s.EndDate);
                else if (lastSubscription.Type == SubscriptionType.Organisation)
                    return DateTimeService.UtcNow.AddMonths(1);
            }

            return null;
        }

        public bool HasValidSubscription(Guid userId)
        {
            var latestSubscription = this.GetLatest(userId);
            if (latestSubscription.HasValue && latestSubscription.Value > DateTimeService.UtcNow)
                return true;

            return false;
        }

        public RedeemCodeStatus RedeemCode(string code)
        {
            var voucher = this.UOW.VouchersRepository.AllAsNoTracking
                .Where(pc => pc.OrganisationId == this.User.OrganisationId && pc.Code == code)
                .SingleOrDefault();

            if (voucher == null)
                return RedeemCodeStatus.NotFound;
            if (voucher.IsRedeemed)
                return RedeemCodeStatus.AlreadyRedeemed;

            if (!this.User.Organisation.SubscriptionEnabled)
                return RedeemCodeStatus.SubscriptionDisabled;

            if (!this.User.Organisation.SubscriptionMonthlyRate.HasValue)
                return RedeemCodeStatus.SubscriptionRateNotSet;

            // register payment record
            var payment = new PaymentRecord
            {
                Date = DateTimeService.UtcNow,
                Amount = voucher.Amount,
                Note = $"Redeemed Voucher - {voucher.Code}",
                Reference = string.Empty,
                Voucher = voucher,
                OrgUserId = this.User.Id
            };
            this.UOW.PaymentsRepository.InsertOrUpdate(payment);

            // update promotion code
            voucher.IsRedeemed = true;
            voucher.PaymentRecordId = payment.Id;
            this.UOW.VouchersRepository.InsertOrUpdate(voucher);

            this.AddSusbcriptions(payment);
            this.User.IsSubscribed = true;
            this.UOW.Save();

            return RedeemCodeStatus.OK;
        }

        private void AddSusbcriptions(PaymentRecord payment)
        {
            var monthlyRate = this.User.Organisation.SubscriptionMonthlyRate;
            var subscriptionCount = Math.Floor(payment.Amount / monthlyRate.Value);
            var latestSubscription = this.GetLatest() ?? DateTimeService.UtcNow;

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
