namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_export_flags_to_assignment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assignments", "CanExportPdf", c => c.Boolean(nullable: false));
            AddColumn("dbo.Assignments", "CanExportZip", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assignments", "CanExportZip");
            DropColumn("dbo.Assignments", "CanExportPdf");
        }
    }
}
