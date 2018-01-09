using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.Model
{
    public class ScoreBoardDbContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Points> Points { get; set; }

        public ScoreBoardDbContext() : base()
        {
            var applicationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EuroSing");
            Directory.CreateDirectory(applicationPath);
            AppDomain.CurrentDomain.SetData("DataDirectory", applicationPath);
        }
    }
}
