namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_IsAggregate_to_projects : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "IsAggregate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "IsAggregate");
        }
    }
}
