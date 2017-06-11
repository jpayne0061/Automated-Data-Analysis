namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedRelationhipToUserAndStatSummaries : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StatSummaryVMs", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.StatSummaryVMs", "ApplicationUserId");
            AddForeignKey("dbo.StatSummaryVMs", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StatSummaryVMs", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.StatSummaryVMs", new[] { "ApplicationUserId" });
            DropColumn("dbo.StatSummaryVMs", "ApplicationUserId");
        }
    }
}
