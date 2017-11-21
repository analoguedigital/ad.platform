namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fix_project_org_link : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Projects", new[] { "OrganisationId" });
            DropIndex("dbo.Projects", new[] { "Organisation_Id" });
            DropForeignKey("dbo.Projects", "FK_dbo.Projects_dbo.Organisations_Organisation_Id");
            DropForeignKey("dbo.Projects", "FK_dbo.Projects_dbo.Organisations_OrganisationId");
            Sql("Update Projects set Organisation_Id = OrganisationId");
            DropColumn("dbo.Projects", "OrganisationId");
            RenameColumn(table: "dbo.Projects", name: "Organisation_Id", newName: "OrganisationId");
            AlterColumn("dbo.Projects", "OrganisationId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Projects", "OrganisationId");
            AddForeignKey("dbo.Projects","OrganisationId","Organisations","Id",false, "FK_dbo.Projects_dbo.Organisations_OrganisationId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Projects", new[] { "OrganisationId" });
            AlterColumn("dbo.Projects", "OrganisationId", c => c.Guid());
            RenameColumn(table: "dbo.Projects", name: "OrganisationId", newName: "Organisation_Id");
            AddColumn("dbo.Projects", "OrganisationId", c => c.Guid(nullable: false));
            Sql("Update Projects set OrganisationId = Organisation_Id");
            CreateIndex("dbo.Projects", "Organisation_Id");
            CreateIndex("dbo.Projects", "OrganisationId");
        }
    }
}
