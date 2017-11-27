namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_time_metric : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormValues", "TimeValue", c => c.Time(precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormValues", "TimeValue");
        }
    }
}
