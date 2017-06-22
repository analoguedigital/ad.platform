namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_subscription_models : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PromotionCodes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false, maxLength: 50),
                        Code = c.String(nullable: false, maxLength: 10, fixedLength: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsRedeemed = c.Boolean(nullable: false),
                        PaymentRecordId = c.Guid(),
                        OrganisationId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.Code, unique: true, name: "IX_PromotionCode_Code")
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.PaymentRecords",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Reference = c.String(maxLength: 50),
                        Note = c.String(),
                        OrgUserId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        PromotionCode_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OrgUserId, cascadeDelete: true)
                .ForeignKey("dbo.PromotionCodes", t => t.PromotionCode_Id)
                .Index(t => t.OrgUserId)
                .Index(t => t.PromotionCode_Id);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Note = c.String(),
                        PaymentRecordId = c.Guid(nullable: false),
                        OrgUserId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OrgUserId)
                .ForeignKey("dbo.PaymentRecords", t => t.PaymentRecordId, cascadeDelete: true)
                .Index(t => t.PaymentRecordId)
                .Index(t => t.OrgUserId);
            
            AddColumn("dbo.Organisations", "SubscriptionEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organisations", "SubscriptionMonthlyRate", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PromotionCodes", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Subscriptions", "PaymentRecordId", "dbo.PaymentRecords");
            DropForeignKey("dbo.Subscriptions", "OrgUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PaymentRecords", "PromotionCode_Id", "dbo.PromotionCodes");
            DropForeignKey("dbo.PaymentRecords", "OrgUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Subscriptions", new[] { "OrgUserId" });
            DropIndex("dbo.Subscriptions", new[] { "PaymentRecordId" });
            DropIndex("dbo.PaymentRecords", new[] { "PromotionCode_Id" });
            DropIndex("dbo.PaymentRecords", new[] { "OrgUserId" });
            DropIndex("dbo.PromotionCodes", new[] { "OrganisationId" });
            DropIndex("dbo.PromotionCodes", "IX_PromotionCode_Code");
            DropColumn("dbo.Organisations", "SubscriptionMonthlyRate");
            DropColumn("dbo.Organisations", "SubscriptionEnabled");
            DropTable("dbo.Subscriptions");
            DropTable("dbo.PaymentRecords");
            DropTable("dbo.PromotionCodes");
        }
    }
}
