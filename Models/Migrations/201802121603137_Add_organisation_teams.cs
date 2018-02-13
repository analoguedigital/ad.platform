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
                "dbo.OrganisationTeamManagers",
                c => new
                    {
                        OrganisationTeamId = c.Guid(nullable: false),
                        OrgUserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrganisationTeamId, t.OrgUserId })
                .ForeignKey("dbo.OrganisationTeams", t => t.OrganisationTeamId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OrgUserId, cascadeDelete: true)
                .Index(t => t.OrganisationTeamId)
                .Index(t => t.OrgUserId);
            
            CreateTable(
                "dbo.OrganisationTeamMembers",
                c => new
                    {
                        OrganisationTeamId = c.Guid(nullable: false),
                        OrgUserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrganisationTeamId, t.OrgUserId })
                .ForeignKey("dbo.OrganisationTeams", t => t.OrganisationTeamId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OrgUserId, cascadeDelete: true)
                .Index(t => t.OrganisationTeamId)
                .Index(t => t.OrgUserId);
            
            CreateTable(
                "dbo.OrganisationTeamUsers",
                c => new
                    {
                        OrganisationTeamId = c.Guid(nullable: false),
                        OrgUserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrganisationTeamId, t.OrgUserId })
                .ForeignKey("dbo.AspNetUsers", t => t.OrganisationTeamId, cascadeDelete: true)
                .ForeignKey("dbo.OrganisationTeams", t => t.OrgUserId, cascadeDelete: true)
                .Index(t => t.OrganisationTeamId)
                .Index(t => t.OrgUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrganisationTeamUsers", "OrgUserId", "dbo.OrganisationTeams");
            DropForeignKey("dbo.OrganisationTeamUsers", "OrganisationTeamId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganisationTeams", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.OrganisationTeamMembers", "OrgUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganisationTeamMembers", "OrganisationTeamId", "dbo.OrganisationTeams");
            DropForeignKey("dbo.OrganisationTeamManagers", "OrgUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganisationTeamManagers", "OrganisationTeamId", "dbo.OrganisationTeams");
            DropIndex("dbo.OrganisationTeamUsers", new[] { "OrgUserId" });
            DropIndex("dbo.OrganisationTeamUsers", new[] { "OrganisationTeamId" });
            DropIndex("dbo.OrganisationTeamMembers", new[] { "OrgUserId" });
            DropIndex("dbo.OrganisationTeamMembers", new[] { "OrganisationTeamId" });
            DropIndex("dbo.OrganisationTeamManagers", new[] { "OrgUserId" });
            DropIndex("dbo.OrganisationTeamManagers", new[] { "OrganisationTeamId" });
            DropIndex("dbo.OrganisationTeams", new[] { "OrganisationId" });
            DropTable("dbo.OrganisationTeamUsers");
            DropTable("dbo.OrganisationTeamMembers");
            DropTable("dbo.OrganisationTeamManagers");
            DropTable("dbo.OrganisationTeams");
        }
    }
}
