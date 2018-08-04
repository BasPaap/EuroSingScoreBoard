using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bas.EuroSing.ScoreBoard.Model;

namespace Bas.EuroSing.ScoreBoard.Services
{
    // Dataservice to be used at design time. Implements only what is necessary for the designer.
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
                new Country() { Id=1, Name = "Nederland" },
                new Country() { Id=2, Name = "Cyprus" },
                new Country() { Id=3, Name = "Hungary" },
                new Country() { Id=4, Name = "Italy" },
                new Country() { Id=5, Name = "Wyoming" }
            });
        }

        public Collection<Vote> GetVotes(int countryIssuingVotesId)
        {
            return new Collection<Vote>(new[]
            {
                new Vote()
                {
                    FromCountry = new Country()  { Id=2, Name = "Cyprus" },
                    ToCountry = new Country()  { Id=1, Name = "Nederland" },
                    NumPoints = 12
                },
                new Vote()
                {
                    FromCountry = new Country()  { Id=2, Name = "Cyprus" },
                    ToCountry = new Country()  { Id=1, Name = "Hungary" },
                },
                new Vote()
                {
                    FromCountry = new Country()  { Id=2, Name = "Cyprus" },
                    ToCountry = new Country()  { Id=1, Name = "Italy" },
                },
                new Vote()
                {
                    FromCountry = new Country()  { Id=2, Name = "Cyprus" },
                    ToCountry = new Country()  { Id=1, Name = "Wyoming" },
                },
            });
        }

        public void SaveVote(Vote vote, bool force = false)
        {
            throw new NotImplementedException();
        }

        public void DeleteVote(Vote vote)
        {
            throw new NotImplementedException();
        }

        public Country GetCountry(int countryId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, IEnumerable<Vote>> GetAllVotes()
        {
            throw new NotImplementedException();
        }

        public Collection<Country> GetCountriesToGiveVotesTo(int id, int numPoints)
        {
            return new Collection<Country>(new[]
            {
                new Country() { Id = 1, Name = "Nederland" },
                new Country() { Id = 2, Name = "Cyprus" },
                new Country() { Id = 3, Name = "Hungary" }
            });
        }

        public Collection<Vote> GetIssuedVotes(int countryIssuingVotesId)
        {
            throw new NotImplementedException();
        }
    }
}
