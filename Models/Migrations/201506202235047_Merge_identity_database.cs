namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Merge_identity_database : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.OrgUsers", newName: "AspNetUsers");
            DropForeignKey("dbo.Assignments", "OrgUserId", "dbo.OrgUsers");
            DropIndex("dbo.AspNetUsers", new[] { "Email" });
            DropIndex("dbo.AspNetUsers", new[] { "OrganisationId" });
            DropIndex("dbo.AspNetUsers", new[] { "TypeId" });
            DropIndex("dbo.SuperUsers", new[] { "Email" });
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        ClaimType = c.String(maxLength: 4000),
                        ClaimValue = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 4000),
                        ProviderKey = c.String(nullable: false, maxLength: 4000),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        RoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            AddColumn("dbo.AspNetUsers", "EmailConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "PasswordHash", c => c.String(maxLength: 4000));
            AddColumn("dbo.AspNetUsers", "SecurityStamp", c => c.String(maxLength: 4000));
            AddColumn("dbo.AspNetUsers", "PhoneNumber", c => c.String(maxLength: 4000));
            AddColumn("dbo.AspNetUsers", "PhoneNumberConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "TwoFactorEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "LockoutEndDateUtc", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "LockoutEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "AccessFailedCount", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "UserName", c => c.String(nullable: true, maxLength: 256));
            Sql("update dbo.AspNetUsers set UserName = Email");
            AlterColumn("dbo.AspNetUsers", "UserName", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.AspNetUsers", "Email", c => c.String(maxLength: 256));
            AlterColumn("dbo.AspNetUsers", "OrganisationId", c => c.Guid());
            AlterColumn("dbo.AspNetUsers", "TypeId", c => c.Guid());
            AlterColumn("dbo.AspNetUsers", "IsRootUser", c => c.Boolean());
            //CreateIndex("dbo.AspNetUsers", "UserName", unique: true, name: "UserNameIndex");
            CreateIndex("dbo.AspNetUsers", "OrganisationId");
            CreateIndex("dbo.AspNetUsers", "TypeId");
            CreateIndex("dbo.FilledForms", "FilledById");
            CreateIndex("dbo.Reports", "CreatedById");
            AddForeignKey("dbo.FilledForms", "FilledById", "dbo.AspNetUsers", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Reports", "CreatedById", "dbo.AspNetUsers", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Assignments", "OrgUserId", "dbo.AspNetUsers", "Id", cascadeDelete: false);
            DropColumn("dbo.AspNetUsers", "DateCreated");
            DropColumn("dbo.AspNetUsers", "DateUpdated");
            DropColumn("dbo.AspNetUsers", "IsExternal");
            DropColumn("dbo.AspNetUsers", "IsAdministrator");
            DropTable("dbo.SuperUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SuperUsers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Email = c.String(nullable: false, maxLength: 256),
                        LastLogin = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "IsAdministrator", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "IsExternal", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "DateUpdated", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "DateCreated", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.Assignments", "OrgUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Reports", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.FilledForms", "FilledById", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Reports", new[] { "CreatedById" });
            DropIndex("dbo.FilledForms", new[] { "FilledById" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "TypeId" });
            DropIndex("dbo.AspNetUsers", new[] { "OrganisationId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            AlterColumn("dbo.AspNetUsers", "IsRootUser", c => c.Boolean(nullable: false));
            AlterColumn("dbo.AspNetUsers", "TypeId", c => c.Guid(nullable: false));
            AlterColumn("dbo.AspNetUsers", "OrganisationId", c => c.Guid(nullable: false));
            AlterColumn("dbo.AspNetUsers", "Email", c => c.String(nullable: false, maxLength: 256));
            DropColumn("dbo.AspNetUsers", "Discriminator");
            DropColumn("dbo.AspNetUsers", "UserName");
            DropColumn("dbo.AspNetUsers", "AccessFailedCount");
            DropColumn("dbo.AspNetUsers", "LockoutEnabled");
            DropColumn("dbo.AspNetUsers", "LockoutEndDateUtc");
            DropColumn("dbo.AspNetUsers", "TwoFactorEnabled");
            DropColumn("dbo.AspNetUsers", "PhoneNumberConfirmed");
            DropColumn("dbo.AspNetUsers", "PhoneNumber");
            DropColumn("dbo.AspNetUsers", "SecurityStamp");
            DropColumn("dbo.AspNetUsers", "PasswordHash");
            DropColumn("dbo.AspNetUsers", "EmailConfirmed");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            CreateIndex("dbo.SuperUsers", "Email", unique: true);
            CreateIndex("dbo.AspNetUsers", "TypeId");
            CreateIndex("dbo.AspNetUsers", "OrganisationId");
            CreateIndex("dbo.AspNetUsers", "Email", unique: true);
            AddForeignKey("dbo.Assignments", "OrgUserId", "dbo.OrgUsers", "Id");
            RenameTable(name: "dbo.AspNetUsers", newName: "OrgUsers");
        }
    }
}
