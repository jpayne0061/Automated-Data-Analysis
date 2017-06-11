namespace CorrelationStation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addKeyValuePropToAnova : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KeyValues", "AnovaStats_Id", c => c.Int());
            CreateIndex("dbo.KeyValues", "AnovaStats_Id");
            AddForeignKey("dbo.KeyValues", "AnovaStats_Id", "dbo.AnovaStats", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KeyValues", "AnovaStats_Id", "dbo.AnovaStats");
            DropIndex("dbo.KeyValues", new[] { "AnovaStats_Id" });
            DropColumn("dbo.KeyValues", "AnovaStats_Id");
        }
    }
}
