namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_org_connection_requests : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrgConnectionRequests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrgUserId = c.Guid(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OrgUserId, cascadeDelete: false)  // is it safe to disable cascadeDelete? the original generated script set this to true.
                .Index(t => t.OrgUserId)
                .Index(t => t.OrganisationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrgConnectionRequests", "OrgUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrgConnectionRequests", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.OrgConnectionRequests", new[] { "OrganisationId" });
            DropIndex("dbo.OrgConnectionRequests", new[] { "OrgUserId" });
            DropTable("dbo.OrgConnectionRequests");
        }
    }
}
