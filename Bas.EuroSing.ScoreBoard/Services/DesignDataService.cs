using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bas.EuroSing.ScoreBoard.Model;

namespace Bas.EuroSing.ScoreBoard.Services
{
    internal sealed class DesignDataService : IDataService
    {
        public async Task<int> AddCountryAsync(Country country)
        {
            var db = new ScoreBoardDbContext();

            db.Countries.Add(country);
            await db.SaveChangesAsync();

            return country.Id;
        }

        public Collection<Country> GetAllCountries()
        {
            return new Collection<Country>(new[] {
                new Country() { Name = "Nederland" },
                new Country() { Name = "Cyprus" }
            });
        }
    }
}
