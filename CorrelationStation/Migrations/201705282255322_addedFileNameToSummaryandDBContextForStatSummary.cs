namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedFileNameToSummaryandDBContextForStatSummary : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StatSummaryVMs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        FileName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AnovaStats", "StatSummaryVM_Id", c => c.Int());
            AddColumn("dbo.ChiStats", "StatSummaryVM_Id", c => c.Int());
            AddColumn("dbo.PearsonCorrs", "StatSummaryVM_Id", c => c.Int());
            CreateIndex("dbo.AnovaStats", "StatSummaryVM_Id");
            CreateIndex("dbo.ChiStats", "StatSummaryVM_Id");
            CreateIndex("dbo.PearsonCorrs", "StatSummaryVM_Id");
            AddForeignKey("dbo.AnovaStats", "StatSummaryVM_Id", "dbo.StatSummaryVMs", "Id");
            AddForeignKey("dbo.ChiStats", "StatSummaryVM_Id", "dbo.StatSummaryVMs", "Id");
            AddForeignKey("dbo.PearsonCorrs", "StatSummaryVM_Id", "dbo.StatSummaryVMs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PearsonCorrs", "StatSummaryVM_Id", "dbo.StatSummaryVMs");
            DropForeignKey("dbo.ChiStats", "StatSummaryVM_Id", "dbo.StatSummaryVMs");
            DropForeignKey("dbo.AnovaStats", "StatSummaryVM_Id", "dbo.StatSummaryVMs");
            DropIndex("dbo.PearsonCorrs", new[] { "StatSummaryVM_Id" });
            DropIndex("dbo.ChiStats", new[] { "StatSummaryVM_Id" });
            DropIndex("dbo.AnovaStats", new[] { "StatSummaryVM_Id" });
            DropColumn("dbo.PearsonCorrs", "StatSummaryVM_Id");
            DropColumn("dbo.ChiStats", "StatSummaryVM_Id");
            DropColumn("dbo.AnovaStats", "StatSummaryVM_Id");
            DropTable("dbo.StatSummaryVMs");
        }
    }
}
