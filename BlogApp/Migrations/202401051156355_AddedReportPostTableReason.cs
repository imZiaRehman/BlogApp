namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedReportPostTableReason : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReportPosts",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    PostId = c.Int(nullable: false),
                    UserId = c.Int(nullable: false),
                    ReportReason = c.String(),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.ReportPosts");
        }
    }
}
