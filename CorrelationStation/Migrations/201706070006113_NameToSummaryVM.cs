namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NameToSummaryVM : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StatSummaryVMs", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StatSummaryVMs", "Name");
        }
    }
}
