using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.Model
{
    public class Points
    {
        public int Id { get; set; }
        public int Value { get; set; }

        public int FromCountryId { get; set; }
        public virtual Country FromCountry { get; set; }

        public int ToCountryId { get; set; }
        public virtual Country ToCountry { get; set; }
    }
}
