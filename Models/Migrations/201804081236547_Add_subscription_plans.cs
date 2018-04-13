namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_subscription_plans : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SubscriptionPlans",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(maxLength: 500),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Interval = c.Int(nullable: false),
                        IsLimited = c.Boolean(nullable: false),
                        MonthlyQuota = c.Int(),
                        PdfExport = c.Boolean(nullable: false),
                        ZipExport = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SubscriptionPlans");
        }
    }
}
