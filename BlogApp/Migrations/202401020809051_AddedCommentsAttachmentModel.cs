namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCommentsAttachmentModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommentAttachments",
                c => new
                    {
                        CommentAttachmentId = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        FilePath = c.String(),
                        CommentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CommentAttachmentId)
                .ForeignKey("dbo.Comments", t => t.CommentId, cascadeDelete: true)
                .Index(t => t.CommentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommentAttachments", "CommentId", "dbo.Comments");
            DropIndex("dbo.CommentAttachments", new[] { "CommentId" });
            DropTable("dbo.CommentAttachments");
        }
    }
}
