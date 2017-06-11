namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class statModelsAndKVPModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnovaStats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoricalVariable = c.String(),
                        NumericalVariable = c.String(),
                        FStat = c.Double(nullable: false),
                        CriticalValueAtAlphaZeroFive = c.Double(nullable: false),
                        SigAtPointZeroFive = c.Boolean(nullable: false),
                        SigAtPointZeroOne = c.Boolean(nullable: false),
                        SignificantResult = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChiStats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Variable1 = c.String(),
                        Variable2 = c.String(),
                        PValue = c.Double(nullable: false),
                        ChiStatistic = c.Double(nullable: false),
                        SignificantResult = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.KeyValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.Double(nullable: false),
                        ChiStats_Id = c.Int(),
                        ChiStats_Id1 = c.Int(),
                        ChiStats_Id2 = c.Int(),
                        ChiStats_Id3 = c.Int(),
                        ChiStats_Id4 = c.Int(),
                        ChiStats_Id5 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChiStats", t => t.ChiStats_Id)
                .ForeignKey("dbo.ChiStats", t => t.ChiStats_Id1)
                .ForeignKey("dbo.ChiStats", t => t.ChiStats_Id2)
                .ForeignKey("dbo.ChiStats", t => t.ChiStats_Id3)
                .ForeignKey("dbo.ChiStats", t => t.ChiStats_Id4)
                .ForeignKey("dbo.ChiStats", t => t.ChiStats_Id5)
                .Index(t => t.ChiStats_Id)
                .Index(t => t.ChiStats_Id1)
                .Index(t => t.ChiStats_Id2)
                .Index(t => t.ChiStats_Id3)
                .Index(t => t.ChiStats_Id4)
                .Index(t => t.ChiStats_Id5);
            
            CreateTable(
                "dbo.PearsonCorrs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        r = c.Double(nullable: false),
                        Variable1 = c.String(),
                        Variable2 = c.String(),
                        SignificantResult = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KeyValues", "ChiStats_Id5", "dbo.ChiStats");
            DropForeignKey("dbo.KeyValues", "ChiStats_Id4", "dbo.ChiStats");
            DropForeignKey("dbo.KeyValues", "ChiStats_Id3", "dbo.ChiStats");
            DropForeignKey("dbo.KeyValues", "ChiStats_Id2", "dbo.ChiStats");
            DropForeignKey("dbo.KeyValues", "ChiStats_Id1", "dbo.ChiStats");
            DropForeignKey("dbo.KeyValues", "ChiStats_Id", "dbo.ChiStats");
            DropIndex("dbo.KeyValues", new[] { "ChiStats_Id5" });
            DropIndex("dbo.KeyValues", new[] { "ChiStats_Id4" });
            DropIndex("dbo.KeyValues", new[] { "ChiStats_Id3" });
            DropIndex("dbo.KeyValues", new[] { "ChiStats_Id2" });
            DropIndex("dbo.KeyValues", new[] { "ChiStats_Id1" });
            DropIndex("dbo.KeyValues", new[] { "ChiStats_Id" });
            DropTable("dbo.PearsonCorrs");
            DropTable("dbo.KeyValues");
            DropTable("dbo.ChiStats");
            DropTable("dbo.AnovaStats");
        }
    }
}
