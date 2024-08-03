namespace DeckDJ.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manymany : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Audios", "Deck_DeckId", "dbo.Decks");
            DropIndex("dbo.Audios", new[] { "Deck_DeckId" });
            CreateTable(
                "dbo.DeckAudios",
                c => new
                    {
                        Deck_DeckId = c.Int(nullable: false),
                        Audio_AudioId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Deck_DeckId, t.Audio_AudioId })
                .ForeignKey("dbo.Decks", t => t.Deck_DeckId, cascadeDelete: true)
                .ForeignKey("dbo.Audios", t => t.Audio_AudioId, cascadeDelete: true)
                .Index(t => t.Deck_DeckId)
                .Index(t => t.Audio_AudioId);
            
            DropColumn("dbo.Audios", "Deck_DeckId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Audios", "Deck_DeckId", c => c.Int());
            DropForeignKey("dbo.DeckAudios", "Audio_AudioId", "dbo.Audios");
            DropForeignKey("dbo.DeckAudios", "Deck_DeckId", "dbo.Decks");
            DropIndex("dbo.DeckAudios", new[] { "Audio_AudioId" });
            DropIndex("dbo.DeckAudios", new[] { "Deck_DeckId" });
            DropTable("dbo.DeckAudios");
            CreateIndex("dbo.Audios", "Deck_DeckId");
            AddForeignKey("dbo.Audios", "Deck_DeckId", "dbo.Decks", "DeckId");
        }
    }
}
