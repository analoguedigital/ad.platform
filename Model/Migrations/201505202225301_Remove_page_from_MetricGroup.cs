namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_page_from_MetricGroup : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MetricGroups", "Page");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MetricGroups", "Page", c => c.Int(nullable: false));
        }
    }
}
