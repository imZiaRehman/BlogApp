namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPostSuggestionReplies : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SuggestionReplies",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        SuggestionText = c.String(),
                        userId = c.Int(nullable: false),
                        SuggestionId = c.Int(nullable: false),
                        PostSuggestion_Id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.PostSuggestions", t => t.PostSuggestion_Id)
                .Index(t => t.PostSuggestion_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SuggestionReplies", "PostSuggestion_Id", "dbo.PostSuggestions");
            DropIndex("dbo.SuggestionReplies", new[] { "PostSuggestion_Id" });
            DropTable("dbo.SuggestionReplies");
        }
    }
}
