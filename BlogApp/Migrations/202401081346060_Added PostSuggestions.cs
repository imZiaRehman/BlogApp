namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPostSuggestions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PostSuggestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        SuggestionText = c.String(),
                        suggestionStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.PostId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PostSuggestions", "PostId", "dbo.Posts");
            DropIndex("dbo.PostSuggestions", new[] { "PostId" });
            DropTable("dbo.PostSuggestions");
        }
    }
}
