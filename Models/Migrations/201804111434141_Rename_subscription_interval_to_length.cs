namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rename_subscription_interval_to_length : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubscriptionPlans", "Length", c => c.Int(nullable: false));
            DropColumn("dbo.SubscriptionPlans", "Interval");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SubscriptionPlans", "Interval", c => c.Int(nullable: false));
            DropColumn("dbo.SubscriptionPlans", "Length");
        }
    }
}
