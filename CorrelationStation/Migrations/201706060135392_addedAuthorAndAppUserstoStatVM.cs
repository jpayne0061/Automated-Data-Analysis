namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedAuthorAndAppUserstoStatVM : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StatSummaryVMs", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.StatSummaryVMs", new[] { "ApplicationUserId" });
            CreateTable(
                "dbo.ApplicationUserStatSummaryVMs",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        StatSummaryVM_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.StatSummaryVM_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.StatSummaryVMs", t => t.StatSummaryVM_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.StatSummaryVM_Id);
            
            AddColumn("dbo.StatSummaryVMs", "Author", c => c.String());
            DropColumn("dbo.StatSummaryVMs", "ApplicationUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StatSummaryVMs", "ApplicationUserId", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ApplicationUserStatSummaryVMs", "StatSummaryVM_Id", "dbo.StatSummaryVMs");
            DropForeignKey("dbo.ApplicationUserStatSummaryVMs", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserStatSummaryVMs", new[] { "StatSummaryVM_Id" });
            DropIndex("dbo.ApplicationUserStatSummaryVMs", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.StatSummaryVMs", "Author");
            DropTable("dbo.ApplicationUserStatSummaryVMs");
            CreateIndex("dbo.StatSummaryVMs", "ApplicationUserId");
            AddForeignKey("dbo.StatSummaryVMs", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
    }
}
