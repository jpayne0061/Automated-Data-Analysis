namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedCreatedOnAndDescriptionToStatSummary : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StatSummaryVMs", "CreatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.StatSummaryVMs", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StatSummaryVMs", "Description");
            DropColumn("dbo.StatSummaryVMs", "CreatedOn");
        }
    }
}
