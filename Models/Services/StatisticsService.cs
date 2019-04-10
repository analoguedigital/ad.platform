using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LightMethods.Survey.Models.Entities.User;

namespace LightMethods.Survey.Models.Services
{
    public class StatisticsService
    {

        private UnitOfWork UnitOfWork { get; set; }

        public StatisticsService(UnitOfWork uow)
        {
            UnitOfWork = uow;
        }

        public UsedSpaceDTO GetUsedSpace(Guid userId)
        {
            // this isn't accurate because we need to query records
            // made in the current month. from day 1 to today.
            // 
            //var lastMonth = DateTimeService.UtcNow.AddMonths(-1);
            //var lastMonthRecords = UnitOfWork.FilledFormsRepository.AllAsNoTracking
            //    .Where(x => x.FilledById == userId && x.DateCreated >= lastMonth);

            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastMonthRecords = UnitOfWork.FilledFormsRepository
                .AllAsNoTracking
                .Where(x => x.FilledById == userId && x.DateCreated >= startDate);

            var attachments = new List<AttachmentFlatDTO>();

            foreach (var record in lastMonthRecords)
            {
                foreach (var formValue in record.FormValues.Where(x => x.Attachments.Count > 0))
                {
                    foreach (var attachment in formValue.Attachments)
                    {
                        attachments.Add(new AttachmentFlatDTO
                        {
                            FileName = attachment.FileName,
                            FileSize = attachment.FileSize,
                            Type = attachment.Type.Name
                        });
                    }
                }
            }

            double totalSize = attachments.Sum(x => x.FileSize);

            return new UsedSpaceDTO
            {
                Attachments = attachments,
                TotalAttachments = attachments.Count,
                TotalSizeInBytes = (int)totalSize,
                TotalSizeInKiloBytes = totalSize / 1024,
                TotalSizeInMegaBytes = totalSize / 1024 / 1024
            };
        }

        public UsedSpaceDTO GetTotalUsedSpace(Guid userId)
        {
            var records = UnitOfWork.FilledFormsRepository
                .AllAsNoTracking
                .Where(x => x.FilledById == userId);

            var attachments = new List<AttachmentFlatDTO>();

            foreach (var record in records)
            {
                foreach (var formValue in record.FormValues.Where(x => x.Attachments.Count > 0))
                {
                    foreach (var attachment in formValue.Attachments)
                    {
                        attachments.Add(new AttachmentFlatDTO
                        {
                            FileName = attachment.FileName,
                            FileSize = attachment.FileSize,
                            Type = attachment.Type.Name
                        });
                    }
                }
            }

            double totalSize = attachments.Sum(x => x.FileSize);

            return new UsedSpaceDTO
            {
                Attachments = attachments,
                TotalAttachments = attachments.Count,
                TotalSizeInBytes = (int)totalSize,
                TotalSizeInKiloBytes = totalSize / 1024,
                TotalSizeInMegaBytes = totalSize / 1024 / 1024
            };
        }

        public UserStatsDTO GetUserStats(Guid userId)
        {
            var result = new UserStatsDTO();

            var project = UnitOfWork.ProjectsRepository
                .AllAsNoTracking
                .Where(x => x.CreatedById == userId)
                .SingleOrDefault();

            result.HasProject = project != null;

            if (project != null)
            {
                result.TotalThreads = UnitOfWork.FormTemplatesRepository
                .AllAsNoTracking
                .Count(x => x.ProjectId == project.Id && x.Discriminator == FormTemplateDiscriminators.RegularThread);

                result.TotalAdviceThreads = UnitOfWork.FormTemplatesRepository
                    .AllAsNoTracking
                    .Count(x => x.ProjectId == project.Id && x.Discriminator == FormTemplateDiscriminators.AdviceThread);

                result.TotalThreadRecords = UnitOfWork.FilledFormsRepository
                    .AllAsNoTracking
                    .Count(x => x.FilledById == userId && x.ProjectId == project.Id && x.FormTemplate.Discriminator == FormTemplateDiscriminators.RegularThread);

                result.TotalAdviceRecords = UnitOfWork.FilledFormsRepository
                    .AllAsNoTracking
                    .Count(x => x.FilledById == userId && x.ProjectId == project.Id && x.FormTemplate.Discriminator == FormTemplateDiscriminators.AdviceThread);

                result.TotalRecords = result.TotalThreadRecords + result.TotalAdviceRecords;

                result.TotalLocations = UnitOfWork.FilledFormLocationsRepository
                    .AllAsNoTracking
                    .Count(x => x.FilledForm.FilledById == userId && x.FilledForm.ProjectId == project.Id);

                result.TotalAttachments = UnitOfWork.AttachmentsRepository
                    .AllAsNoTracking
                    .Count(x => x.FormValue.FilledForm.FilledById == userId && x.FormValue.FilledForm.ProjectId == project.Id);

                var totalAttachmentsSize = UnitOfWork.AttachmentsRepository
                    .AllAsNoTracking
                    .Where(x => x.FormValue.FilledForm.FilledById == userId && x.FormValue.FilledForm.ProjectId == project.Id);

                if (totalAttachmentsSize.Any())
                {
                    result.TotalAttachmentsSize = totalAttachmentsSize.Sum(x => x.FileSize);
                    result.TotalAttachmentsSizeInKB = (double)result.TotalAttachmentsSize / 1024;
                    result.TotalAttachmentsSizeInMB = (double)result.TotalAttachmentsSize / 1024 / 1024;
                }

                // current month stats
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                result.CurrentMonthThreadRecords = UnitOfWork.FilledFormsRepository
                    .AllAsNoTracking
                    .Count(x => x.FilledById == userId && x.ProjectId == project.Id && x.FormTemplate.Discriminator == FormTemplateDiscriminators.RegularThread && x.DateCreated >= startDate);

                result.CurrentMonthAdviceRecords = UnitOfWork.FilledFormsRepository
                    .AllAsNoTracking
                    .Count(x => x.FilledById == userId && x.ProjectId == project.Id && x.FormTemplate.Discriminator == FormTemplateDiscriminators.AdviceThread && x.DateCreated >= startDate);

                result.CurrentMonthRecords = result.CurrentMonthThreadRecords + result.CurrentMonthAdviceRecords;

                result.CurrentMonthLocations = UnitOfWork.FilledFormLocationsRepository
                    .AllAsNoTracking
                    .Count(x => x.FilledForm.FilledById == userId && x.FilledForm.ProjectId == project.Id && x.DateCreated >= startDate);

                result.CurrentMonthAttachments = UnitOfWork.AttachmentsRepository
                    .AllAsNoTracking
                    .Count(x => x.FormValue.FilledForm.FilledById == userId && x.FormValue.FilledForm.ProjectId == project.Id && x.DateCreated >= startDate);

                var currentMonthAttachmentsSize = UnitOfWork.AttachmentsRepository
                    .AllAsNoTracking
                    .Where(x => x.FormValue.FilledForm.FilledById == userId && x.FormValue.FilledForm.ProjectId == project.Id && x.DateCreated >= startDate);

                if (currentMonthAttachmentsSize.Any())
                {
                    result.CurrentMonthAttachmentsSize = currentMonthAttachmentsSize.Sum(x => x.FileSize);
                    result.CurrentMonthAttachmentsSizeInKB = (double)result.CurrentMonthAttachmentsSize / 1024;
                    result.CurrentMonthAttachmentsSizeInMB = (double)result.CurrentMonthAttachmentsSize / 1024 / 1024;
                }

                // threads stat.
                result.ThreadStats = UnitOfWork.FormTemplatesRepository
                    .AllAsNoTracking
                    .Where(x => x.ProjectId == project.Id)
                    .Select(x => new UserThreadStatDTO
                    {
                        Title = x.Title,
                        Color = x.Colour,
                        Discriminator = x.Discriminator,
                        Records = UnitOfWork.FilledFormsRepository
                            .AllAsNoTracking
                            .Count(r => r.FormTemplateId == x.Id && r.FilledById == userId)
                    })
                    .ToList();

                // attachments stat.
                var attachmentTypes = UnitOfWork.AttachmentTypesRepository.AllAsNoTracking;
                foreach (var type in attachmentTypes)
                {
                    var item = new AttachmentTypeStatsDTO { Name = type.Name };

                    item.Count = UnitOfWork.AttachmentsRepository
                        .AllAsNoTracking
                        .Count(x => x.TypeId == type.Id && x.FormValue.FilledForm.FilledById == userId && x.FormValue.FilledForm.ProjectId == project.Id);

                    var currentMonthAttachments = UnitOfWork.AttachmentsRepository
                        .AllAsNoTracking
                        .Where(x => x.TypeId == type.Id && x.FormValue.FilledForm.FilledById == userId && x.FormValue.FilledForm.ProjectId == project.Id);

                    if (currentMonthAttachments.Any())
                    {
                        item.TotalSize = currentMonthAttachments.Sum(x => x.FileSize);

                        item.TotalSize = item.TotalSize;
                        item.TotalSizeInKB = (double)item.TotalSize / 1024;
                        item.TotalSizeInMB = (double)item.TotalSize / 1024 / 1024;

                        result.AttachmentStats.Types.Add(item);
                    }
                }
            }

            return result;
        }

        public PlatformStatsDTO GetPlatformStats()
        {
            var result = new PlatformStatsDTO();

            result.TotalOrganizations = UnitOfWork.OrganisationRepository.AllAsNoTracking.Count();
            result.TotalCases = UnitOfWork.ProjectsRepository.AllAsNoTracking.Count();
            result.TotalThreads = UnitOfWork.FormTemplatesRepository.AllAsNoTracking.Count(x => x.Discriminator == Entities.FormTemplateDiscriminators.RegularThread);
            result.TotalAdviceThreads = UnitOfWork.FormTemplatesRepository.AllAsNoTracking.Count(x => x.Discriminator == Entities.FormTemplateDiscriminators.AdviceThread);

            result.TotalThreadRecords = UnitOfWork.FilledFormsRepository.AllAsNoTracking.Count(x => x.FormTemplate.Discriminator == Entities.FormTemplateDiscriminators.RegularThread);
            result.TotalAdviceRecords = UnitOfWork.FilledFormsRepository.AllAsNoTracking.Count(x => x.FormTemplate.Discriminator == Entities.FormTemplateDiscriminators.AdviceThread);

            result.TotalRecords = result.TotalThreadRecords + result.TotalAdviceRecords;

            result.TotalThreadAttachments = UnitOfWork.AttachmentsRepository
                .AllAsNoTracking
                .Count(x => x.FormValue.FilledForm.FormTemplate.Discriminator == Entities.FormTemplateDiscriminators.RegularThread);

            result.TotalAdviceThreadAttachments = UnitOfWork.AttachmentsRepository
                .AllAsNoTracking
                .Count(x => x.FormValue.FilledForm.FormTemplate.Discriminator == Entities.FormTemplateDiscriminators.AdviceThread);

            result.TotalAttachments = result.TotalThreadAttachments + result.TotalAdviceThreadAttachments;

            result.TotalMobileAccounts = UnitOfWork.OrgUsersRepository.AllAsNoTracking.Count(x => x.AccountType == Entities.AccountType.MobileAccount);
            result.TotalWebAccounts = UnitOfWork.OrgUsersRepository.AllAsNoTracking.Count(x => x.AccountType == Entities.AccountType.WebAccount);
            result.TotalAccounts = result.TotalMobileAccounts + result.TotalWebAccounts;

            var sysAdmins = UnitOfWork.SuperUsersRepository.AllAsNoTracking.Count();
            var platformAdmins = UnitOfWork.PlatformUsersRepository.AllAsNoTracking.Count();

            result.TotalSystemAdmins = sysAdmins;
            result.TotalPlatformAdmins = platformAdmins;
            result.TotalAdmins = sysAdmins + platformAdmins;

            // team stats
            result.TotalTeams = UnitOfWork.OrganisationTeamsRepository.AllAsNoTracking.Count();

            var teams = UnitOfWork.OrganisationTeamsRepository.AllAsNoTracking;
            foreach (var team in teams)
            {
                result.TeamStats.Add(new TeamStatsDTO
                {
                    Name = team.Name,
                    Color = team.Colour,
                    Organization = team.Organisation.Name,
                    Members = team.Users.Count
                });
            }

            // attachment stats
            var totalSizeInBytes = UnitOfWork.AttachmentsRepository
                .AllAsNoTracking
                .Sum(x => x.FileSize);

            result.AttachmentStats.TotalSize = totalSizeInBytes;
            result.AttachmentStats.TotalSizeInKB = (double)totalSizeInBytes / 1024;
            result.AttachmentStats.TotalSizeInMB = (double)totalSizeInBytes / 1024 / 1024;

            var attachmentTypes = UnitOfWork.AttachmentTypesRepository.AllAsNoTracking;
            foreach (var type in attachmentTypes)
            {
                var item = new AttachmentTypeStatsDTO { Name = type.Name };

                item.Count = UnitOfWork.AttachmentsRepository
                    .AllAsNoTracking
                    .Count(x => x.TypeId == type.Id);

                var totalSize = UnitOfWork.AttachmentsRepository
                    .AllAsNoTracking
                    .Where(x => x.TypeId == type.Id)
                    .Sum(x => x.FileSize);

                item.TotalSize = totalSize;
                item.TotalSizeInKB = (double)totalSize / 1024;
                item.TotalSizeInMB = (double)totalSize / 1024 / 1024;

                result.AttachmentStats.Types.Add(item);
            }

            // unconfirmed accounts
            result.UnconfirmedAccounts = UnitOfWork.OrgUsersRepository
                .AllAsNoTracking
                .Where(x => !x.EmailConfirmed)
                .Select(x => new OrgUserFlatDTO
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    Surname = x.Surname,
                    Gender = x.Gender,
                    Birthdate = x.Birthdate,
                    Address = x.Address,
                    IsWebUser = x.IsWebUser,
                    IsMobileUser = x.IsMobileUser,
                    IsActive = x.IsActive,
                    LastLogin = x.LastLogin,
                    AccountType = x.AccountType,
                    IsRootUser = x.IsRootUser,
                })
                .ToList();

            return result;
        }

    }

    #region DTOs

    public class AttachmentFlatDTO
    {
        public string FileName { get; set; }

        public int FileSize { get; set; }

        public string Type { get; set; }
    }

    public class UsedSpaceDTO
    {
        public List<AttachmentFlatDTO> Attachments { get; set; }

        public int TotalAttachments { get; set; }

        public int TotalSizeInBytes { get; set; }

        public double TotalSizeInKiloBytes { get; set; }

        public double TotalSizeInMegaBytes { get; set; }
    }

    public class UserStatsDTO
    {
        public bool HasProject { get; set; }

        public int TotalThreads { get; set; }

        public int TotalAdviceThreads { get; set; }

        public int TotalThreadRecords { get; set; }

        public int TotalAdviceRecords { get; set; }

        public int TotalRecords { get; set; }

        public int TotalLocations { get; set; }

        public int TotalAttachments { get; set; }

        public int? TotalAttachmentsSize { get; set; }

        public double TotalAttachmentsSizeInKB { get; set; }

        public double TotalAttachmentsSizeInMB { get; set; }

        // current month stats
        public int CurrentMonthThreadRecords { get; set; }

        public int CurrentMonthAdviceRecords { get; set; }

        public int CurrentMonthRecords { get; set; }

        public int CurrentMonthLocations { get; set; }

        public int CurrentMonthAttachments { get; set; }

        public int? CurrentMonthAttachmentsSize { get; set; }

        public double CurrentMonthAttachmentsSizeInKB { get; set; }

        public double CurrentMonthAttachmentsSizeInMB { get; set; }

        public List<UserThreadStatDTO> ThreadStats { get; set; }

        public AttachmentStatsDTO AttachmentStats { get; set; }

        public UserStatsDTO()
        {
            this.ThreadStats = new List<UserThreadStatDTO>();
            this.AttachmentStats = new AttachmentStatsDTO();
        }
    }

    public class PlatformStatsDTO
    {
        public int TotalOrganizations { get; set; }

        public int TotalCases { get; set; }

        // threads and advice threads
        public int TotalThreads { get; set; }

        public int TotalAdviceThreads { get; set; }

        // records and advice records
        public int TotalThreadRecords { get; set; }

        public int TotalAdviceRecords { get; set; }

        public int TotalRecords { get; set; }

        // attachments
        public int TotalThreadAttachments { get; set; }

        public int TotalAdviceThreadAttachments { get; set; }

        public int TotalAttachments { get; set; }

        // user accounts
        public int TotalMobileAccounts { get; set; }

        public int TotalWebAccounts { get; set; }

        public int TotalAccounts { get; set; }

        // admin accounts
        public int TotalAdmins { get; set; }

        public int TotalSystemAdmins { get; set; }

        public int TotalPlatformAdmins { get; set; }

        public AttachmentStatsDTO AttachmentStats { get; set; }

        // organization teams
        public int TotalTeams { get; set; }

        public List<TeamStatsDTO> TeamStats { get; set; }

        public List<OrgUserFlatDTO> UnconfirmedAccounts { get; set; }

        public PlatformStatsDTO()
        {
            this.AttachmentStats = new AttachmentStatsDTO();
            this.TeamStats = new List<TeamStatsDTO>();
            this.UnconfirmedAccounts = new List<OrgUserFlatDTO>();
        }
    }

    public class TeamStatsDTO
    {
        public string Name { get; set; }

        public string Color { get; set; }

        public string Organization { get; set; }

        public int Members { get; set; }
    }

    public class AttachmentStatsDTO
    {
        public long TotalSize { get; set; }

        public double TotalSizeInKB { get; set; }

        public double TotalSizeInMB { get; set; }

        public List<AttachmentTypeStatsDTO> Types { get; set; }

        public AttachmentStatsDTO()
        {
            this.Types = new List<AttachmentTypeStatsDTO>();
        }
    }

    public class AttachmentTypeStatsDTO
    {
        public string Name { get; set; }

        public int Count { get; set; }

        public long TotalSize { get; set; }

        public double TotalSizeInKB { get; set; }

        public double TotalSizeInMB { get; set; }
    }

    public class OrgUserFlatDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public GenderType? Gender { get; set; }

        public DateTime? Birthdate { get; set; }

        public string Address { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool IsWebUser { get; set; }

        public bool IsMobileUser { get; set; }

        public bool IsRootUser { get; set; }

        public AccountType AccountType { get; set; }
    }

    public class UserThreadStatDTO
    {
        public string Title { get; set; }

        public string Color { get; set; }

        public FormTemplateDiscriminators Discriminator { get; set; }

        public int Records { get; set; }
    }

    #endregion

}
