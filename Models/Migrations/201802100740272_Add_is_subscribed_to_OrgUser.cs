namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_is_subscribed_to_OrgUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsSubscribed", c => c.Boolean());

            // Set default value for all org users 
            Sql("UPDATE AspNetUsers SET IsSubscribed='false' WHERE IsSubscribed is NULL");
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsSubscribed");
        }
    }
}
