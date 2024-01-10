namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEmailConfirmOptioninUserTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsEmailConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "EmailConfirmationToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "EmailConfirmationToken");
            DropColumn("dbo.Users", "IsEmailConfirmed");
        }
    }
}
