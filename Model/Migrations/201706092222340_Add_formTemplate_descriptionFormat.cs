namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_formTemplate_descriptionFormat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormTemplates", "DescriptionFormat", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormTemplates", "DescriptionFormat");
        }
    }
}
