namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_feedback : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Feedbacks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AddedById = c.Guid(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        AddedAt = c.DateTime(nullable: false),
                        Comment = c.String(nullable: false, maxLength: 500),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AddedById, cascadeDelete: true)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: false)
                .Index(t => t.AddedById)
                .Index(t => t.OrganisationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Feedbacks", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Feedbacks", "AddedById", "dbo.AspNetUsers");
            DropIndex("dbo.Feedbacks", new[] { "OrganisationId" });
            DropIndex("dbo.Feedbacks", new[] { "AddedById" });
            DropTable("dbo.Feedbacks");
        }
    }
}
