namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_security_QA_to_users : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "SecurityQuestion", c => c.Byte());
            AddColumn("dbo.AspNetUsers", "SecurityAnswer", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SecurityAnswer");
            DropColumn("dbo.AspNetUsers", "SecurityQuestion");
        }
    }
}
