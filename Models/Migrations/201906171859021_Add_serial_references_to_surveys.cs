namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_serial_references_to_surveys : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FilledForms", "SerialReferences", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FilledForms", "SerialReferences");
        }
    }
}
