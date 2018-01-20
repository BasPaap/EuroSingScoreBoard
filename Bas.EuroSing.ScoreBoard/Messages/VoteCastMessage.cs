using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.Messages
{
    class VoteCastMessage
    {
        public int? CountryId { get; set; }

        public VoteCastMessage(int? countryId)
        {
            CountryId = countryId;
        }
    }
}
