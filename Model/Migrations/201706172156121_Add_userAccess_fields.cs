namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Add_userAccess_fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsWebUser", c => c.Boolean(defaultValue: true));
            AddColumn("dbo.AspNetUsers", "IsMobileUser", c => c.Boolean(defaultValue: true));
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsMobileUser");
            DropColumn("dbo.AspNetUsers", "IsWebUser");
        }
    }
}
