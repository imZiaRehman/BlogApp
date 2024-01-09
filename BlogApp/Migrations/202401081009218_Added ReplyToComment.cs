namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedReplyToComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "ParentCommentId", c => c.Int());
            AddColumn("dbo.Comments", "ParentComment_CommentId", c => c.Int());
            CreateIndex("dbo.Comments", "ParentComment_CommentId");
            AddForeignKey("dbo.Comments", "ParentComment_CommentId", "dbo.Comments", "CommentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "ParentComment_CommentId", "dbo.Comments");
            DropIndex("dbo.Comments", new[] { "ParentComment_CommentId" });
            DropColumn("dbo.Comments", "ParentComment_CommentId");
            DropColumn("dbo.Comments", "ParentCommentId");
        }
    }
}
