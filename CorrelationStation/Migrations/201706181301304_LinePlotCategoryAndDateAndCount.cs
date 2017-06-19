namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinePlotCategoryAndDateAndCount : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CategoryCounts", "TimePeriod_Id", "dbo.TimePeriods");
            DropForeignKey("dbo.TimePeriods", "DateAndCategory_Id", "dbo.DateAndCategories");
            DropIndex("dbo.CategoryCounts", new[] { "TimePeriod_Id" });
            DropIndex("dbo.TimePeriods", new[] { "DateAndCategory_Id" });
            CreateTable(
                "dbo.LinePlotCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DateAndCategory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DateAndCategories", t => t.DateAndCategory_Id)
                .Index(t => t.DateAndCategory_Id);
            
            CreateTable(
                "dbo.DateAndCounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                        DateTime = c.DateTime(nullable: false),
                        MonthAndYear = c.String(),
                        Count = c.Int(nullable: false),
                        linePlotCategory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LinePlotCategories", t => t.linePlotCategory_Id)
                .Index(t => t.linePlotCategory_Id);
            
            DropTable("dbo.CategoryCounts");
            DropTable("dbo.TimePeriods");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TimePeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateString = c.String(),
                        Date = c.DateTime(nullable: false),
                        DateAndCategory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CategoryCounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Count = c.Int(nullable: false),
                        TimePeriod_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.LinePlotCategories", "DateAndCategory_Id", "dbo.DateAndCategories");
            DropForeignKey("dbo.DateAndCounts", "linePlotCategory_Id", "dbo.LinePlotCategories");
            DropIndex("dbo.DateAndCounts", new[] { "linePlotCategory_Id" });
            DropIndex("dbo.LinePlotCategories", new[] { "DateAndCategory_Id" });
            DropTable("dbo.DateAndCounts");
            DropTable("dbo.LinePlotCategories");
            CreateIndex("dbo.TimePeriods", "DateAndCategory_Id");
            CreateIndex("dbo.CategoryCounts", "TimePeriod_Id");
            AddForeignKey("dbo.TimePeriods", "DateAndCategory_Id", "dbo.DateAndCategories", "Id");
            AddForeignKey("dbo.CategoryCounts", "TimePeriod_Id", "dbo.TimePeriods", "Id");
        }
    }
}
