namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_terms_and_conditions_to_organizations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organisations", "TermsAndConditions", c => c.String());
            AddColumn("dbo.Organisations", "RequiresAgreement", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organisations", "RequiresAgreement");
            DropColumn("dbo.Organisations", "TermsAndConditions");
        }
    }
}
