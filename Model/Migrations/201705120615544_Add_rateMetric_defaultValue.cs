namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_rateMetric_defaultValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Metrics", "DefaultValue", c => c.Int());
            Sql("UPDATE dbo.Metrics SET DefaultValue = MinValue");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Metrics", "DefaultValue");
        }
    }
}
