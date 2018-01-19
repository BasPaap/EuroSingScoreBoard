using Bas.EuroSing.ScoreBoard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.Messages
{
    internal class SetNextCountryMessage
    {
        public CountryResultsControlViewModel Country { get; set; }
        public SetNextCountryMessage(CountryResultsControlViewModel country)
        {
            Country = country;
        }
    }
}
