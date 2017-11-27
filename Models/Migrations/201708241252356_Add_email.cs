namespace LightMethods.Survey.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_email : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Emails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        To = c.String(nullable: false, maxLength: 150),
                        Cc = c.String(),
                        Bcc = c.String(),
                        Subject = c.String(nullable: false, maxLength: 150),
                        Content = c.String(nullable: false),
                        IsSent = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Emails");
        }
    }
}
