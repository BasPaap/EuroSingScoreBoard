using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.Messages
{
    internal class VotesToCastUpdatedMessage
    {
        public Collection<int> VotesToCast { get; set; }

        public VotesToCastUpdatedMessage(IEnumerable<int> votesToCast)
        {
            VotesToCast = new Collection<int>(votesToCast.ToList());
        }
    }
}
