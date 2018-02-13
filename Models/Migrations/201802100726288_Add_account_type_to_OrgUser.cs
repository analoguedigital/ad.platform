namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_account_type_to_OrgUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AccountType", c => c.Int());

            // Set default account type for all org users 
            Sql("UPDATE AspNetUsers SET AccountType=1 WHERE AccountType is NULL");
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AccountType");
        }
    }
}
