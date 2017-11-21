namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_uniqueness_to_user_emails : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.OrgUsers", "Email", unique: true);
            CreateIndex("dbo.SuperUsers", "Email", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.SuperUsers", new[] { "Email" });
            DropIndex("dbo.OrgUsers", new[] { "Email" });
        }
    }
}
