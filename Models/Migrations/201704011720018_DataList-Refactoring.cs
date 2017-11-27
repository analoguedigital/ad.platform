namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataListRefactoring : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DataListItems", "Attr1Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr2Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr3Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr4Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr5Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr6Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr7Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr8Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItems", "Attr9Id", "dbo.DataListItems");
            DropForeignKey("dbo.DataLists", "Relationship1Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship2Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship3Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship4Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship5Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship6Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship7Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship8Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataLists", "Relationship9Id", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataListRelationships", "DataListId", "dbo.DataLists");
            DropForeignKey("dbo.DataListRelationships", "OwnerId", "dbo.DataLists");
            DropIndex("dbo.DataLists", new[] { "Relationship1Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship2Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship3Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship4Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship5Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship6Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship7Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship8Id" });
            DropIndex("dbo.DataLists", new[] { "Relationship9Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr1Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr2Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr3Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr4Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr5Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr6Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr7Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr8Id" });
            DropIndex("dbo.DataListItems", new[] { "Attr9Id" });
            DropIndex("dbo.DataListRelationships", new[] { "OwnerId" });
            CreateTable(
                "dbo.DataListItemAttrs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OwnerId = c.Guid(nullable: false),
                        RelationshipId = c.Guid(nullable: false),
                        ValueId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataListItems", t => t.OwnerId, cascadeDelete: true)
                .ForeignKey("dbo.DataListRelationships", t => t.RelationshipId, cascadeDelete: true)
                .ForeignKey("dbo.DataListItems", t => t.ValueId)
                .Index(t => t.OwnerId)
                .Index(t => t.RelationshipId)
                .Index(t => t.ValueId);
            
            AddColumn("dbo.DataListRelationships", "Order", c => c.Int(nullable: false));
            CreateIndex("dbo.DataListRelationships", "OwnerId");
            AddForeignKey("dbo.DataListRelationships", "DataListId", "dbo.DataLists", "Id");
            AddForeignKey("dbo.DataListRelationships", "OwnerId", "dbo.DataLists", "Id");

            Sql(@"
Insert into DataListItemAttrs (Id, OwnerId, RelationshipId, ValueId, DateUpdated, DateCreated)
select *, GETDATE() as DateUpdated, GETDATE() as DateCreated from (
select NEWID() as Id, di.id as OwnerId,Relationship1Id as RelationshipId ,Attr1Id as ValueId from datalistitems di
inner join DataLists dl on (di.DataListId = dl.id)
where Relationship1Id is not null 

union

select NEWID() as Id, di.id as OwnerId,Relationship2Id as RelationshipId ,Attr2Id as ValueId from datalistitems di
inner join DataLists dl on (di.DataListId = dl.id)
where Relationship2Id is not null 

union

select NEWID() as Id, di.id as OwnerId,Relationship3Id as RelationshipId ,Attr3Id as ValueId from datalistitems di
inner join DataLists dl on (di.DataListId = dl.id)
where Relationship3Id is not null 

union

select NEWID() as Id, di.id as OwnerId,Relationship4Id as RelationshipId ,Attr4Id as ValueId from datalistitems di
inner join DataLists dl on (di.DataListId = dl.id)
where Relationship4Id is not null 

union

select NEWID() as Id, di.id as OwnerId,Relationship5Id as RelationshipId ,Attr5Id as ValueId from datalistitems di
inner join DataLists dl on (di.DataListId = dl.id)
where Relationship5Id is not null 

union

select NEWID() as Id, di.id as OwnerId,Relationship6Id as RelationshipId ,Attr6Id as ValueId from datalistitems di
inner join DataLists dl on (di.DataListId = dl.id)
where Relationship6Id is not null 

union

select NEWID() as Id, di.id as OwnerId,Relationship7Id as RelationshipId ,Attr7Id as ValueId from datalistitems di
inner join DataLists dl on (di.DataListId = dl.id)
where Relationship7Id is not null 

union

select NEWID() as Id, di.id as OwnerId,Relationship8Id as RelationshipId ,Attr8Id as ValueId from datalistitems di
inner join DataLists dl on (di.DataListId = dl.id)
where Relationship8Id is not null 

union

select NEWID() as Id, di.id as OwnerId,Relationship9Id as RelationshipId ,Attr9Id as ValueId from datalistitems di
inner join DataLists dl on (di.DataListId = dl.id)
where Relationship9Id is not null ) temp");


            Sql(@"
Update DataListRelationships
set [Order] = num
from (
select *, ROW_NUMBER() OVER(ORDER BY OwnerId ASC) AS Num  from DataListRelationships
) as DataListRelationships");


            DropColumn("dbo.DataLists", "Relationship1Id");
            DropColumn("dbo.DataLists", "Relationship2Id");
            DropColumn("dbo.DataLists", "Relationship3Id");
            DropColumn("dbo.DataLists", "Relationship4Id");
            DropColumn("dbo.DataLists", "Relationship5Id");
            DropColumn("dbo.DataLists", "Relationship6Id");
            DropColumn("dbo.DataLists", "Relationship7Id");
            DropColumn("dbo.DataLists", "Relationship8Id");
            DropColumn("dbo.DataLists", "Relationship9Id");
            DropColumn("dbo.DataListItems", "Attr1Id");
            DropColumn("dbo.DataListItems", "Attr2Id");
            DropColumn("dbo.DataListItems", "Attr3Id");
            DropColumn("dbo.DataListItems", "Attr4Id");
            DropColumn("dbo.DataListItems", "Attr5Id");
            DropColumn("dbo.DataListItems", "Attr6Id");
            DropColumn("dbo.DataListItems", "Attr7Id");
            DropColumn("dbo.DataListItems", "Attr8Id");
            DropColumn("dbo.DataListItems", "Attr9Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DataListItems", "Attr9Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr8Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr7Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr6Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr5Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr4Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr3Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr2Id", c => c.Guid());
            AddColumn("dbo.DataListItems", "Attr1Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship9Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship8Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship7Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship6Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship5Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship4Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship3Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship2Id", c => c.Guid());
            AddColumn("dbo.DataLists", "Relationship1Id", c => c.Guid());
            DropForeignKey("dbo.DataListRelationships", "OwnerId", "dbo.DataLists");
            DropForeignKey("dbo.DataListRelationships", "DataListId", "dbo.DataLists");
            DropForeignKey("dbo.DataListItemAttrs", "ValueId", "dbo.DataListItems");
            DropForeignKey("dbo.DataListItemAttrs", "RelationshipId", "dbo.DataListRelationships");
            DropForeignKey("dbo.DataListItemAttrs", "OwnerId", "dbo.DataListItems");
            DropIndex("dbo.DataListRelationships", new[] { "OwnerId" });
            DropIndex("dbo.DataListItemAttrs", new[] { "ValueId" });
            DropIndex("dbo.DataListItemAttrs", new[] { "RelationshipId" });
            DropIndex("dbo.DataListItemAttrs", new[] { "OwnerId" });
            DropColumn("dbo.DataListRelationships", "Order");
            DropTable("dbo.DataListItemAttrs");
            CreateIndex("dbo.DataListRelationships", "OwnerId");
            CreateIndex("dbo.DataListItems", "Attr9Id");
            CreateIndex("dbo.DataListItems", "Attr8Id");
            CreateIndex("dbo.DataListItems", "Attr7Id");
            CreateIndex("dbo.DataListItems", "Attr6Id");
            CreateIndex("dbo.DataListItems", "Attr5Id");
            CreateIndex("dbo.DataListItems", "Attr4Id");
            CreateIndex("dbo.DataListItems", "Attr3Id");
            CreateIndex("dbo.DataListItems", "Attr2Id");
            CreateIndex("dbo.DataListItems", "Attr1Id");
            CreateIndex("dbo.DataLists", "Relationship9Id");
            CreateIndex("dbo.DataLists", "Relationship8Id");
            CreateIndex("dbo.DataLists", "Relationship7Id");
            CreateIndex("dbo.DataLists", "Relationship6Id");
            CreateIndex("dbo.DataLists", "Relationship5Id");
            CreateIndex("dbo.DataLists", "Relationship4Id");
            CreateIndex("dbo.DataLists", "Relationship3Id");
            CreateIndex("dbo.DataLists", "Relationship2Id");
            CreateIndex("dbo.DataLists", "Relationship1Id");
            AddForeignKey("dbo.DataListRelationships", "OwnerId", "dbo.DataLists", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DataListRelationships", "DataListId", "dbo.DataLists", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DataLists", "Relationship9Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship8Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship7Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship6Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship5Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship4Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship3Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship2Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataLists", "Relationship1Id", "dbo.DataListRelationships", "Id");
            AddForeignKey("dbo.DataListItems", "Attr9Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr8Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr7Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr6Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr5Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr4Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr3Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr2Id", "dbo.DataListItems", "Id");
            AddForeignKey("dbo.DataListItems", "Attr1Id", "dbo.DataListItems", "Id");
        }
    }
}
