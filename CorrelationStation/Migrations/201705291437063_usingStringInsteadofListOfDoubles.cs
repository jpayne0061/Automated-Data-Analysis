namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usingStringInsteadofListOfDoubles : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AnInts", "PearsonCorr_Id", "dbo.PearsonCorrs");
            DropForeignKey("dbo.AnInts", "PearsonCorr_Id1", "dbo.PearsonCorrs");
            DropIndex("dbo.AnInts", new[] { "PearsonCorr_Id" });
            DropIndex("dbo.AnInts", new[] { "PearsonCorr_Id1" });
            AddColumn("dbo.PearsonCorrs", "Variable1Data", c => c.String());
            AddColumn("dbo.PearsonCorrs", "Variable2Data", c => c.String());
            DropColumn("dbo.AnInts", "PearsonCorr_Id");
            DropColumn("dbo.AnInts", "PearsonCorr_Id1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AnInts", "PearsonCorr_Id1", c => c.Int());
            AddColumn("dbo.AnInts", "PearsonCorr_Id", c => c.Int());
            DropColumn("dbo.PearsonCorrs", "Variable2Data");
            DropColumn("dbo.PearsonCorrs", "Variable1Data");
            CreateIndex("dbo.AnInts", "PearsonCorr_Id1");
            CreateIndex("dbo.AnInts", "PearsonCorr_Id");
            AddForeignKey("dbo.AnInts", "PearsonCorr_Id1", "dbo.PearsonCorrs", "Id");
            AddForeignKey("dbo.AnInts", "PearsonCorr_Id", "dbo.PearsonCorrs", "Id");
        }
    }
}
