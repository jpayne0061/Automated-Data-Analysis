namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedClassJustToStoreAnIntBecauseEntityFrameWorkCantStorePrimitiveTypesInAList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnInts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Double(nullable: false),
                        PearsonCorr_Id = c.Int(),
                        PearsonCorr_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PearsonCorrs", t => t.PearsonCorr_Id)
                .ForeignKey("dbo.PearsonCorrs", t => t.PearsonCorr_Id1)
                .Index(t => t.PearsonCorr_Id)
                .Index(t => t.PearsonCorr_Id1);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AnInts", "PearsonCorr_Id1", "dbo.PearsonCorrs");
            DropForeignKey("dbo.AnInts", "PearsonCorr_Id", "dbo.PearsonCorrs");
            DropIndex("dbo.AnInts", new[] { "PearsonCorr_Id1" });
            DropIndex("dbo.AnInts", new[] { "PearsonCorr_Id" });
            DropTable("dbo.AnInts");
        }
    }
}
