namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Add_hasTimeValue_to_date_metric : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Metrics", "HasTimeValue", c => c.Boolean());
            Sql("UPDATE dbo.Metrics SET HasTimeValue = 0");
        }

        public override void Down()
        {
            DropColumn("dbo.Metrics", "HasTimeValue");
        }
    }
}
