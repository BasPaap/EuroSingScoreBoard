using Bas.EuroSing.ScoreBoard.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.Messages
{
    internal class ChangeStateMessage
    {
        public ResultsState State { get; set; }
        public Country CurrentCountry { get; set; }
        public ChangeStateMessage(ResultsState state, Country currentCountry)
        {
            State = state;
            CurrentCountry = currentCountry;
        }
    }
}
