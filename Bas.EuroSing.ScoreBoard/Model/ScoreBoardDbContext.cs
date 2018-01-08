using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.Model
{
    public class ScoreBoardDbContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Points> Points { get; set; }
    }
}
