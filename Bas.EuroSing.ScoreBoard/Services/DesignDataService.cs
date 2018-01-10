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
        public Task<int> AddCountryAsync(Country country)
        {
            throw new NotImplementedException();
        }

        public Task ChangeCountryNameAsync(int id, string name)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAllVotesAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteCountryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Collection<Country> GetAllCountries()
        {
            return new Collection<Country>(new[] 
            {
                new Country() { Name = "Nederland" },
                new Country() { Name = "Cyprus" }
            });
        }

        public Collection<Vote> GetVotes(int countryIssuingVotesId)
        {
            return new Collection<Vote>(new[]
            {
                new Vote()
                {
                    FromCountry = new Country() { Name = "Nederland" },
                    ToCountry = new Country() { Name = "Cyprus" },
                    NumPoints = 12
                }
            });
        }
    }
}
