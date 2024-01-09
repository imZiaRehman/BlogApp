namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedReportCommentTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReportComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommenntId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        ReportReason = c.String(),
                        ReportedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ReportComments");
        }
    }
}
