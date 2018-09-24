namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_email_recipients : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailRecipients",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrgUserId = c.Guid(nullable: false),
                        Feedbacks = c.Boolean(nullable: false),
                        NewMobileUsers = c.Boolean(nullable: false),
                        OrgRequests = c.Boolean(nullable: false),
                        OrgConnectionRequests = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OrgUserId, cascadeDelete: true)
                .Index(t => t.OrgUserId, unique: true, name: "IX_User_Recipient");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailRecipients", "OrgUserId", "dbo.AspNetUsers");
            DropIndex("dbo.EmailRecipients", "IX_User_Recipient");
            DropTable("dbo.EmailRecipients");
        }
    }
}
