namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Database_optimization : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DataListItems", new[] { "DataListId" });
            DropIndex("dbo.MetricGroups", new[] { "FormTemplateId" });
            DropIndex("dbo.Metrics", new[] { "FormTemplateId" });
            DropIndex("dbo.Metrics", new[] { "MetricGroupId" });
            DropIndex("dbo.FilledForms", new[] { "FormTemplateId" });
            DropIndex("dbo.FormValues", new[] { "FilledFormId" });
            CreateIndex("dbo.DataListItems", "DataListId");
            CreateIndex("dbo.MetricGroups", "FormTemplateId");
            CreateIndex("dbo.Metrics", "FormTemplateId");
            CreateIndex("dbo.Metrics", "MetricGroupId");
            CreateIndex("dbo.FilledForms", "FormTemplateId");
            CreateIndex("dbo.FilledForms", "SurveyDate");
            CreateIndex("dbo.FormValues", "FilledFormId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.FormValues", new[] { "FilledFormId" });
            DropIndex("dbo.FilledForms", new[] { "SurveyDate" });
            DropIndex("dbo.FilledForms", new[] { "FormTemplateId" });
            DropIndex("dbo.Metrics", new[] { "MetricGroupId" });
            DropIndex("dbo.Metrics", new[] { "FormTemplateId" });
            DropIndex("dbo.MetricGroups", new[] { "FormTemplateId" });
            DropIndex("dbo.DataListItems", new[] { "DataListId" });
            CreateIndex("dbo.FormValues", "FilledFormId");
            CreateIndex("dbo.FilledForms", "FormTemplateId");
            CreateIndex("dbo.Metrics", "MetricGroupId");
            CreateIndex("dbo.Metrics", "FormTemplateId");
            CreateIndex("dbo.MetricGroups", "FormTemplateId");
            CreateIndex("dbo.DataListItems", "DataListId");
        }
    }
}
