namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Replace_voucher_amount_with_period : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vouchers", "Period", c => c.Int(nullable: false));
            DropColumn("dbo.Vouchers", "Amount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Vouchers", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Vouchers", "Period");
        }
    }
}
