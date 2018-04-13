namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_organisation_invitations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrganisationInvitations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Token = c.String(nullable: false, maxLength: 10, fixedLength: true),
                        Limit = c.Int(nullable: false),
                        Used = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.Token, unique: true, name: "IX_OrgInvitation_Token")
                .Index(t => t.OrganisationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrganisationInvitations", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.OrganisationInvitations", new[] { "OrganisationId" });
            DropIndex("dbo.OrganisationInvitations", "IX_OrgInvitation_Token");
            DropTable("dbo.OrganisationInvitations");
        }
    }
}
