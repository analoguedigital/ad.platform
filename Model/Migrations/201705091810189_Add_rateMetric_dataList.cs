namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_rateMetric_dataList : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Metrics", "RateMetricDataListId", c => c.Guid());
            CreateIndex("dbo.Metrics", "RateMetricDataListId");
            AddForeignKey("dbo.Metrics", "RateMetricDataListId", "dbo.DataLists", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Metrics", "RateMetricDataListId", "dbo.DataLists");
            DropIndex("dbo.Metrics", new[] { "RateMetricDataListId" });
            DropColumn("dbo.Metrics", "RateMetricDataListId");
        }
    }
}
