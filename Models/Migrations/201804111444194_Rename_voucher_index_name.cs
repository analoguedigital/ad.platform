namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rename_voucher_index_name : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.Vouchers", name: "IX_PromotionCode_Code", newName: "IX_Voucher_Code");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Vouchers", name: "IX_Voucher_Code", newName: "IX_PromotionCode_Code");
        }
    }
}
