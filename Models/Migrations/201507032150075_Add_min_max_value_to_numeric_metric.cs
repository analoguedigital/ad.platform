namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_min_max_value_to_numeric_metric : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Metrics", "MinVal", c => c.Int());
            AddColumn("dbo.Metrics", "MaxVal", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Metrics", "MaxVal");
            DropColumn("dbo.Metrics", "MinVal");
        }
    }
}
