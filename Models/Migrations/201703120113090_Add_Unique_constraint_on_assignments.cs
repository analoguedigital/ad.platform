namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Unique_constraint_on_assignments : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Assignments", new[] { "OrgUserId" });
            DropIndex("dbo.Assignments", new[] { "ProjectId" });
            CreateIndex("dbo.Assignments", new[] { "ProjectId", "OrgUserId" }, unique: true, name: "IX_User_Project");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Assignments", "IX_User_Project");
            CreateIndex("dbo.Assignments", "ProjectId");
            CreateIndex("dbo.Assignments", "OrgUserId");
        }
    }
}
