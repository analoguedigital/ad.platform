namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_location_data : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FilledFormLocations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FilledFormId = c.Guid(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        Accuracy = c.Double(nullable: false),
                        Error = c.String(maxLength: 4000),
                        Event = c.String(maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FilledForms", t => t.FilledFormId, cascadeDelete: true)
                .Index(t => t.FilledFormId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FilledFormLocations", "FilledFormId", "dbo.FilledForms");
            DropIndex("dbo.FilledFormLocations", new[] { "FilledFormId" });
            DropTable("dbo.FilledFormLocations");
        }
    }
}
