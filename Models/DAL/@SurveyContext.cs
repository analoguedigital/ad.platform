using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Core;
using LightMethods.Survey.Models.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using LightMethods.Survey.Models.Services;
using LightMethods.Survey.Models.EntityConfig;

namespace LightMethods.Survey.Models.DAL
{
    public class SurveyContext : IdentityDbContext<User, Role, Guid, UserLogin, UserRole, UserClaim>
    {
        public const string CONNECTION_WEB_CONFIG_KEY = "LightSurveys";

        public static SurveyContext Create()
        {
            return new SurveyContext();
        }
        public SurveyContext()
           : base(CONNECTION_WEB_CONFIG_KEY)
        { }

        public SurveyContext(bool autoDetectChangesEnabled = true)
            : base(CONNECTION_WEB_CONFIG_KEY)
        {
            this.Configuration.AutoDetectChangesEnabled = autoDetectChangesEnabled;
        }

        #region System_Basic_Datasets

        //public DbSet<User> Users { get; set; }
        public DbSet<OrgUser> OrgUsers { get; set; }
        public DbSet<OrgUserType> OrgUserTypes { get; set; }
        public DbSet<SeverityLevel> SeverityLevels { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<ContactNumberType> ContactNumberTypes { get; set; }
        public DbSet<ContactNumber> ContactNumbers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AddressType> AddressTypes { get; set; }
        public DbSet<SuperUser> SupersUsers { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<AdultTitle> AdultTitles { get; set; }
        public DbSet<File> Files { set; get; }
        public DbSet<Guidance> Guidance { set; get; }
        public DbSet<Settings> Settings { set; get; }
        public DbSet<Language> Languages { set; get; }
        public DbSet<Calendar> Calendars { set; get; }
        public DbSet<PromotionCode> PromotionCodes { get; set; }
        public DbSet<PaymentRecord> PaymentRecords { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Email> Emails { get; set; }

        #endregion

        #region Project_Basic_Datasets

        public DbSet<Project> Projects { get; set; }
        public DbSet<Adult> Adults { get; set; }
        public DbSet<AdultAddress> AdultAddresses { get; set; }
        public DbSet<ExternalOrganisation> ExternalOrganisations { get; set; }
        public DbSet<OrganisationWorker> OrganisationWorkers { get; set; }
        public DbSet<KeyLocation> KeyLocations { get; set; }
        public DbSet<Commentary> Commentaries { set; get; }
        public DbSet<CommentaryDocument> CommentaryDocuments { set; get; }
        public DbSet<DataList> DataLists { set; get; }
        public DbSet<DataListItem> DataListItems { set; get; }
        public DbSet<DataListRelationship> DataListRelationships { set; get; }
        public DbSet<DataListItemAttr> Attributes { set; get; }

        #endregion

        #region Form_Template_Datasets

        public DbSet<FormTemplateCategory> FormTemplateCategories { get; set; }
        public DbSet<FormTemplate> FormTemplates { set; get; }
        public DbSet<MetricGroup> MetricGroups { set; get; }
        public DbSet<Metric> Metrics { set; get; }
        public DbSet<Attachment> Attachments { set; get; }
        public DbSet<AttachmentType> AttachmentTypes { set; get; }
        public DbSet<AttachmentMetric> AttachmentMetrics { set; get; }
        public DbSet<FreeTextMetric> FreeTextMetrics { set; get; }
        public DbSet<DichotomousMetric> DichotomousMetrics { set; get; }
        public DbSet<NumericMetric> NumericMetrics { set; get; }
        public DbSet<RateMetric> RateMetrics { set; get; }
        public DbSet<DateMetric> DateMetrics { set; get; }
        public DbSet<TimeMetric> TimeMetrics { set; get; }
        public DbSet<MultipleChoiceMetric> MultipleChoiceMetrics { set; get; }
        public DbSet<FilledForm> FilledForms { set; get; }
        public DbSet<FilledFormLocation> FilledFormLocations { set; get; }
        public DbSet<FormValue> FormValues { set; get; }

        #endregion

        #region Report_Template_Datasets

        public DbSet<ReportTemplateCategory> ReportTemplateCategories { get; set; }
        public DbSet<ReportTemplate> ReportTemplates { get; set; }
        public DbSet<ReportChart> ReportCharts { get; set; }
        public DbSet<ChartSerie> ChartSeries { get; set; }
        public DbSet<ChartSerieType> ChartSerieTypes { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportTable> ReportTables { get; set; }
        public DbSet<TableColumn> TableColumns { get; set; }
        public DbSet<ReportList> ReportLists { get; set; }
        public DbSet<ReportListDataType> ReportListDataTypes { get; set; }

        #endregion

        public override System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken)
        {
            EntitiesPreSaveActions();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            EntitiesPreSaveActions();

            try
            {
                return base.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        //raise a new exception inserting the current one as the InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }

        private void EntitiesPreSaveActions()
        {
            foreach (var item in ChangeTracker.Entries())
            {
                if (item.Entity is IEntity)
                {
                    if (item.State == EntityState.Added)
                    {
                        if ((item.Entity as IEntity).Id == Guid.Empty)
                            (item.Entity as IEntity).Id = Guid.NewGuid();

                        if (item.Entity is Entity)
                        {
                            (item.Entity as Entity).DateCreated = DateTimeService.UtcNow;
                            (item.Entity as Entity).DateUpdated = DateTimeService.UtcNow;
                        }
                    }
                    else if (item.State == EntityState.Modified && item.Entity is Entity)
                    {
                        (item.Entity as Entity).DateUpdated = DateTimeService.UtcNow;
                    }
                }
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // projects and organisations config
            modelBuilder.Configurations.Add(new ProjectConfig());
            modelBuilder.Configurations.Add(new AssignmentConfig());
            modelBuilder.Configurations.Add(new OrganisationConfig());
            modelBuilder.Configurations.Add(new OrganisationWorkerConfig());

            // users config
            modelBuilder.Configurations.Add(new OrgUserConfig());
            modelBuilder.Configurations.Add(new AdultConfig());
            modelBuilder.Configurations.Add(new AdultAddressConfig());
            modelBuilder.Configurations.Add(new AdultContactNumberConfig());
            modelBuilder.Configurations.Add(new ExternalOrgContactNumberConfig());

            // data lists config
            modelBuilder.Configurations.Add(new DataListConfig());
            modelBuilder.Configurations.Add(new DataListItemConfig());
            modelBuilder.Configurations.Add(new DataListItemAttrConfig());
            modelBuilder.Configurations.Add(new DataListRelationshipConfig());

            modelBuilder.Configurations.Add(new MultipleChoiceMetricConfig());
            modelBuilder.Configurations.Add(new RateMetricConfig());
            modelBuilder.Configurations.Add(new DocumentConfig());
            modelBuilder.Configurations.Add(new CommentaryConfig());
            modelBuilder.Configurations.Add(new CommentaryDocumentConfig());

            // form templates config
            modelBuilder.Configurations.Add(new ReportTemplateConfig());
            modelBuilder.Configurations.Add(new ReportTemplateCategoryConfig());
            modelBuilder.Configurations.Add(new FormTemplateCategoryConfig());
            modelBuilder.Configurations.Add(new FormTemplateConfig());
            modelBuilder.Configurations.Add(new MetricGroupConfig());
            modelBuilder.Configurations.Add(new FormValueConfig());
            modelBuilder.Configurations.Add(new FilledFormConfig());
            modelBuilder.Configurations.Add(new AttachmentConfig());
            modelBuilder.Configurations.Add(new AttachmentMetricConfig());

            // subscriptions config
            modelBuilder.Configurations.Add(new PromotionCodeConfig());
            modelBuilder.Configurations.Add(new PaymentRecordConfig());
            modelBuilder.Configurations.Add(new SubscriptionConfig());
        }
    }
}
