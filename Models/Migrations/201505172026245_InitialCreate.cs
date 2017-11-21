namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AddressLine1 = c.String(nullable: false, maxLength: 30),
                        AddressLine2 = c.String(maxLength: 30),
                        AddressLine3 = c.String(maxLength: 30),
                        Town = c.String(maxLength: 30),
                        County = c.String(maxLength: 30),
                        Country = c.String(maxLength: 30),
                        Postcode = c.String(maxLength: 10),
                        Note = c.String(maxLength: 50),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AddressTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        Name = c.String(maxLength: 4000),
                        SystemName = c.String(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Adults",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        TitleId = c.Guid(),
                        FirstName = c.String(nullable: false, maxLength: 30),
                        Surname = c.String(nullable: false, maxLength: 30),
                        EmailAddress = c.String(maxLength: 50),
                        DateOfBirth = c.DateTime(),
                        GenderData = c.String(nullable: false, maxLength: 1),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .ForeignKey("dbo.AdultTitles", t => t.TitleId)
                .Index(t => t.ProjectId)
                .Index(t => t.TitleId);
            
            CreateTable(
                "dbo.ContactNumbers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TypeId = c.Guid(nullable: false),
                        Number = c.String(nullable: false, maxLength: 20),
                        Note = c.String(maxLength: 50),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        AdultId = c.Guid(),
                        OrganisationId = c.Guid(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Adults", t => t.AdultId, cascadeDelete: true)
                .ForeignKey("dbo.ExternalOrganisations", t => t.OrganisationId, cascadeDelete: true)
                .ForeignKey("dbo.ContactNumberTypes", t => t.TypeId, cascadeDelete: true)
                .Index(t => t.TypeId)
                .Index(t => t.AdultId)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.ContactNumberTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        Name = c.String(maxLength: 4000),
                        SystemName = c.String(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        Number = c.String(maxLength: 4000),
                        Name = c.String(nullable: false, maxLength: 4000),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        Flagged = c.Boolean(nullable: false),
                        Archived = c.Boolean(nullable: false),
                        Notes = c.String(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        Organisation_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.Organisation_Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .Index(t => t.OrganisationId)
                .Index(t => t.Organisation_Id);
            
            CreateTable(
                "dbo.Assignments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrgUserId = c.Guid(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrgUsers", t => t.OrgUserId)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.OrgUserId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.Organisations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(maxLength: 30),
                        TelNumber = c.String(maxLength: 30),
                        AddressLine1 = c.String(maxLength: 30),
                        AddressLine2 = c.String(maxLength: 30),
                        Town = c.String(maxLength: 30),
                        County = c.String(maxLength: 30),
                        Postcode = c.String(maxLength: 8),
                        IsActive = c.Boolean(nullable: false),
                        RootUserId = c.Guid(),
                        DefaultLanguageId = c.Guid(nullable: false),
                        DefaultCalendarId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Calendars", t => t.DefaultCalendarId, cascadeDelete: true)
                .ForeignKey("dbo.Languages", t => t.DefaultLanguageId, cascadeDelete: true)
                .ForeignKey("dbo.OrgUsers", t => t.RootUserId)
                .Index(t => t.RootUserId)
                .Index(t => t.DefaultLanguageId)
                .Index(t => t.DefaultCalendarId);
            
            CreateTable(
                "dbo.DataLists",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        Name = c.String(maxLength: 10),
                        DateArchived = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.DataListItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DataListId = c.Guid(nullable: false),
                        Text = c.String(maxLength: 256),
                        Value = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        DateArchived = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataLists", t => t.DataListId, cascadeDelete: true)
                .Index(t => t.DataListId);
            
            CreateTable(
                "dbo.Calendars",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        Name = c.String(maxLength: 4000),
                        SystemName = c.String(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        Name = c.String(maxLength: 4000),
                        SystemName = c.String(maxLength: 4000),
                        Calture = c.String(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FormTemplateCategories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        Title = c.String(nullable: false, maxLength: 50),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.FormTemplates",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        Code = c.String(nullable: false, maxLength: 10),
                        Title = c.String(nullable: false, maxLength: 50),
                        Version = c.Double(nullable: false),
                        Description = c.String(nullable: false, maxLength: 4000),
                        IsPublished = c.Boolean(nullable: false),
                        FormTemplateCategoryId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        Organisation_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FormTemplateCategories", t => t.FormTemplateCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .ForeignKey("dbo.Organisations", t => t.Organisation_Id)
                .Index(t => t.ProjectId)
                .Index(t => t.FormTemplateCategoryId)
                .Index(t => t.Organisation_Id);
            
            CreateTable(
                "dbo.MetricGroups",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false, maxLength: 4000),
                        Page = c.Int(nullable: false),
                        HelpContext = c.String(maxLength: 4000),
                        Order = c.Int(nullable: false),
                        FormTemplateId = c.Guid(nullable: false),
                        DataListId = c.Guid(),
                        NumberOfRows = c.Int(),
                        CanAddMoreRows = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataLists", t => t.DataListId)
                .ForeignKey("dbo.FormTemplates", t => t.FormTemplateId)
                .Index(t => t.FormTemplateId)
                .Index(t => t.DataListId);
            
            CreateTable(
                "dbo.Metrics",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DateArchived = c.DateTime(),
                        FormTemplateId = c.Guid(nullable: false),
                        ShortTitle = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false, maxLength: 4000),
                        MetricGroupId = c.Guid(nullable: false),
                        Mandatory = c.Boolean(nullable: false),
                        SectionTitle = c.String(maxLength: 100),
                        Order = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        NumberOfLine = c.Int(),
                        MaxLength = c.Int(),
                        DataListId = c.Guid(),
                        RowsDataListId = c.Guid(),
                        IsSumNeeded = c.Boolean(),
                        IsCountNeeded = c.Boolean(),
                        DataListId1 = c.Guid(),
                        MinValue = c.Int(),
                        MaxValue = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FormTemplates", t => t.FormTemplateId, cascadeDelete: true)
                .ForeignKey("dbo.DataLists", t => t.DataListId)
                .ForeignKey("dbo.DataLists", t => t.RowsDataListId)
                .ForeignKey("dbo.DataLists", t => t.DataListId1)
                .ForeignKey("dbo.MetricGroups", t => t.MetricGroupId)
                .Index(t => t.FormTemplateId)
                .Index(t => t.MetricGroupId)
                .Index(t => t.DataListId)
                .Index(t => t.RowsDataListId)
                .Index(t => t.DataListId1);
            
            CreateTable(
                "dbo.ReportTemplateCategories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        Title = c.String(nullable: false, maxLength: 50),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.ReportTemplates",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        Name = c.String(maxLength: 100),
                        Description = c.String(maxLength: 4000),
                        CategoryId = c.Guid(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReportTemplateCategories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .Index(t => t.OrganisationId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.ReportItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ReportTemplateId = c.Guid(nullable: false),
                        Name = c.String(maxLength: 100),
                        Order = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        DataTypeId = c.Guid(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReportTemplates", t => t.ReportTemplateId, cascadeDelete: true)
                .ForeignKey("dbo.ReportListDataTypes", t => t.DataTypeId, cascadeDelete: true)
                .Index(t => t.ReportTemplateId)
                .Index(t => t.DataTypeId);
            
            CreateTable(
                "dbo.ChartSeries",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ChartId = c.Guid(nullable: false),
                        TypeId = c.Guid(nullable: false),
                        MetricId = c.Guid(nullable: false),
                        MetricRowId = c.Guid(),
                        MetricColumnId = c.Guid(),
                        LabelId = c.Guid(),
                        LabelRowId = c.Guid(),
                        LabelColumnId = c.Guid(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReportItems", t => t.ChartId, cascadeDelete: true)
                .ForeignKey("dbo.Metrics", t => t.LabelId)
                .ForeignKey("dbo.Metrics", t => t.MetricId, cascadeDelete: true)
                .ForeignKey("dbo.ChartSerieTypes", t => t.TypeId, cascadeDelete: true)
                .Index(t => t.ChartId)
                .Index(t => t.TypeId)
                .Index(t => t.MetricId)
                .Index(t => t.LabelId);
            
            CreateTable(
                "dbo.ChartSerieTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        Name = c.String(maxLength: 4000),
                        SystemName = c.String(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReportListDataTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        Name = c.String(maxLength: 4000),
                        SystemName = c.String(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TableColumns",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        HeaderText = c.String(nullable: false, maxLength: 4000),
                        TableId = c.Guid(nullable: false),
                        MetricId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Metrics", t => t.MetricId, cascadeDelete: true)
                .ForeignKey("dbo.ReportItems", t => t.TableId, cascadeDelete: true)
                .Index(t => t.TableId)
                .Index(t => t.MetricId);
            
            CreateTable(
                "dbo.OrgUserTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        Name = c.String(maxLength: 4000),
                        SystemName = c.String(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AdultTitles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        Name = c.String(maxLength: 4000),
                        SystemName = c.String(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExternalOrganisations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        Email = c.String(maxLength: 255),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        AddressId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.AddressId, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.Commentaries",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        ShortTitle = c.String(nullable: false, maxLength: 150),
                        Description = c.String(maxLength: 4000),
                        Date = c.DateTime(nullable: false),
                        SeverityLevelId = c.Guid(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.SeverityLevels", t => t.SeverityLevelId)
                .Index(t => t.ProjectId)
                .Index(t => t.SeverityLevelId);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(maxLength: 4000),
                        FileName = c.String(maxLength: 4000),
                        FileExt = c.String(maxLength: 4000),
                        FileId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .Index(t => t.FileId);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Content = c.Binary(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SeverityLevels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        Name = c.String(maxLength: 4000),
                        SystemName = c.String(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FilledForms",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FormTemplateId = c.Guid(nullable: false),
                        Serial = c.Int(nullable: false, identity: true),
                        SurveyDate = c.DateTime(nullable: false),
                        FilledById = c.Guid(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FormTemplates", t => t.FormTemplateId, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.FormTemplateId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.FormValues",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FilledFormId = c.Guid(nullable: false),
                        MetricId = c.Guid(nullable: false),
                        RowNumber = c.Int(),
                        RowDataListItemId = c.Guid(),
                        TextValue = c.String(maxLength: 4000),
                        BoolValue = c.Boolean(),
                        NumericValue = c.Double(),
                        DateValue = c.DateTime(),
                        GuidValue = c.Guid(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Metrics", t => t.MetricId)
                .ForeignKey("dbo.DataListItems", t => t.RowDataListItemId)
                .ForeignKey("dbo.FilledForms", t => t.FilledFormId, cascadeDelete: true)
                .Index(t => t.FilledFormId)
                .Index(t => t.MetricId)
                .Index(t => t.RowDataListItemId);
            
            CreateTable(
                "dbo.Guidances",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Page = c.String(maxLength: 4000),
                        Content = c.String(maxLength: 4000),
                        UserTypeId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrgUserTypes", t => t.UserTypeId, cascadeDelete: true)
                .Index(t => t.UserTypeId);
            
            CreateTable(
                "dbo.KeyLocations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        ProjectId = c.Guid(nullable: false),
                        AddressId = c.Guid(nullable: false),
                        Note = c.String(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.AddressId, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TemplateId = c.Guid(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CreatedById = c.Guid(nullable: false),
                        Introduction = c.String(maxLength: 4000),
                        Conclusion = c.String(maxLength: 4000),
                        PdfFileId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.PdfFileId, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.ReportTemplates", t => t.TemplateId, cascadeDelete: true)
                .Index(t => t.TemplateId)
                .Index(t => t.ProjectId)
                .Index(t => t.PdfFileId);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProjectViewTemplateId = c.Guid(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReportTemplates", t => t.ProjectViewTemplateId)
                .Index(t => t.ProjectViewTemplateId);
            
            CreateTable(
                "dbo.AdultAddresses",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AdultId = c.Guid(nullable: false),
                        AddressTypeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.Id)
                .ForeignKey("dbo.Adults", t => t.AdultId, cascadeDelete: true)
                .ForeignKey("dbo.AddressTypes", t => t.AddressTypeId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.AdultId)
                .Index(t => t.AddressTypeId);
            
            CreateTable(
                "dbo.CommentaryDocuments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CommentaryId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.Id)
                .ForeignKey("dbo.Commentaries", t => t.CommentaryId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.CommentaryId);
            
            CreateTable(
                "dbo.OrganisationWorkers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        RoleDescription = c.String(maxLength: 4000),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Adults", t => t.Id)
                .ForeignKey("dbo.ExternalOrganisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.OrgUsers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Email = c.String(nullable: false, maxLength: 256),
                        LastLogin = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        FirstName = c.String(maxLength: 30),
                        Surname = c.String(maxLength: 30),
                        IsExternal = c.Boolean(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        TypeId = c.Guid(nullable: false),
                        IsAdministrator = c.Boolean(nullable: false),
                        IsRootUser = c.Boolean(nullable: false),
                        CurrentProjectId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .ForeignKey("dbo.OrgUserTypes", t => t.TypeId)
                .ForeignKey("dbo.Projects", t => t.CurrentProjectId)
                .Index(t => t.OrganisationId)
                .Index(t => t.TypeId)
                .Index(t => t.CurrentProjectId);
            
            CreateTable(
                "dbo.SuperUsers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Email = c.String(nullable: false, maxLength: 256),
                        LastLogin = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrgUsers", "CurrentProjectId", "dbo.Projects");
            DropForeignKey("dbo.OrgUsers", "TypeId", "dbo.OrgUserTypes");
            DropForeignKey("dbo.OrgUsers", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.OrganisationWorkers", "OrganisationId", "dbo.ExternalOrganisations");
            DropForeignKey("dbo.OrganisationWorkers", "Id", "dbo.Adults");
            DropForeignKey("dbo.CommentaryDocuments", "CommentaryId", "dbo.Commentaries");
            DropForeignKey("dbo.CommentaryDocuments", "Id", "dbo.Documents");
            DropForeignKey("dbo.AdultAddresses", "AddressTypeId", "dbo.AddressTypes");
            DropForeignKey("dbo.AdultAddresses", "AdultId", "dbo.Adults");
            DropForeignKey("dbo.AdultAddresses", "Id", "dbo.Addresses");
            DropForeignKey("dbo.Settings", "ProjectViewTemplateId", "dbo.ReportTemplates");
            DropForeignKey("dbo.Reports", "TemplateId", "dbo.ReportTemplates");
            DropForeignKey("dbo.Reports", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Reports", "PdfFileId", "dbo.Documents");
            DropForeignKey("dbo.Documents", "FileId", "dbo.Files");
            DropForeignKey("dbo.KeyLocations", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.KeyLocations", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Guidances", "UserTypeId", "dbo.OrgUserTypes");
            DropForeignKey("dbo.FilledForms", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.FormValues", "FilledFormId", "dbo.FilledForms");
            DropForeignKey("dbo.FormValues", "RowDataListItemId", "dbo.DataListItems");
            DropForeignKey("dbo.FormValues", "MetricId", "dbo.Metrics");
            DropForeignKey("dbo.FilledForms", "FormTemplateId", "dbo.FormTemplates");
            DropForeignKey("dbo.ContactNumbers", "TypeId", "dbo.ContactNumberTypes");
            DropForeignKey("dbo.Commentaries", "SeverityLevelId", "dbo.SeverityLevels");
            DropForeignKey("dbo.Commentaries", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ExternalOrganisations", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ContactNumbers", "OrganisationId", "dbo.ExternalOrganisations");
            DropForeignKey("dbo.ExternalOrganisations", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Adults", "TitleId", "dbo.AdultTitles");
            DropForeignKey("dbo.Adults", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Projects", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Assignments", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Assignments", "OrgUserId", "dbo.OrgUsers");
            DropForeignKey("dbo.Organisations", "RootUserId", "dbo.OrgUsers");
            DropForeignKey("dbo.ReportTemplates", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.TableColumns", "TableId", "dbo.ReportItems");
            DropForeignKey("dbo.TableColumns", "MetricId", "dbo.Metrics");
            DropForeignKey("dbo.ReportItems", "DataTypeId", "dbo.ReportListDataTypes");
            DropForeignKey("dbo.ChartSeries", "TypeId", "dbo.ChartSerieTypes");
            DropForeignKey("dbo.ChartSeries", "MetricId", "dbo.Metrics");
            DropForeignKey("dbo.ChartSeries", "LabelId", "dbo.Metrics");
            DropForeignKey("dbo.ChartSeries", "ChartId", "dbo.ReportItems");
            DropForeignKey("dbo.ReportItems", "ReportTemplateId", "dbo.ReportTemplates");
            DropForeignKey("dbo.ReportTemplates", "CategoryId", "dbo.ReportTemplateCategories");
            DropForeignKey("dbo.ReportTemplateCategories", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Projects", "Organisation_Id", "dbo.Organisations");
            DropForeignKey("dbo.FormTemplates", "Organisation_Id", "dbo.Organisations");
            DropForeignKey("dbo.FormTemplateCategories", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.FormTemplates", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.MetricGroups", "FormTemplateId", "dbo.FormTemplates");
            DropForeignKey("dbo.Metrics", "MetricGroupId", "dbo.MetricGroups");
            DropForeignKey("dbo.Metrics", "DataListId1", "dbo.DataLists");
            DropForeignKey("dbo.Metrics", "RowsDataListId", "dbo.DataLists");
            DropForeignKey("dbo.Metrics", "DataListId", "dbo.DataLists");
            DropForeignKey("dbo.Metrics", "FormTemplateId", "dbo.FormTemplates");
            DropForeignKey("dbo.MetricGroups", "DataListId", "dbo.DataLists");
            DropForeignKey("dbo.FormTemplates", "FormTemplateCategoryId", "dbo.FormTemplateCategories");
            DropForeignKey("dbo.Organisations", "DefaultLanguageId", "dbo.Languages");
            DropForeignKey("dbo.Organisations", "DefaultCalendarId", "dbo.Calendars");
            DropForeignKey("dbo.DataLists", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.DataListItems", "DataListId", "dbo.DataLists");
            DropForeignKey("dbo.ContactNumbers", "AdultId", "dbo.Adults");
            DropIndex("dbo.OrgUsers", new[] { "CurrentProjectId" });
            DropIndex("dbo.OrgUsers", new[] { "TypeId" });
            DropIndex("dbo.OrgUsers", new[] { "OrganisationId" });
            DropIndex("dbo.OrganisationWorkers", new[] { "OrganisationId" });
            DropIndex("dbo.OrganisationWorkers", new[] { "Id" });
            DropIndex("dbo.CommentaryDocuments", new[] { "CommentaryId" });
            DropIndex("dbo.CommentaryDocuments", new[] { "Id" });
            DropIndex("dbo.AdultAddresses", new[] { "AddressTypeId" });
            DropIndex("dbo.AdultAddresses", new[] { "AdultId" });
            DropIndex("dbo.AdultAddresses", new[] { "Id" });
            DropIndex("dbo.Settings", new[] { "ProjectViewTemplateId" });
            DropIndex("dbo.Reports", new[] { "PdfFileId" });
            DropIndex("dbo.Reports", new[] { "ProjectId" });
            DropIndex("dbo.Reports", new[] { "TemplateId" });
            DropIndex("dbo.KeyLocations", new[] { "AddressId" });
            DropIndex("dbo.KeyLocations", new[] { "ProjectId" });
            DropIndex("dbo.Guidances", new[] { "UserTypeId" });
            DropIndex("dbo.FormValues", new[] { "RowDataListItemId" });
            DropIndex("dbo.FormValues", new[] { "MetricId" });
            DropIndex("dbo.FormValues", new[] { "FilledFormId" });
            DropIndex("dbo.FilledForms", new[] { "ProjectId" });
            DropIndex("dbo.FilledForms", new[] { "FormTemplateId" });
            DropIndex("dbo.Documents", new[] { "FileId" });
            DropIndex("dbo.Commentaries", new[] { "SeverityLevelId" });
            DropIndex("dbo.Commentaries", new[] { "ProjectId" });
            DropIndex("dbo.ExternalOrganisations", new[] { "AddressId" });
            DropIndex("dbo.ExternalOrganisations", new[] { "ProjectId" });
            DropIndex("dbo.TableColumns", new[] { "MetricId" });
            DropIndex("dbo.TableColumns", new[] { "TableId" });
            DropIndex("dbo.ChartSeries", new[] { "LabelId" });
            DropIndex("dbo.ChartSeries", new[] { "MetricId" });
            DropIndex("dbo.ChartSeries", new[] { "TypeId" });
            DropIndex("dbo.ChartSeries", new[] { "ChartId" });
            DropIndex("dbo.ReportItems", new[] { "DataTypeId" });
            DropIndex("dbo.ReportItems", new[] { "ReportTemplateId" });
            DropIndex("dbo.ReportTemplates", new[] { "CategoryId" });
            DropIndex("dbo.ReportTemplates", new[] { "OrganisationId" });
            DropIndex("dbo.ReportTemplateCategories", new[] { "OrganisationId" });
            DropIndex("dbo.Metrics", new[] { "DataListId1" });
            DropIndex("dbo.Metrics", new[] { "RowsDataListId" });
            DropIndex("dbo.Metrics", new[] { "DataListId" });
            DropIndex("dbo.Metrics", new[] { "MetricGroupId" });
            DropIndex("dbo.Metrics", new[] { "FormTemplateId" });
            DropIndex("dbo.MetricGroups", new[] { "DataListId" });
            DropIndex("dbo.MetricGroups", new[] { "FormTemplateId" });
            DropIndex("dbo.FormTemplates", new[] { "Organisation_Id" });
            DropIndex("dbo.FormTemplates", new[] { "FormTemplateCategoryId" });
            DropIndex("dbo.FormTemplates", new[] { "ProjectId" });
            DropIndex("dbo.FormTemplateCategories", new[] { "OrganisationId" });
            DropIndex("dbo.DataListItems", new[] { "DataListId" });
            DropIndex("dbo.DataLists", new[] { "OrganisationId" });
            DropIndex("dbo.Organisations", new[] { "DefaultCalendarId" });
            DropIndex("dbo.Organisations", new[] { "DefaultLanguageId" });
            DropIndex("dbo.Organisations", new[] { "RootUserId" });
            DropIndex("dbo.Assignments", new[] { "ProjectId" });
            DropIndex("dbo.Assignments", new[] { "OrgUserId" });
            DropIndex("dbo.Projects", new[] { "Organisation_Id" });
            DropIndex("dbo.Projects", new[] { "OrganisationId" });
            DropIndex("dbo.ContactNumbers", new[] { "OrganisationId" });
            DropIndex("dbo.ContactNumbers", new[] { "AdultId" });
            DropIndex("dbo.ContactNumbers", new[] { "TypeId" });
            DropIndex("dbo.Adults", new[] { "TitleId" });
            DropIndex("dbo.Adults", new[] { "ProjectId" });
            DropTable("dbo.SuperUsers");
            DropTable("dbo.OrgUsers");
            DropTable("dbo.OrganisationWorkers");
            DropTable("dbo.CommentaryDocuments");
            DropTable("dbo.AdultAddresses");
            DropTable("dbo.Settings");
            DropTable("dbo.Reports");
            DropTable("dbo.KeyLocations");
            DropTable("dbo.Guidances");
            DropTable("dbo.FormValues");
            DropTable("dbo.FilledForms");
            DropTable("dbo.SeverityLevels");
            DropTable("dbo.Files");
            DropTable("dbo.Documents");
            DropTable("dbo.Commentaries");
            DropTable("dbo.ExternalOrganisations");
            DropTable("dbo.AdultTitles");
            DropTable("dbo.OrgUserTypes");
            DropTable("dbo.TableColumns");
            DropTable("dbo.ReportListDataTypes");
            DropTable("dbo.ChartSerieTypes");
            DropTable("dbo.ChartSeries");
            DropTable("dbo.ReportItems");
            DropTable("dbo.ReportTemplates");
            DropTable("dbo.ReportTemplateCategories");
            DropTable("dbo.Metrics");
            DropTable("dbo.MetricGroups");
            DropTable("dbo.FormTemplates");
            DropTable("dbo.FormTemplateCategories");
            DropTable("dbo.Languages");
            DropTable("dbo.Calendars");
            DropTable("dbo.DataListItems");
            DropTable("dbo.DataLists");
            DropTable("dbo.Organisations");
            DropTable("dbo.Assignments");
            DropTable("dbo.Projects");
            DropTable("dbo.ContactNumberTypes");
            DropTable("dbo.ContactNumbers");
            DropTable("dbo.Adults");
            DropTable("dbo.AddressTypes");
            DropTable("dbo.Addresses");
        }
    }
}
