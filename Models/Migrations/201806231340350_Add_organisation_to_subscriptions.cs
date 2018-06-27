namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_organisation_to_subscriptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscriptions", "OrganisationId", c => c.Guid());
            CreateIndex("dbo.Subscriptions", "OrganisationId");
            AddForeignKey("dbo.Subscriptions", "OrganisationId", "dbo.Organisations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.Subscriptions", new[] { "OrganisationId" });
            DropColumn("dbo.Subscriptions", "OrganisationId");
        }
    }
}
