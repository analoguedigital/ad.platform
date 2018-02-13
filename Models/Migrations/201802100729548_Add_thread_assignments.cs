namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_thread_assignments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ThreadAssignments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrgUserId = c.Guid(nullable: false),
                        FormTemplateId = c.Guid(nullable: false),
                        CanAdd = c.Boolean(nullable: false, defaultValue: true),
                        CanEdit = c.Boolean(nullable: false, defaultValue: true),
                        CanDelete = c.Boolean(nullable: false, defaultValue: true),
                        CanView = c.Boolean(nullable: false, defaultValue: true),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FormTemplates", t => t.FormTemplateId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OrgUserId)
                .Index(t => new { t.FormTemplateId, t.OrgUserId }, unique: true, name: "IX_User_Thread");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ThreadAssignments", "OrgUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ThreadAssignments", "FormTemplateId", "dbo.FormTemplates");
            DropIndex("dbo.ThreadAssignments", "IX_User_Thread");
            DropTable("dbo.ThreadAssignments");
        }
    }
}
