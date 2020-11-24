namespace Repository.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Artists",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(maxLength: 255),
                        Label = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Links",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(maxLength: 255),
                        Code = c.String(maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                        Url = c.String(),
                        MediaType = c.Int(nullable: false),
                        DomainId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Domains", t => t.DomainId, cascadeDelete: true)
                .Index(t => new { t.Title, t.Code, t.IsActive }, name: "IX_Links_Title_Code_IsActive")
                .Index(t => t.Code, name: "IX_Links_Code")
                .Index(t => t.DomainId);
            
            CreateTable(
                "dbo.Domains",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MediaServices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LinkArtists",
                c => new
                    {
                        Link_Id = c.Guid(nullable: false),
                        Artist_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Link_Id, t.Artist_Id })
                .ForeignKey("dbo.Links", t => t.Link_Id, cascadeDelete: true)
                .ForeignKey("dbo.Artists", t => t.Artist_Id, cascadeDelete: true)
                .Index(t => t.Link_Id)
                .Index(t => t.Artist_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Links", "DomainId", "dbo.Domains");
            DropForeignKey("dbo.LinkArtists", "Artist_Id", "dbo.Artists");
            DropForeignKey("dbo.LinkArtists", "Link_Id", "dbo.Links");
            DropIndex("dbo.LinkArtists", new[] { "Artist_Id" });
            DropIndex("dbo.LinkArtists", new[] { "Link_Id" });
            DropIndex("dbo.Links", new[] { "DomainId" });
            DropIndex("dbo.Links", "IX_Links_Code");
            DropIndex("dbo.Links", "IX_Links_Title_Code_IsActive");
            DropTable("dbo.LinkArtists");
            DropTable("dbo.MediaServices");
            DropTable("dbo.Domains");
            DropTable("dbo.Links");
            DropTable("dbo.Artists");
        }
    }
}
