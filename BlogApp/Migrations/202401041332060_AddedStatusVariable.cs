namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStatusVariable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "_CommentStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Posts", "PostStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "PostStatus");
            DropColumn("dbo.Comments", "_CommentStatus");
        }
    }
}
