namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_disk_space_to_subscriptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubscriptionPlans", "MonthlyDiskSpace", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubscriptionPlans", "MonthlyDiskSpace");
        }
    }
}
