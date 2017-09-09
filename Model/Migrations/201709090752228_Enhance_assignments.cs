namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Enhance_assignments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assignments", "CanAdd", c => c.Boolean(nullable: false));
            AddColumn("dbo.Assignments", "CanEdit", c => c.Boolean(nullable: false));
            AddColumn("dbo.Assignments", "CanDelete", c => c.Boolean(nullable: false));
            AddColumn("dbo.Assignments", "CanView", c => c.Boolean(nullable: false));
            DropColumn("dbo.Assignments", "HasReadAccess");
            DropColumn("dbo.Assignments", "HasWriteAccess");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assignments", "HasWriteAccess", c => c.Boolean(nullable: false));
            AddColumn("dbo.Assignments", "HasReadAccess", c => c.Boolean(nullable: false));
            DropColumn("dbo.Assignments", "CanView");
            DropColumn("dbo.Assignments", "CanDelete");
            DropColumn("dbo.Assignments", "CanEdit");
            DropColumn("dbo.Assignments", "CanAdd");
        }
    }
}
