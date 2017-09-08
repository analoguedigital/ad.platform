namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_assignments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assignments", "HasReadAccess", c => c.Boolean(nullable: false));
            AddColumn("dbo.Assignments", "HasWriteAccess", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assignments", "HasWriteAccess");
            DropColumn("dbo.Assignments", "HasReadAccess");
        }
    }
}
