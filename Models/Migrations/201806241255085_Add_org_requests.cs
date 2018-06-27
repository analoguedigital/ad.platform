namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_org_requests : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrgRequests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 30),
                        Address = c.String(maxLength: 150),
                        ContactName = c.String(maxLength: 30),
                        Email = c.String(maxLength: 256),
                        TelNumber = c.String(maxLength: 30),
                        Postcode = c.String(maxLength: 8),
                        OrgUserId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OrgUserId, cascadeDelete: true)
                .Index(t => t.OrgUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrgRequests", "OrgUserId", "dbo.AspNetUsers");
            DropIndex("dbo.OrgRequests", new[] { "OrgUserId" });
            DropTable("dbo.OrgRequests");
        }
    }
}
