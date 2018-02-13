namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_created_by_to_project : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "CreatedById", c => c.Guid());
            CreateIndex("dbo.Projects", "CreatedById");
            AddForeignKey("dbo.Projects", "CreatedById", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "CreatedById", "dbo.AspNetUsers");
            DropIndex("dbo.Projects", new[] { "CreatedById" });
            DropColumn("dbo.Projects", "CreatedById");
        }
    }
}
