using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Services
{

    public class SubscriptionEntryDTO
    {
        public Guid? Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? Price { get; set; }

        public string Note { get; set; }

        public string Reference { get; set; }

        public Guid? PaymentRecordId { get; set; }

        public UserSubscriptionType Type { get; set; }

        public bool IsActive { get; set; }

        public SubscriptionPlanDTO SubscriptionPlan { get; set; }

        public OrganisationDTO Organisation { get; set; }
    }

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
            SubscriptionCountLessThanOne,
            OK,
            Error
        }

        public SubscriptionService(OrgUser user, UnitOfWork uow)
        {
            this.User = user;
            this.UOW = uow;
        }

        public List<SubscriptionEntryDTO> GetUserSubscriptions(Guid? orgUserId)
        {
            var result = new List<SubscriptionEntryDTO>();
            var userId = orgUserId.HasValue ? orgUserId.Value : this.User.Id;

            var orgSubscriptions = this.UOW.SubscriptionsRepository.AllAsNoTracking
                .Where(x => x.OrgUserId == userId && x.Type == UserSubscriptionType.Organisation)
                .OrderByDescending(x => x.DateCreated)
                .ToList();

            var payments = this.UOW.PaymentsRepository.AllAsNoTracking
                .Where(x => x.OrgUserId == userId)
                .OrderByDescending(x => x.DateCreated)
                .ToList();

            foreach (var item in orgSubscriptions)
            {
                result.Add(new SubscriptionEntryDTO
                {
                    Id = item.Id,
                    Type = item.Type,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    Note = item.Note,
                    IsActive = item.IsActive,
                    Organisation = Mapper.Map<OrganisationDTO>(item.Organisation)
                });
            }

            foreach (var item in payments)
            {
                if (item.Subscriptions.Any())
                {
                    var entry = new SubscriptionEntryDTO();

                    var startDate = item.Subscriptions.Min(x => x.StartDate);
                    var endDate = item.Subscriptions.Max(x => x.EndDate);
                    var lastSubscription = item.Subscriptions.OrderByDescending(x => x.DateCreated).Take(1).ToList().SingleOrDefault();

                    entry.PaymentRecordId = item.Id;
                    entry.Type = lastSubscription.Type;
                    entry.StartDate = startDate;
                    entry.EndDate = endDate;
                    entry.Note = item.Note;
                    entry.Price = item.Amount;
                    entry.Reference = item.Reference;
                    entry.IsActive = lastSubscription.IsActive;
                    entry.SubscriptionPlan = Mapper.Map<SubscriptionPlanDTO>(lastSubscription.SubscriptionPlan);

                    result.Add(entry);
                }
            }

            result = result.OrderByDescending(x => x.StartDate).ToList();

            return result;
        }

        public DateTime? GetLatest()
        {
            return this.GetLatest(this.User.Id);
        }

        public DateTime? GetLatest(Guid userId)
        {
            var subscriptions = this.UOW.SubscriptionsRepository.AllAsNoTracking.Where(s => s.OrgUserId == userId && s.IsActive);
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
                .Where(x => x.OrgUserId == userId && x.IsActive)
                .OrderByDescending(x => x.DateCreated)
                .Take(1)
                .ToList()
                .SingleOrDefault();

            return Mapper.Map<SubscriptionDTO>(subscription);
        }

        public MonthlyQuotaDTO GetMonthlyQuota(Guid userId)
        {
            var result = new MonthlyQuotaDTO();

            var expiryDate = this.GetLatest(userId);
            var fixedQuota = Convert.ToInt32(ConfigurationManager.AppSettings["FixedMonthlyQuota"]);

            if (expiryDate == null)
            {
                // unsubscribed users have a fixed quota.
                result.Quota = fixedQuota;
            }
            else
            {
                var lastSubscription = this.GetLastSubscription(userId);
                switch (lastSubscription.Type)
                {
                    case UserSubscriptionType.Paid:
                        result.Quota = lastSubscription.SubscriptionPlan.IsLimited ? lastSubscription.SubscriptionPlan.MonthlyQuota : null;
                        break;
                    case UserSubscriptionType.Organisation:
                        result.Quota = null;
                        break;
                    case UserSubscriptionType.Voucher:
                        result.Quota = fixedQuota;
                        break;
                    default:
                        break;
                }
            }

            var lastMonth = DateTimeService.UtcNow.AddMonths(-1);
            var lastMonthRecords = this.UOW.FilledFormsRepository.AllAsNoTracking
                        .Count(x => x.FilledById == userId && x.DateCreated >= lastMonth);

            result.Used = lastMonthRecords;

            return result;
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

            // validate subscription count. it should result to at least 1.
            if (voucher.Period < 1)
                return RedeemCodeStatus.SubscriptionCountLessThanOne;

            var totalAmount = voucher.Period * this.User.Organisation.SubscriptionMonthlyRate.Value;

            // register payment record
            var payment = new PaymentRecord
            {
                Date = DateTimeService.UtcNow,
                Amount = totalAmount,
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

            for (var index = 0; index < voucher.Period; index++)
            {
                var subscription = new Subscription
                {
                    IsActive = true,
                    Type = UserSubscriptionType.Voucher,
                    StartDate = DateTimeService.UtcNow.AddMonths(index),
                    EndDate = DateTimeService.UtcNow.AddMonths(index).AddMonths(1),
                    Note = "Subscribed with a voucher",
                    PaymentRecord = payment,
                    OrgUserId = this.User.Id
                };
                this.UOW.SubscriptionsRepository.InsertOrUpdate(subscription);
            }

            this.User.IsSubscribed = true;

            // cancel last subscription, if any.
            var lastSubscription = this.UOW.SubscriptionsRepository.AllAsNoTracking
               .Where(x => x.OrgUserId == this.User.Id && x.IsActive)
               .OrderByDescending(x => x.DateCreated)
               .FirstOrDefault();

            if (lastSubscription != null)
            {
                if (lastSubscription.Type == UserSubscriptionType.Organisation)
                {
                    lastSubscription.EndDate = DateTimeService.UtcNow;
                    lastSubscription.IsActive = false;

                    this.UOW.SubscriptionsRepository.InsertOrUpdate(lastSubscription);
                }
                else
                {
                    var paymentRecord = this.UOW.PaymentsRepository.Find(lastSubscription.PaymentRecord.Id);
                    foreach (var record in paymentRecord.Subscriptions)
                    {
                        record.IsActive = false;
                    }

                    this.UOW.PaymentsRepository.InsertOrUpdate(paymentRecord);
                }
            }

            try
            {
                this.UOW.Save();
                return RedeemCodeStatus.OK;
            }
            catch (Exception)
            {
                return RedeemCodeStatus.Error;
            }
        }
    }

}
