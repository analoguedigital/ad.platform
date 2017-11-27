namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_multiple_answer_metric : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Metrics", "IsMultipleAnswer", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Metrics", "IsMultipleAnswer");
        }
    }
}
