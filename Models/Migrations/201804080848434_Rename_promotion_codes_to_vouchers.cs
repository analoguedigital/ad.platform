namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rename_promotion_codes_to_vouchers : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PromotionCodes", newName: "Vouchers");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Vouchers", newName: "PromotionCodes");
        }
    }
}
