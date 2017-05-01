namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_datalist_relationships : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataListRelationships",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OwnerId = c.Guid(nullable: false),
                        DataListId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataLists", t => t.DataListId, cascadeDelete: false)
                .ForeignKey("dbo.DataLists", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.OwnerId)
                .Index(t => t.DataListId);
            
            AddColumn("dbo.DataLists", "Relationship1Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship2Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship3Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship4Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship5Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship6Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship7Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship8Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship9Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr1Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr2Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr3Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr4Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr5Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr6Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr7Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr8Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr9Id", c => c.Guid());
            CreateIndex("dbo.DataLists", "Relationship1Id");
            CreateIndex("dbo.DataLists", "Relationship2Id");
            CreateIndex("dbo.DataLists", "Relationship3Id");
            CreateIndex("dbo.DataLists", "Relationship4Id");
            CreateIndex("dbo.DataLists", "Relationship5Id");
            CreateIndex("dbo.DataLists", "Relationship6Id");
            CreateIndex("dbo.DataLists", "Relationship7Id");
            CreateIndex("dbo.DataLists", "Relationship8Id");
            CreateIndex("dbo.DataLists", "Relationship9Id");
            CreateIndex("dbo.DataListItems", "Attr1Id");
            CreateIndex("dbo.DataListItems", "Attr2Id");
            CreateIndex("dbo.DataListItems", "Attr3Id");
            CreateIndex("dbo.DataListItems", "Attr4Id");
            CreateIndex("dbo.DataListItems", "Attr5Id");
            CreateIndex("dbo.DataListItems", "Attr6Id");
            CreateIndex("dbo.DataListItems", "Attr7Id");
            CreateIndex("dbo.DataListItems", "Attr8Id");
            CreateIndex("dbo.DataListItems", "Attr9Id");
            AddForeignKey("dbo.DataListItems", "Attr1Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr2Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr3Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr4Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr5Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr6Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr7Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr8Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr9Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataLists", "Relationship1Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship2Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship3Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship4Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship5Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship6Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship7Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship8Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship9Id", "dbo.DataListRelationships", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DataLists", "Relationship9Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship8Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship7Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship6Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship5Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship4Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship3Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship2Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship1Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataListRelationships", "OwnerId", "dbo.DataLists");
            DropForeignKey("dbo.DataListRelationships", "DataListId", "dbo.DataLists");
            DropForeignKey("dbo.DataListItems", "Attr9Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr8Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr7Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr6Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr5Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr4Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr3Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr2Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr1Id", "dbo.DataListItems");
            DropIndex("dbo.DataListRelationships", new[] { "DataListId" });
            DropIndex("dbo.DataListRelationships", new[] { "OwnerId" });
            DropIndex("dbo.DataListItems", new[] { "Attr9Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr8Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr7Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr6Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr5Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr4Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr3Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr2Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr1Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship9Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship8Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship7Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship6Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship5Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship4Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship3Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship2Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship1Id" });
            DropColumn("dbo.DataListItems", "Attr9Id");
            DropColumn("dbo.DataListItems", "Attr8Id");
            DropColumn("dbo.DataListItems", "Attr7Id");
            DropColumn("dbo.DataListItems", "Attr6Id");
            DropColumn("dbo.DataListItems", "Attr5Id");
            DropColumn("dbo.DataListItems", "Attr4Id");
            DropColumn("dbo.DataListItems", "Attr3Id");
            DropColumn("dbo.DataListItems", "Attr2Id");
            DropColumn("dbo.DataListItems", "Attr1Id");
            DropColumn("dbo.DataLists", "Relationship9Id");
            DropColumn("dbo.DataLists", "Relationship8Id");
            DropColumn("dbo.DataLists", "Relationship7Id");
            DropColumn("dbo.DataLists", "Relationship6Id");
            DropColumn("dbo.DataLists", "Relationship5Id");
            DropColumn("dbo.DataLists", "Relationship4Id");
            DropColumn("dbo.DataLists", "Relationship3Id");
            DropColumn("dbo.DataLists", "Relationship2Id");
            DropColumn("dbo.DataLists", "Relationship1Id");
            DropTable("dbo.DataListRelationships");
        }
    }
}
