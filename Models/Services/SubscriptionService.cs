using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LightMethods.Survey.Models.Services
{
    public class SubscriptionService
    {

        private UnitOfWork UnitOfWork { get; set; }

        public SubscriptionService(UnitOfWork uow)
        {
            UnitOfWork = uow;
        }

        #region User Subscriptions

        public DateTime? GetLatest(OrgUser orgUser)
        {
            return GetLatest(orgUser.Id);
        }

        public DateTime? GetLatest(Guid userId)
        {
            var subscriptions = UnitOfWork.SubscriptionsRepository
                .AllAsNoTracking
                .Where(s => s.OrgUserId == userId && s.IsActive);

            if (subscriptions.Any())
            {
                var lastSubscription = subscriptions.OrderByDescending(x => x.DateCreated)
                    .Take(1)
                    .SingleOrDefault();

                if (lastSubscription.Type == UserSubscriptionType.Organisation)
                    return DateTimeService.UtcNow.AddMonths(1);
                else
                    return subscriptions.Max(s => s.EndDate);   // paid plan or voucher.
            }

            return null;
        }

        public List<SubscriptionEntryDTO> GetUserSubscriptions(Guid userId)
        {
            var result = new List<SubscriptionEntryDTO>();

            var orgSubscriptions = UnitOfWork.SubscriptionsRepository
                .AllAsNoTracking
                .Where(x => x.OrgUserId == userId && x.Type == UserSubscriptionType.Organisation)
                .OrderByDescending(x => x.DateCreated)
                .ToList();

            var payments = UnitOfWork.PaymentsRepository
                .AllAsNoTracking
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

        public SubscriptionDTO GetLastSubscription(Guid userId)
        {
            var subscription = UnitOfWork.SubscriptionsRepository
                .AllAsNoTracking
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

            var expiryDate = GetLatest(userId);
            var fixedQuota = Convert.ToInt32(ConfigurationManager.AppSettings["FixedMonthlyQuota"]);

            var fixedDiskSpace = GetFixedMonthlyDiskSpace();
            var statsService = new StatisticsService(UnitOfWork);
            var usedSpace = statsService.GetUsedSpace(userId);

            if (expiryDate == null)
            {
                // unsubscribed users have a fixed quota.
                result.Quota = fixedQuota;
                result.MaxDiskSpace = fixedDiskSpace;
            }
            else
            {
                var lastSubscription = GetLastSubscription(userId);
                switch (lastSubscription.Type)
                {
                    case UserSubscriptionType.Paid:
                        result.Quota = lastSubscription.SubscriptionPlan.IsLimited ? lastSubscription.SubscriptionPlan.MonthlyQuota : null;
                        result.MaxDiskSpace = lastSubscription.SubscriptionPlan.IsLimited ? lastSubscription.SubscriptionPlan.MonthlyDiskSpace : null;
                        break;
                    case UserSubscriptionType.Organisation:
                        result.Quota = null;
                        result.MaxDiskSpace = null;
                        break;
                    case UserSubscriptionType.Voucher:
                        result.Quota = fixedQuota;
                        result.MaxDiskSpace = fixedDiskSpace;
                        break;
                    default:
                        break;
                }
            }

            //var lastMonth = DateTimeService.UtcNow.AddMonths(-1);
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastMonthRecords = UnitOfWork.FilledFormsRepository
                .AllAsNoTracking
                .Count(x => x.FilledById == userId && x.DateCreated >= startDate);

            result.Used = lastMonthRecords;
            result.UsedDiskSpace = (int)usedSpace.TotalSizeInKiloBytes;

            return result;
        }

        public RedeemCodeStatus RedeemCode(string code, OrgUser orgUser)
        {
            var voucher = UnitOfWork.VouchersRepository
                .AllAsNoTracking
                .Where(x => x.Code == code)
                .SingleOrDefault();

            if (voucher == null)
                return RedeemCodeStatus.NotFound;
            if (voucher.IsRedeemed)
                return RedeemCodeStatus.AlreadyRedeemed;
            if (!orgUser.Organisation.SubscriptionEnabled)
                return RedeemCodeStatus.SubscriptionDisabled;
            if (!orgUser.Organisation.SubscriptionMonthlyRate.HasValue)
                return RedeemCodeStatus.SubscriptionRateNotSet;

            // validate subscription count. it should result to at least 1.
            if (voucher.Period < 1)
                return RedeemCodeStatus.SubscriptionCountLessThanOne;

            var monthlyRate = orgUser.Organisation.SubscriptionMonthlyRate.Value;
            var totalAmount = (voucher.Period * monthlyRate);

            // register payment record
            var payment = new PaymentRecord
            {
                Date = DateTimeService.UtcNow,
                Amount = totalAmount,
                Note = $"Redeemed Voucher - {voucher.Code}",
                Reference = string.Empty,
                Voucher = voucher,
                OrgUserId = orgUser.Id
            };

            UnitOfWork.PaymentsRepository.InsertOrUpdate(payment);

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
                    OrgUserId = orgUser.Id
                };

                UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);
            }

            // update promotion code
            voucher.IsRedeemed = true;
            voucher.PaymentRecordId = payment.Id;

            UnitOfWork.VouchersRepository.InsertOrUpdate(voucher);
            orgUser.IsSubscribed = true;

            // cancel last subscription, if any.
            CloseLastSubscription(orgUser);

            try
            {
                UnitOfWork.Save();
                return RedeemCodeStatus.OK;
            }
            catch (Exception)
            {
                return RedeemCodeStatus.Error;
            }
        }

        #endregion

        #region Organization Management

        public void MoveUsersToOrganisation(Organisation organisation, List<Guid> orgUsers)
        {
            if (orgUsers.Any())
            {
                foreach (var id in orgUsers)
                {
                    var orgUser = UnitOfWork.OrgUsersRepository.Find(id);
                    MoveUserToOrganization(organisation, orgUser);
                }
            }
        }

        public void MoveUserToOrganization(Organisation organisation, OrgUser orgUser)
        {
            // remove this user from any teams in the current organisation.
            RemoveUserFromCurrentTeams(orgUser);

            // remove this user from any assignments in the current organization.
            RemoveUserFromCurrentAssignments(orgUser);

            // remove the assignment from current organisation's root user
            // NOTE: this could be removed too. we're not assigning root users
            // so there's no need to unassign them. doesn't hurt to confirm though.
            UnassignRootUserFromProject(orgUser, orgUser.Organisation);

            // remove any staff/client assignments from current organization
            UnassignStaffAndClientsFromProject(orgUser, orgUser.Organisation);

            // move the user and their personal case to the new organisation.
            MoveUserCaseToOrganization(orgUser, organisation);

            // assign Org root user to the project
            // NOTE: not necessary, because OrgAdmins don't need assignments.
            //AssignRootUserToProject(orgUser.CurrentProjectId.Value, organisation.RootUserId.Value);

            // close last subscription
            CloseLastSubscription(orgUser);

            // grant export access to the user
            GrantExportCapabilities(orgUser);

            // subscribe to the new organization
            SubscribeUserToOrganization(orgUser, organisation);

            orgUser.IsSubscribed = true;
        }

        public void RemoveUserFromOrganization(Organisation organisation, OrgUser orgUser)
        {
            var OnRecord = UnitOfWork.OrganisationRepository
                .AllAsNoTracking
                .Where(x => x.Name == "OnRecord")
                .FirstOrDefault();

            // remove user from any teams in current organisation.
            RemoveUserFromCurrentTeams(orgUser);

            // remove this user from any assignments in the current organization.
            RemoveUserFromCurrentAssignments(orgUser);

            // remove the assignment from current organisation's root user
            UnassignRootUserFromProject(orgUser, organisation);

            // remove any staff/client assignments from current organization
            UnassignStaffAndClientsFromProject(orgUser, organisation);

            // assign OnRecord admin to user's project
            // NOTE: not necessary, since OrgAdmins don't need assignments.
            //if (OnRecord.RootUser != null)
            //    AssignRootUserToProject(orgUser.CurrentProjectId.Value, OnRecord.RootUser.Id);

            // move user back to OnRecord
            MoveUserCaseToOrganization(orgUser, OnRecord);

            // close the last organisation subscription.
            CloseLastOrganisationSubscription(orgUser);

            // subscribe user to OnRecord again.
            SubscribeUserToOrganization(orgUser, OnRecord);

            // NOTE: I think we should disable export permissions here.
            // and also set IsSubscribed = false.
        }

        #endregion

        #region Helpers

        public void RemoveUserFromCurrentAssignments(OrgUser user)
        {
            var assignments = UnitOfWork.AssignmentsRepository
                .AllAsNoTracking
                .Where(x => x.OrgUserId == user.Id && x.ProjectId != user.CurrentProjectId)
                .Where(x => x.Project.OrganisationId == user.OrganisationId)
                .ToList();

            UnitOfWork.AssignmentsRepository.Delete(assignments);
        }

        public void UnassignRootUserFromProject(OrgUser user, Organisation organisation)
        {
            if (user.CurrentProject != null && user.CurrentProject.CreatedById == user.Id)
            {
                // remove org root admin from this project
                var rootUserAssignment = UnitOfWork.AssignmentsRepository
                    .AllAsNoTracking
                    .Where(x => x.OrgUserId == organisation.RootUserId && x.ProjectId == user.CurrentProject.Id)
                    .SingleOrDefault();

                if (rootUserAssignment != null)
                    UnitOfWork.AssignmentsRepository.Delete(rootUserAssignment);
            }
        }

        public void UnassignStaffAndClientsFromProject(OrgUser user, Organisation organisation)
        {
            if (user.CurrentProject != null && user.CurrentProject.CreatedById == user.Id)
            {
                var assignments = UnitOfWork.AssignmentsRepository
                    .AllAsNoTracking
                    .Where(x => x.ProjectId == user.CurrentProject.Id)
                    .Where(x => x.OrgUserId != user.Id)
                    .Where(x => !x.OrgUser.IsRootUser)
                    .Where(x => x.OrgUser.Organisation.Id == organisation.Id)
                    .ToList();

                UnitOfWork.AssignmentsRepository.Delete(assignments);
            }
        }

        public void AssignRootUserToProject(Guid projectId, Guid rootUserId)
        {
            var assignment = UnitOfWork.AssignmentsRepository
                .AllAsNoTracking
                .Where(x => x.ProjectId == projectId && x.OrgUserId == rootUserId)
                .SingleOrDefault();

            if (assignment == null)
            {
                var rootUserAssignment = new Assignment
                {
                    ProjectId = projectId,
                    OrgUserId = rootUserId,
                    CanView = true,
                    CanAdd = true,
                    CanEdit = true,
                    CanDelete = true,
                    CanExportPdf = true,
                    CanExportZip = true
                };

                UnitOfWork.AssignmentsRepository.InsertOrUpdate(rootUserAssignment);
            }
        }

        public void RemoveUserFromCurrentTeams(OrgUser user)
        {
            // remove this user from any teams in current organisation.
            var teamRecords = UnitOfWork.OrgTeamUsersRepository
                .AllAsNoTracking
                .Where(x => x.OrgUserId == user.Id && x.OrganisationTeam.OrganisationId == user.OrganisationId)
                .ToList();

            foreach (var record in teamRecords)
                UnitOfWork.OrgTeamUsersRepository.Delete(record);
        }

        public void MoveUserCaseToOrganization(OrgUser user, Organisation organisation)
        {
            // update user's organisation
            user.OrganisationId = organisation.Id;

            // update user's current project, if exists
            if (user.CurrentProject != null && user.CurrentProject.CreatedById == user.Id)
            {
                var project = UnitOfWork.ProjectsRepository.Find(user.CurrentProject.Id);
                project.OrganisationId = organisation.Id;    // update project's organisation

                // update threads under this project
                var threads = UnitOfWork.FormTemplatesRepository
                    .AllAsNoTracking
                    .Where(t => t.ProjectId == project.Id)
                    .ToList();

                // update form templates' organisation
                foreach (var form in threads)
                {
                    form.OrganisationId = organisation.Id;
                    UnitOfWork.FormTemplatesRepository.InsertOrUpdate(form);
                }
            }
        }

        public void CloseLastSubscription(OrgUser user)
        {
            // cancel last subscription, if any.
            var lastSubscription = UnitOfWork.SubscriptionsRepository.AllAsNoTracking
               .Where(x => x.OrgUserId == user.Id && x.IsActive)
               .OrderByDescending(x => x.DateCreated)
               .FirstOrDefault();

            if (lastSubscription != null)
            {
                if (lastSubscription.Type == UserSubscriptionType.Organisation)
                {
                    lastSubscription.EndDate = DateTimeService.UtcNow;
                    lastSubscription.IsActive = false;
                    UnitOfWork.SubscriptionsRepository.InsertOrUpdate(lastSubscription);
                }
                else
                {
                    var paymentRecord = UnitOfWork.PaymentsRepository.Find(lastSubscription.PaymentRecord.Id);
                    foreach (var record in paymentRecord.Subscriptions)
                        record.IsActive = false;

                    UnitOfWork.PaymentsRepository.InsertOrUpdate(paymentRecord);
                }
            }
        }

        public void CloseLastOrganisationSubscription(OrgUser user)
        {
            var subscription = UnitOfWork.SubscriptionsRepository
               .AllAsNoTracking
               .Where(x => x.OrgUserId == user.Id && x.Type == UserSubscriptionType.Organisation && x.IsActive)
               .OrderByDescending(x => x.DateCreated)
               .FirstOrDefault();

            if (subscription != null)
            {
                subscription.EndDate = DateTimeService.UtcNow;
                subscription.IsActive = false;
                UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);
            }
        }

        public void GrantExportCapabilities(OrgUser user)
        {
            if (user.CurrentProjectId.HasValue)
            {
                var orgUserAssignment = user.Assignments
                    .Where(x => x.ProjectId == user.CurrentProject.Id)
                    .SingleOrDefault();

                if (orgUserAssignment != null)
                {
                    orgUserAssignment.CanExportPdf = true;
                    orgUserAssignment.CanExportZip = true;
                    UnitOfWork.AssignmentsRepository.InsertOrUpdate(orgUserAssignment);
                }
            }
        }

        public void SubscribeUserToOrganization(OrgUser user, Organisation organisation)
        {
            var subscription = new Subscription
            {
                IsActive = true,
                Type = UserSubscriptionType.Organisation,
                StartDate = DateTimeService.UtcNow,
                EndDate = null,
                Note = $"Joined organisation - {organisation.Name}",
                OrgUserId = user.Id,
                OrganisationId = organisation.Id
            };

            UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);
        }

        public void SubscribeUserToOnRecord(OrgUser user)
        {
            var OnRecord = UnitOfWork.OrganisationRepository.AllAsNoTracking
                .Where(x => x.Name == "OnRecord")
                .SingleOrDefault();

            SubscribeUserToOrganization(user, OnRecord);
        }

        public void PurchaseSubscription(SubscriptionPlan plan, OrgUser user)
        {
            var payment = new PaymentRecord
            {
                Date = DateTimeService.UtcNow,
                Amount = plan.Price,
                Note = $"Purchased subscription - {plan.Name}",
                Reference = string.Empty,
                OrgUserId = user.Id
            };

            UnitOfWork.PaymentsRepository.InsertOrUpdate(payment);

            var startDate = DateTimeService.UtcNow;
            for (var index = 0; index < plan.Length; index++)
            {
                var subscription = new Subscription
                {
                    IsActive = true,
                    Type = UserSubscriptionType.Paid,
                    StartDate = startDate.AddMonths(index),
                    EndDate = startDate.AddMonths(index).AddMonths(1),
                    Note = $"Paid subscription - {plan.Name}",
                    PaymentRecord = payment,
                    OrgUserId = user.Id,
                    SubscriptionPlanId = plan.Id,
                };

                UnitOfWork.SubscriptionsRepository.InsertOrUpdate(subscription);
            }

            user.IsSubscribed = true;

            GrantExportCapabilities(user);
            CloseLastSubscription(user);
        }

        public bool HasValidSubscription(Guid userId)
        {
            var latestSubscription = GetLatest(userId);
            if (latestSubscription.HasValue && latestSubscription.Value > DateTimeService.UtcNow)
                return true;

            return false;
        }

        public bool HasAccessToExportZip(OrgUser orgUser, Guid projectId)
        {
            var project = UnitOfWork.ProjectsRepository.Find(projectId);
            var assignment = project.Assignments.SingleOrDefault(a => a.OrgUserId == orgUser.Id);
            if (assignment == null || !assignment.CanExportZip)
                return false;

            // mobile accounts need an active subscription.
            if (orgUser.AccountType == AccountType.MobileAccount)
            {
                var latestSubscription = GetLatest(orgUser.Id);
                if (latestSubscription == null)
                    return false;

                // determine if this user has access to export pdfs.
                var subscription = GetLastSubscription(orgUser.Id);
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
            var project = UnitOfWork.ProjectsRepository.Find(projectId);
            var assignment = project.Assignments.SingleOrDefault(a => a.OrgUserId == orgUser.Id);
            if (assignment == null || !assignment.CanExportPdf)
                return false;

            // mobile accounts need an active subscription.
            if (orgUser.AccountType == AccountType.MobileAccount)
            {
                var latestSubscription = GetLatest(orgUser.Id);
                if (latestSubscription == null)
                    return false;

                // determine if this user has access to export pdfs.
                var subscription = GetLastSubscription(orgUser.Id);
                if (subscription == null)
                    return false;

                // organization subscribers don't need access to export capabilities.
                // they are granted access by default. Check paid subscriptions only.
                if (subscription.Type == UserSubscriptionType.Paid && !subscription.SubscriptionPlan.PdfExport)
                    return false;
            }

            return true;
        }

        private int GetFixedMonthlyDiskSpace()
        {
            var quota = ConfigurationManager.AppSettings["FixedMonthlyDiskSpace"];
            if (!string.IsNullOrEmpty(quota))
                return int.Parse(quota);

            return 1024;  // default hard-coded value
        }

        #endregion Helpers

    }

}
