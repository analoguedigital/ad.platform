namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Make_project_non_mandatory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FormTemplates", "Organisation_Id", "dbo.Organisations");
            DropIndex("dbo.FormTemplates", new[] { "ProjectId" });
            DropIndex("dbo.FormTemplates", new[] { "Organisation_Id" });
            RenameColumn(table: "dbo.FormTemplates", name: "Organisation_Id", newName: "OrganisationId");

            Sql("Update dbo.FormTemplates set OrganisationId = (select OrganisationId from dbo.Projects where Projects.Id = FormTemplates.ProjectId )");

            AlterColumn("dbo.FormTemplates", "ProjectId", c => c.Guid());
            AlterColumn("dbo.FormTemplates", "OrganisationId", c => c.Guid(nullable: false));
            CreateIndex("dbo.FormTemplates", "OrganisationId");
            CreateIndex("dbo.FormTemplates", "ProjectId");
            AddForeignKey("dbo.FormTemplates", "OrganisationId", "dbo.Organisations", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FormTemplates", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.FormTemplates", new[] { "ProjectId" });
            DropIndex("dbo.FormTemplates", new[] { "OrganisationId" });
            AlterColumn("dbo.FormTemplates", "OrganisationId", c => c.Guid());
            AlterColumn("dbo.FormTemplates", "ProjectId", c => c.Guid(nullable: false));
            RenameColumn(table: "dbo.FormTemplates", name: "OrganisationId", newName: "Organisation_Id");
            CreateIndex("dbo.FormTemplates", "Organisation_Id");
            CreateIndex("dbo.FormTemplates", "ProjectId");
            AddForeignKey("dbo.FormTemplates", "Organisation_Id", "dbo.Organisations", "Id");
        }
    }
}
