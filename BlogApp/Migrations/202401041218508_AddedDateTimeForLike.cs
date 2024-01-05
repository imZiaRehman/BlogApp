namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDateTimeForLike : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Likes", "CreatedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Likes", "CreatedAt");
        }
    }
}
