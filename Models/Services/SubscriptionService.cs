using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
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

            //var data = this.UOW.SubscriptionsRepository.AllAsNoTracking
            //    .Where(x => x.OrgUserId == this.User.Id)
            //    .OrderByDescending(x => x.DateCreated)
            //    .GroupBy(x => x.Type)
            //    .ToList();

            //foreach (var item in data)
            //{
            //    var subs = item.GroupBy(x => x.PaymentRecord).ToList();
            //    var typeName = item.Key.ToString();
            //}

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
                if (lastSubscription.Type == UserSubscriptionType.Organisation)
                    return DateTimeService.UtcNow.AddMonths(1);
                else
                {
                    // paid plan or voucher.
                    return subscriptions.Max(s => s.EndDate);
                }
            }

            return null;
        }

        public SubscriptionDTO GetLastSubscription(Guid userId)
        {
            var subscription = this.UOW.SubscriptionsRepository.AllAsNoTracking
                .Where(x => x.OrgUserId == userId)
                .OrderByDescending(x => x.DateCreated)
                .Take(1)
                .ToList()
                .SingleOrDefault();

            return Mapper.Map<SubscriptionDTO>(subscription);
        }

        public bool HasValidSubscription(Guid userId)
        {
            var latestSubscription = this.GetLatest(userId);
            if (latestSubscription.HasValue && latestSubscription.Value > DateTimeService.UtcNow)
                return true;

            return false;
        }

        public bool HasAccessToExportZip(OrgUser orgUser, Guid projectId)
        {
            var project = this.UOW.ProjectsRepository.Find(projectId);
            var assignment = project.Assignments.SingleOrDefault(a => a.OrgUserId == orgUser.Id);
            if (assignment == null || !assignment.CanExportZip)
                return false;

            // mobile accounts need an active subscription.
            if (orgUser.AccountType == AccountType.MobileAccount)
            {
                var latestSubscription = this.GetLatest(orgUser.Id);
                if (latestSubscription == null)
                    return false;

                // determine if this user has access to export pdfs.
                var subscription = this.GetLastSubscription(orgUser.Id);
                if (subscription == null)
                    return false;

                // organization subscribers don't need access to export capabilities.
                // they are granted access by default. Check paid subscriptions only.
                if (subscription.Type == UserSubscriptionType.Paid && !subscription.SubscriptionPlan.ZipExport)
                    return false;
            }

            return true;
        }

        public bool HasAccessToExportPdf(OrgUser orgUser, Guid projectId)
        {
            var project = this.UOW.ProjectsRepository.Find(projectId);
            var assignment = project.Assignments.SingleOrDefault(a => a.OrgUserId == orgUser.Id);
            if (assignment == null || !assignment.CanExportPdf)
                return false;

            // mobile accounts need an active subscription.
            if (orgUser.AccountType == AccountType.MobileAccount)
            {
                var latestSubscription = this.GetLatest(orgUser.Id);
                if (latestSubscription == null)
                    return false;

                // determine if this user has access to export pdfs.
                var subscription = this.GetLastSubscription(orgUser.Id);
                if (subscription == null)
                    return false;

                // organization subscribers don't need access to export capabilities.
                // they are granted access by default. Check paid subscriptions only.
                if (subscription.Type == UserSubscriptionType.Paid && !subscription.SubscriptionPlan.PdfExport)
                    return false;
            }

            return true;
        }

        public RedeemCodeStatus RedeemCode(string code)
        {
            var voucher = this.UOW.VouchersRepository.AllAsNoTracking
                .Where(x => x.Code == code)
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

            this.AddVoucherSusbcriptions(payment);
            this.User.IsSubscribed = true;
            this.UOW.Save();

            return RedeemCodeStatus.OK;
        }

        private void AddVoucherSusbcriptions(PaymentRecord payment)
        {
            var monthlyRate = this.User.Organisation.SubscriptionMonthlyRate;
            var subscriptionCount = Math.Floor(payment.Amount / monthlyRate.Value);
            var latestSubscription = this.GetLatest() ?? DateTimeService.UtcNow;

            for (var index = 0; index < subscriptionCount; index++)
            {
                var subscription = new Subscription
                {
                    IsActive = true,
                    Type = UserSubscriptionType.Voucher,
                    StartDate = latestSubscription.AddMonths(index),
                    EndDate = latestSubscription.AddMonths(index).AddMonths(1),
                    Note = "Subscribed with a voucher",
                    PaymentRecord = payment,
                    OrgUserId = this.User.Id
                };
                this.UOW.SubscriptionsRepository.InsertOrUpdate(subscription);
            }
        }
    }
}
