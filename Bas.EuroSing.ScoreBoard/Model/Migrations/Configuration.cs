namespace Bas.EuroSing.ScoreBoard.Model.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Bas.EuroSing.ScoreBoard.Model.ScoreBoardDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            this.MigrationsDirectory = "Model\\Migrations";
            this.MigrationsNamespace = "Bas.EuroSing.ScoreBoard.Model.Migrations";
        }

        protected override void Seed(Bas.EuroSing.ScoreBoard.Model.ScoreBoardDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
