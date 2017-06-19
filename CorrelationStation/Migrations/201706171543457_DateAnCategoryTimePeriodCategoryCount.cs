namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateAnCategoryTimePeriodCategoryCount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CategoryCounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Count = c.Int(nullable: false),
                        TimePeriod_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TimePeriods", t => t.TimePeriod_Id)
                .Index(t => t.TimePeriod_Id);
            
            CreateTable(
                "dbo.DateAndCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Variable1 = c.String(),
                        Variable2 = c.String(),
                        StatSummaryVM_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StatSummaryVMs", t => t.StatSummaryVM_Id)
                .Index(t => t.StatSummaryVM_Id);
            
            CreateTable(
                "dbo.TimePeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateString = c.String(),
                        Date = c.DateTime(nullable: false),
                        DateAndCategory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DateAndCategories", t => t.DateAndCategory_Id)
                .Index(t => t.DateAndCategory_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DateAndCategories", "StatSummaryVM_Id", "dbo.StatSummaryVMs");
            DropForeignKey("dbo.TimePeriods", "DateAndCategory_Id", "dbo.DateAndCategories");
            DropForeignKey("dbo.CategoryCounts", "TimePeriod_Id", "dbo.TimePeriods");
            DropIndex("dbo.TimePeriods", new[] { "DateAndCategory_Id" });
            DropIndex("dbo.DateAndCategories", new[] { "StatSummaryVM_Id" });
            DropIndex("dbo.CategoryCounts", new[] { "TimePeriod_Id" });
            DropTable("dbo.TimePeriods");
            DropTable("dbo.DateAndCategories");
            DropTable("dbo.CategoryCounts");
        }
    }
}
