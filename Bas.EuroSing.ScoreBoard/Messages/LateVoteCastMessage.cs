using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.Messages
{
    internal class LateVoteCastMessage
    {
        public int FromCountryId { get; set; }
        public int ToCountryId { get; set; }
        public int NumPoints { get; set; }

        public LateVoteCastMessage(int fromCountryId, int toCountryId, int numPoints)
        {
            FromCountryId = fromCountryId;
            ToCountryId = toCountryId;
            NumPoints = numPoints;
        }
    }
}
