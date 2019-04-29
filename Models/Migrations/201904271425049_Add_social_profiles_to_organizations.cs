namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_social_profiles_to_organizations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organisations", "FacebookId", c => c.String(maxLength: 50));
            AddColumn("dbo.Organisations", "TwitterId", c => c.String(maxLength: 50));
            AddColumn("dbo.Organisations", "InstagramId", c => c.String(maxLength: 50));
            AddColumn("dbo.Organisations", "SkypeId", c => c.String(maxLength: 50));
            AddColumn("dbo.Organisations", "LinkedinUrl", c => c.String(maxLength: 100));
            AddColumn("dbo.Organisations", "YouTubeUrl", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organisations", "YouTubeUrl");
            DropColumn("dbo.Organisations", "LinkedinUrl");
            DropColumn("dbo.Organisations", "SkypeId");
            DropColumn("dbo.Organisations", "InstagramId");
            DropColumn("dbo.Organisations", "TwitterId");
            DropColumn("dbo.Organisations", "FacebookId");
        }
    }
}
