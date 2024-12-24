namespace kaaraoke_app.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AlbumCovers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImagePath = c.String(),
                        AudioFile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AudioFiles", t => t.AudioFile_Id)
                .Index(t => t.AudioFile_Id);
            
            CreateTable(
                "dbo.AudioFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LRCFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        AudioFile_Id = c.Int(),
                        AudioFile_Id1 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AudioFiles", t => t.AudioFile_Id)
                .ForeignKey("dbo.AudioFiles", t => t.AudioFile_Id1)
                .Index(t => t.AudioFile_Id)
                .Index(t => t.AudioFile_Id1);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AlbumCovers", "AudioFile_Id", "dbo.AudioFiles");
            DropForeignKey("dbo.LRCFiles", "AudioFile_Id1", "dbo.AudioFiles");
            DropForeignKey("dbo.LRCFiles", "AudioFile_Id", "dbo.AudioFiles");
            DropIndex("dbo.LRCFiles", new[] { "AudioFile_Id1" });
            DropIndex("dbo.LRCFiles", new[] { "AudioFile_Id" });
            DropIndex("dbo.AlbumCovers", new[] { "AudioFile_Id" });
            DropTable("dbo.LRCFiles");
            DropTable("dbo.AudioFiles");
            DropTable("dbo.AlbumCovers");
        }
    }
}
