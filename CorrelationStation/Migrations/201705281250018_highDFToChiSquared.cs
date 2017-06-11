namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class highDFToChiSquared : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChiStats", "HighDF", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChiStats", "HighDF");
        }
    }
}
