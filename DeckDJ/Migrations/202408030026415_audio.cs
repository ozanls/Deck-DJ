namespace DeckDJ.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class audio : DbMigration
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
                        Deck_DeckId = c.Int(),
                    })
                .PrimaryKey(t => t.AudioId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Decks", t => t.Deck_DeckId)
                .Index(t => t.CategoryId)
                .Index(t => t.Deck_DeckId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Audios", "Deck_DeckId", "dbo.Decks");
            DropForeignKey("dbo.Audios", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Audios", new[] { "Deck_DeckId" });
            DropIndex("dbo.Audios", new[] { "CategoryId" });
            DropTable("dbo.Categories");
            DropTable("dbo.Audios");
        }
    }
}
