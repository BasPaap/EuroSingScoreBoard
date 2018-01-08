namespace Bas.EuroSing.ScoreBoard.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
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
                        FromCountryId = c.Int(nullable: false),
                        ToCountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.FromCountryId, cascadeDelete: true)
                .ForeignKey("dbo.Countries", t => t.ToCountryId, cascadeDelete: true)
                .Index(t => t.FromCountryId)
                .Index(t => t.ToCountryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Points", "ToCountryId", "dbo.Countries");
            DropForeignKey("dbo.Points", "FromCountryId", "dbo.Countries");
            DropIndex("dbo.Points", new[] { "ToCountryId" });
            DropIndex("dbo.Points", new[] { "FromCountryId" });
            DropTable("dbo.Points");
            DropTable("dbo.Countries");
        }
    }
}
