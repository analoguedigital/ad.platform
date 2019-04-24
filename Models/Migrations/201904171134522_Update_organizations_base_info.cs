namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_organizations_base_info : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organisations", "Description", c => c.String(maxLength: 500));
            AddColumn("dbo.Organisations", "Website", c => c.String(maxLength: 150));
            AddColumn("dbo.Organisations", "LogoUrl", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organisations", "LogoUrl");
            DropColumn("dbo.Organisations", "Website");
            DropColumn("dbo.Organisations", "Description");
        }
    }
}
