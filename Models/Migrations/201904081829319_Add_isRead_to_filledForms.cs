namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_isRead_to_filledForms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FilledForms", "IsRead", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FilledForms", "IsRead");
        }
    }
}
