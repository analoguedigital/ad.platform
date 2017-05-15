using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Core;
using LightMethods.Survey.Models.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

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
                            (item.Entity as Entity).DateCreated = DateTime.Now;
                            (item.Entity as Entity).DateUpdated = DateTime.Now;
                        }
                    }
                    else if (item.State == EntityState.Modified && item.Entity is Entity)
                    {
                        (item.Entity as Entity).DateUpdated = DateTime.Now;
                    }
                }
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<OrgUser>().Map(m =>
            //  {
            //      m.MapInheritedProperties();
            //      m.ToTable("OrgUsers");
            //  });

            //modelBuilder.Entity<SuperUser>().Map(m =>
            //{
            //    m.MapInheritedProperties();
            //    m.ToTable("SuperUsers");
            //});

            modelBuilder.Entity<User>()
                .Property(x => x.UserName)
                .HasMaxLength(256);

            modelBuilder.Entity<OrgUser>()
                .HasRequired<OrgUserType>(u => u.Type)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrgUser>()
                .HasRequired<Organisation>(u => u.Organisation)
                .WithMany(o => o.OrgUsers)
                .HasForeignKey(u => u.OrganisationId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Project>()
                .HasRequired<Organisation>(u => u.Organisation)
                .WithMany(o => o.Projects)
                .HasForeignKey(p => p.OrganisationId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Assignment>()
                .HasRequired<OrgUser>(u => u.OrgUser)
                .WithMany(o => o.Assignments)
                .HasForeignKey(u => u.OrgUserId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<OrgUser>()
                .HasOptional<Project>(u => u.CurrentProject)
                .WithMany()
                .HasForeignKey(u => u.CurrentProjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Assignment>()
                .HasRequired<Project>(u => u.Project)
                .WithMany(o => o.Assignments)
                .HasForeignKey(u => u.ProjectId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<OrganisationWorker>()
               .ToTable("OrganisationWorkers");

            modelBuilder.Entity<OrganisationWorker>()
                .HasRequired<ExternalOrganisation>(w => w.Organisation)
                .WithMany(o => o.Workers)
                .HasForeignKey(w => w.OrganisationId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Adult>()
                .HasRequired<Project>(r => r.Project)
                .WithMany()
                .HasForeignKey(r => r.ProjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AdultAddress>()
                .ToTable("AdultAddresses");

            modelBuilder.Entity<AdultAddress>()
                .HasRequired<Adult>(a => a.Adult)
                .WithMany(a => a.Addresses)
                .HasForeignKey(a => a.AdultId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<AdultContactNumber>()
                .HasRequired<Adult>(a => a.Adult)
                .WithMany(a => a.ContactNumbers)
                .HasForeignKey(a => a.AdultId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ExternalOrgContactNumber>()
                .HasRequired<ExternalOrganisation>(a => a.Organisation)
                .WithMany(a => a.ContactNumbers)
                .HasForeignKey(a => a.OrganisationId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Commentary>()
                .HasRequired<Project>(c => c.Project)
                .WithMany()
                .WillCascadeOnDelete();

            modelBuilder.Entity<DataListItem>()
                .HasRequired<DataList>(d => d.DataList)
                .WithMany(d => d.AllItems)
                .HasForeignKey(d => d.DataListId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DataList>()
                .HasRequired<Organisation>(d => d.Organisation)
                .WithMany(o => o.DataLists)
                .HasForeignKey(d => d.OrganisationId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DataListRelationship>()
                .HasRequired<DataList>(d => d.Owner)
                .WithMany(d => d.NotOrderedRelationships)
                .HasForeignKey(r => r.OwnerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DataListRelationship>()
                .HasRequired<DataList>(d => d.DataList)
                .WithMany()
                .HasForeignKey(r => r.DataListId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DataListItemAttr>()
                .HasRequired<DataListItem>(a => a.Owner)
                .WithMany(i => i.Attributes)
                .HasForeignKey(a => a.OwnerId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DataListItemAttr>()
             .HasRequired<DataListRelationship>(a => a.Relationship)
             .WithMany()
             .HasForeignKey(a => a.RelationshipId)
             .WillCascadeOnDelete(true);


            modelBuilder.Entity<DataListItemAttr>()
                .HasRequired<DataListItem>(a => a.Value)
                .WithMany()
                .HasForeignKey(a => a.ValueId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MultipleChoiceMetric>()
                .HasRequired<DataList>(m => m.DataList)
                .WithMany()
                .HasForeignKey(m => m.DataListId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RateMetric>()
                .HasOptional<DataList>(m => m.DataList)
                .WithMany()
                .HasForeignKey(m => m.DataListId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrganisationWorker>().ToTable("OrganisationWorkers");

            OnDocumentModelCreating(modelBuilder);

            OnFormTemplateCreating(modelBuilder);
        }

        private void OnFormTemplateCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ReportTemplate>()
                .HasRequired<Organisation>(f => f.Organisation)
                .WithMany(o => o.ReportTemplates)
                .HasForeignKey(f => f.OrganisationId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ReportTemplateCategory>()
                .HasRequired<Organisation>(f => f.Organisation)
                .WithMany(o => o.ReportTemplateCategories)
                .HasForeignKey(f => f.OrganisationId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormTemplateCategory>()
                .HasRequired<Organisation>(f => f.Organisation)
                .WithMany(o => o.FormTemplateCategories)
                .HasForeignKey(f => f.OrganisationId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormTemplate>()
                .HasMany<MetricGroup>(t => t.MetricGroups)
                .WithRequired(g => g.FormTemplate)
                .HasForeignKey(g => g.FormTemplateId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormTemplate>()
                .HasRequired<OrgUser>(f => f.CreatedBy)
                .WithMany()
                .HasForeignKey(f => f.CreatedById)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MetricGroup>()
                .HasMany<Metric>(t => t.Metrics)
                .WithRequired(g => g.MetricGroup)
                .HasForeignKey(g => g.MetricGroupId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormValue>()
                .HasRequired<Metric>(v => v.Metric)
                .WithMany()
                .HasForeignKey(v => v.MetricId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FilledForm>()
                .HasMany<FormValue>(f => f.FormValues)
                .WithRequired(v => v.FilledForm)
                .HasForeignKey(v => v.FilledFormId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<FilledForm>()
                .HasMany<FilledFormLocation>(f => f.Locations)
                .WithRequired(v => v.FilledForm)
                .HasForeignKey(v => v.FilledFormId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Attachment>()
                .HasRequired<FormValue>(a => a.FormValue)
                .WithMany(a => a.Attachments)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AttachmentMetric>()
                .HasMany<AttachmentType>(m => m.AllowedAttachmentTypes)
                .WithMany()
                .Map(x =>
                {
                    x.MapLeftKey("AttachmentMetricId");
                    x.MapRightKey("AttachmentTypeId");
                    x.ToTable("AttachmentMetricAllowedTypes");
                });

        }

        void OnDocumentModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>()
                .HasRequired<File>(d => d.File)
                .WithMany()
                .HasForeignKey(d => d.FileId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<CommentaryDocument>()
                .HasRequired<Commentary>(d => d.Commentary)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CommentaryId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<CommentaryDocument>()
                .ToTable("CommentaryDocuments");
        }
    }
}
