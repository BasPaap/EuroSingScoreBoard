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

        public ChangeStateMessage(ResultsState state)
        {
            State = state;
        }
    }
}
