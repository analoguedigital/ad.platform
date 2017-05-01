namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_created_by_to_form_templates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormTemplates", "CreatedById", c => c.Guid(nullable: false));
            Sql("Update formtemplates set createdbyId = (select top 1 id from aspnetUsers where aspnetUsers.OrganisationId = formtemplates.organisationid and IsRootUser=1)");
            CreateIndex("dbo.FormTemplates", "CreatedById");
            AddForeignKey("dbo.FormTemplates", "CreatedById", "dbo.AspNetUsers", "Id");


        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FormTemplates", "CreatedById", "dbo.AspNetUsers");
            DropIndex("dbo.FormTemplates", new[] { "CreatedById" });
            DropColumn("dbo.FormTemplates", "CreatedById");
        }
    }
}
