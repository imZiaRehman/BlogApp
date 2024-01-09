namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCurrentStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "currentStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "currentStatus");
        }
    }
}
