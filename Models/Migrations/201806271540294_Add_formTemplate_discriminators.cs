namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_formTemplate_discriminators : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormTemplates", "Discriminator", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormTemplates", "Discriminator");
        }
    }
}
