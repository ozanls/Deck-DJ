namespace DeckDJ.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AudioExtension : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Audios", "AudioHasAudio", c => c.Boolean(nullable: false));
            AddColumn("dbo.Audios", "AudioExtension", c => c.String());
            DropColumn("dbo.Audios", "AudioURL");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Audios", "AudioURL", c => c.String());
            DropColumn("dbo.Audios", "AudioExtension");
            DropColumn("dbo.Audios", "AudioHasAudio");
        }
    }
}
