namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Update_assignments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assignments", "CanAdd", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Assignments", "CanEdit", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Assignments", "CanDelete", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Assignments", "CanView", c => c.Boolean(nullable: false, defaultValue: true));
        }

        public override void Down()
        {
            DropColumn("dbo.Assignments", "CanView");
            DropColumn("dbo.Assignments", "CanDelete");
            DropColumn("dbo.Assignments", "CanEdit");
            DropColumn("dbo.Assignments", "CanAdd");
        }
    }
}
