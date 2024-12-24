namespace kaaraoke_app.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAudioFileIdToLRCFile : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LRCFiles", "AudioFile_Id", "dbo.AudioFiles");
            DropIndex("dbo.LRCFiles", new[] { "AudioFile_Id" });
            DropIndex("dbo.LRCFiles", new[] { "AudioFile_Id1" });
            DropColumn("dbo.LRCFiles", "AudioFile_Id");
            RenameColumn(table: "dbo.LRCFiles", name: "AudioFile_Id1", newName: "AudioFile_Id");
            AddColumn("dbo.LRCFiles", "AudioFileId", c => c.Int(nullable: false));
            AlterColumn("dbo.LRCFiles", "AudioFile_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.LRCFiles", "AudioFileId");
            CreateIndex("dbo.LRCFiles", "AudioFile_Id");
            AddForeignKey("dbo.LRCFiles", "AudioFileId", "dbo.AudioFiles", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LRCFiles", "AudioFileId", "dbo.AudioFiles");
            DropIndex("dbo.LRCFiles", new[] { "AudioFile_Id" });
            DropIndex("dbo.LRCFiles", new[] { "AudioFileId" });
            AlterColumn("dbo.LRCFiles", "AudioFile_Id", c => c.Int());
            DropColumn("dbo.LRCFiles", "AudioFileId");
            RenameColumn(table: "dbo.LRCFiles", name: "AudioFile_Id", newName: "AudioFile_Id1");
            AddColumn("dbo.LRCFiles", "AudioFile_Id", c => c.Int());
            CreateIndex("dbo.LRCFiles", "AudioFile_Id1");
            CreateIndex("dbo.LRCFiles", "AudioFile_Id");
            AddForeignKey("dbo.LRCFiles", "AudioFile_Id", "dbo.AudioFiles", "Id");
        }
    }
}
