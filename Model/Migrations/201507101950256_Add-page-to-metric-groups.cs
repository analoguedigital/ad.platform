namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addpagetometricgroups : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetricGroups", "Page", c => c.Int(nullable: false, defaultValue:1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetricGroups", "Page");
        }
    }
}
