using Bas.EuroSing.ScoreBoard.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.Services
{
    internal interface IDataService
    {
        Collection<Country> GetAllCountries();
        Task<int> AddCountryAsync(Country country);
        Task DeleteAllVotesAsync();
        Task DeleteCountryAsync(int id);
        Task ChangeCountryNameAsync(int id, string name);
        Collection<Vote> GetVotes(int countryIssuingVotesId);
        void SaveVote(Vote vote, string numPoints);
    }
}
