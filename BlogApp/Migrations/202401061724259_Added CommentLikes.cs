namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCommentLikes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommentLikes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        commentId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Comments", t => t.commentId, cascadeDelete: true)
                .Index(t => t.commentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommentLikes", "commentId", "dbo.Comments");
            DropIndex("dbo.CommentLikes", new[] { "commentId" });
            DropTable("dbo.CommentLikes");
        }
    }
}
