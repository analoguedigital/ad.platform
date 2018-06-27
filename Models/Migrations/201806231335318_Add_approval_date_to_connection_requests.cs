namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_approval_date_to_connection_requests : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrgConnectionRequests", "ApprovalDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrgConnectionRequests", "ApprovalDate");
        }
    }
}
