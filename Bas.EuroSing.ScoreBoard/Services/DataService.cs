using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bas.EuroSing.ScoreBoard.Model;

namespace Bas.EuroSing.ScoreBoard.Services
{
    internal sealed class DataService : IDataService
    {
        public async Task<int> AddCountryAsync(Country country)
        {
            var db = new ScoreBoardDbContext();

            db.Countries.Add(country);
            await db.SaveChangesAsync();

            return country.Id;
        }

        public async Task ChangeCountryNameAsync(int id, string name)
        {
            var db = new ScoreBoardDbContext();

            var country = db.Countries.Find(id);
            country.Name = name;
            await db.SaveChangesAsync();
        }

        public async Task DeleteAllVotesAsync()
        {
            var db = new ScoreBoardDbContext();

            db.Points.RemoveRange(db.Points);
            await db.SaveChangesAsync();
        }

        public async Task DeleteCountryAsync(int id)
        {
            var db = new ScoreBoardDbContext();
            var country = await db.Countries.FindAsync(id);
            db.Countries.Remove(country);

            await db.SaveChangesAsync();
        }

        public Collection<Country> GetAllCountries()
        {
            var db = new ScoreBoardDbContext();

            return new Collection<Country>(db.Countries.ToList());
        }
    }
}
