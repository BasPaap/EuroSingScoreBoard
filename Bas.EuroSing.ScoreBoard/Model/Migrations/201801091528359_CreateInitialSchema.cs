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
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Points",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Int(nullable: false),
                        FromCountryId = c.Int(),
                        ToCountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.FromCountryId)
                .ForeignKey("dbo.Countries", t => t.ToCountryId, cascadeDelete: true)
                .Index(t => new { t.Value, t.FromCountryId }, unique: true, name: "IX_ValueAndFromCountryId")
                .Index(t => new { t.FromCountryId, t.ToCountryId }, unique: true, name: "IX_FromCountryIdAndToCountryId");            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Points", "ToCountryId", "dbo.Countries");
            DropForeignKey("dbo.Points", "FromCountryId", "dbo.Countries");
            DropIndex("dbo.Points", "IX_FromCountryIdAndToCountryId");
            DropIndex("dbo.Points", "IX_ValueAndFromCountryId");
            DropTable("dbo.Points");
            DropTable("dbo.Countries");
        }
    }
}
