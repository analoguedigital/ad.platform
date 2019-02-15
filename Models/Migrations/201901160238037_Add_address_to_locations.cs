namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_address_to_locations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FilledFormLocations", "Address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FilledFormLocations", "Address");
        }
    }
}
