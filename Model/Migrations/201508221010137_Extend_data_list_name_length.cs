namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Extend_data_list_name_length : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DataLists", "Name", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DataLists", "Name", c => c.String(maxLength: 10));
        }
    }
}
