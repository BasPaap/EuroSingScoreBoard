namespace Bas.EuroSing.ScoreBoard.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateInitialSchema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        FlagImage = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Votes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumPoints = c.Int(nullable: false),
                        FromCountryId = c.Int(),
                        ToCountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.FromCountryId)
                .ForeignKey("dbo.Countries", t => t.ToCountryId, cascadeDelete: true)
                .Index(t => new { t.NumPoints, t.FromCountryId }, unique: true, name: "IX_NumPointsAndFromCountryId")
                .Index(t => new { t.FromCountryId, t.ToCountryId }, unique: true, name: "IX_FromCountryIdAndToCountryId");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Votes", "ToCountryId", "dbo.Countries");
            DropForeignKey("dbo.Votes", "FromCountryId", "dbo.Countries");
            DropIndex("dbo.Votes", "IX_FromCountryIdAndToCountryId");
            DropIndex("dbo.Votes", "IX_NumPointsAndFromCountryId");
            DropTable("dbo.Votes");
            DropTable("dbo.Countries");
        }
    }
}
