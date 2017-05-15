namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_rateMetric_defaultValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Metrics", "DefaultValue", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Metrics", "DefaultValue");
        }
    }
}
