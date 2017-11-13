namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_form_template_advanced_settings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormTemplates", "Colour", c => c.String());
            AddColumn("dbo.FormTemplates", "CalendarDateMetricId", c => c.Guid());
            AddColumn("dbo.FormTemplates", "TimelineBarMetricId", c => c.Guid());
            CreateIndex("dbo.FormTemplates", "CalendarDateMetricId");
            CreateIndex("dbo.FormTemplates", "TimelineBarMetricId");
            AddForeignKey("dbo.FormTemplates", "CalendarDateMetricId", "dbo.Metrics", "Id");
            AddForeignKey("dbo.FormTemplates", "TimelineBarMetricId", "dbo.Metrics", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FormTemplates", "TimelineBarMetricId", "dbo.Metrics");
            DropForeignKey("dbo.FormTemplates", "CalendarDateMetricId", "dbo.Metrics");
            DropIndex("dbo.FormTemplates", new[] { "TimelineBarMetricId" });
            DropIndex("dbo.FormTemplates", new[] { "CalendarDateMetricId" });
            DropColumn("dbo.FormTemplates", "TimelineBarMetricId");
            DropColumn("dbo.FormTemplates", "CalendarDateMetricId");
            DropColumn("dbo.FormTemplates", "Colour");
        }
    }
}
