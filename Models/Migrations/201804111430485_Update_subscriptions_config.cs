namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_subscriptions_config : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Subscriptions", new[] { "PaymentRecordId" });
            RenameColumn(table: "dbo.PaymentRecords", name: "PromotionCode_Id", newName: "Voucher_Id");
            RenameIndex(table: "dbo.PaymentRecords", name: "IX_PromotionCode_Id", newName: "IX_Voucher_Id");
            AddColumn("dbo.Subscriptions", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Subscriptions", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Subscriptions", "SubscriptionPlanId", c => c.Guid());
            AlterColumn("dbo.Subscriptions", "EndDate", c => c.DateTime());
            AlterColumn("dbo.Subscriptions", "PaymentRecordId", c => c.Guid());
            CreateIndex("dbo.Subscriptions", "PaymentRecordId");
            CreateIndex("dbo.Subscriptions", "SubscriptionPlanId");
            AddForeignKey("dbo.Subscriptions", "SubscriptionPlanId", "dbo.SubscriptionPlans", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "SubscriptionPlanId", "dbo.SubscriptionPlans");
            DropIndex("dbo.Subscriptions", new[] { "SubscriptionPlanId" });
            DropIndex("dbo.Subscriptions", new[] { "PaymentRecordId" });
            AlterColumn("dbo.Subscriptions", "PaymentRecordId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Subscriptions", "EndDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Subscriptions", "SubscriptionPlanId");
            DropColumn("dbo.Subscriptions", "IsActive");
            DropColumn("dbo.Subscriptions", "Type");
            RenameIndex(table: "dbo.PaymentRecords", name: "IX_Voucher_Id", newName: "IX_PromotionCode_Id");
            RenameColumn(table: "dbo.PaymentRecords", name: "Voucher_Id", newName: "PromotionCode_Id");
            CreateIndex("dbo.Subscriptions", "PaymentRecordId");
        }
    }
}
