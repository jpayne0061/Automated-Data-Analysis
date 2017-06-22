namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateAndNumerals : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DateAndNumerals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateData = c.String(),
                        NumeralData = c.String(),
                        DateName = c.String(),
                        NumeralName = c.String(),
                        StatSummaryVM_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StatSummaryVMs", t => t.StatSummaryVM_Id)
                .Index(t => t.StatSummaryVM_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DateAndNumerals", "StatSummaryVM_Id", "dbo.StatSummaryVMs");
            DropIndex("dbo.DateAndNumerals", new[] { "StatSummaryVM_Id" });
            DropTable("dbo.DateAndNumerals");
        }
    }
}
