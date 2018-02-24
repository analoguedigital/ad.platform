namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_organisation_teams : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrganisationTeams",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 30),
                        Description = c.String(maxLength: 150),
                        Colour = c.String(maxLength: 7),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.OrgTeamUsers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrganisationTeamId = c.Guid(nullable: false),
                        OrgUserId = c.Guid(nullable: false),
                        IsManager = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganisationTeams", t => t.OrganisationTeamId)
                .ForeignKey("dbo.AspNetUsers", t => t.OrgUserId)
                .Index(t => t.OrganisationTeamId)
                .Index(t => t.OrgUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrgTeamUsers", "OrgUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrgTeamUsers", "OrganisationTeamId", "dbo.OrganisationTeams");
            DropForeignKey("dbo.OrganisationTeams", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.OrgTeamUsers", new[] { "OrgUserId" });
            DropIndex("dbo.OrgTeamUsers", new[] { "OrganisationTeamId" });
            DropIndex("dbo.OrganisationTeams", new[] { "OrganisationId" });
            DropTable("dbo.OrgTeamUsers");
            DropTable("dbo.OrganisationTeams");
        }
    }
}
