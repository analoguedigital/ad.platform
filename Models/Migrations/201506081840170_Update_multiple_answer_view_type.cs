namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_multiple_answer_view_type : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Metrics", "ViewType", c => c.Int());
            DropColumn("dbo.Metrics", "IsMultipleAnswer");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Metrics", "IsMultipleAnswer", c => c.Boolean());
            DropColumn("dbo.Metrics", "ViewType");
        }
    }
}
