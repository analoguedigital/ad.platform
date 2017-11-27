namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Add_minimum_length_to_freetext_metric : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Metrics", "MinLength", c => c.Int(null, false, 0));
        }

        public override void Down()
        {
            DropColumn("dbo.Metrics", "MinLength");
        }
    }
}
