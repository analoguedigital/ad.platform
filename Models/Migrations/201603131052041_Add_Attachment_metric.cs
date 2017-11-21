namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Attachment_metric : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AttachmentTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        MaxFileSize = c.Int(nullable: false),
                        AllowedExtensions = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Attachments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FormValueId = c.Guid(nullable: false),
                        FileName = c.String(),
                        TypeId = c.Guid(nullable: false),
                        FileSize = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FormValues", t => t.FormValueId)
                .ForeignKey("dbo.AttachmentTypes", t => t.TypeId, cascadeDelete: true)
                .Index(t => t.FormValueId)
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.AttachmentMetricAllowedTypes",
                c => new
                    {
                        AttachmentMetricId = c.Guid(nullable: false),
                        AttachmentTypeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.AttachmentMetricId, t.AttachmentTypeId })
                .ForeignKey("dbo.Metrics", t => t.AttachmentMetricId, cascadeDelete: true)
                .ForeignKey("dbo.AttachmentTypes", t => t.AttachmentTypeId, cascadeDelete: true)
                .Index(t => t.AttachmentMetricId)
                .Index(t => t.AttachmentTypeId);
            
            AddColumn("dbo.Metrics", "AllowMultipleFiles", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Attachments", "TypeId", "dbo.AttachmentTypes");
            DropForeignKey("dbo.Attachments", "FormValueId", "dbo.FormValues");
            DropForeignKey("dbo.AttachmentMetricAllowedTypes", "AttachmentTypeId", "dbo.AttachmentTypes");
            DropForeignKey("dbo.AttachmentMetricAllowedTypes", "AttachmentMetricId", "dbo.Metrics");
            DropIndex("dbo.AttachmentMetricAllowedTypes", new[] { "AttachmentTypeId" });
            DropIndex("dbo.AttachmentMetricAllowedTypes", new[] { "AttachmentMetricId" });
            DropIndex("dbo.Attachments", new[] { "TypeId" });
            DropIndex("dbo.Attachments", new[] { "FormValueId" });
            DropColumn("dbo.Metrics", "AllowMultipleFiles");
            DropTable("dbo.AttachmentMetricAllowedTypes");
            DropTable("dbo.Attachments");
            DropTable("dbo.AttachmentTypes");
        }
    }
}
