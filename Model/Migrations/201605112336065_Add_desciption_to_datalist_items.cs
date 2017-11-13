namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_desciption_to_datalist_items : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataListItems", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DataListItems", "Description");
        }
    }
}
