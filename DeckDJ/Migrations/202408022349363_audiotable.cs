namespace DeckDJ.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class audiotable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Audios",
                c => new
                    {
                        AudioId = c.Int(nullable: false, identity: true),
                        AudioName = c.String(),
                        AudioURL = c.String(),
                        AudioLength = c.Int(nullable: false),
                        AudioTimestamp = c.DateTime(nullable: false),
                        AudioStreams = c.Int(nullable: false),
                        AudioUploaderId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AudioId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            AddColumn("dbo.Decks", "AudioId", c => c.Int(nullable: false));
            CreateIndex("dbo.Decks", "AudioId");
            AddForeignKey("dbo.Decks", "AudioId", "dbo.Audios", "AudioId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Decks", "AudioId", "dbo.Audios");
            DropForeignKey("dbo.Audios", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Decks", new[] { "AudioId" });
            DropIndex("dbo.Audios", new[] { "CategoryId" });
            DropColumn("dbo.Decks", "AudioId");
            DropTable("dbo.Categories");
            DropTable("dbo.Audios");
        }
    }
}
