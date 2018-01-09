using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.Model
{
    public class Points
    {
        public int Id { get; set; }

        [Index("IX_ValueAndFromCountryId", 1, IsUnique = true)]
        public int Value { get; set; }

        [Index("IX_ValueAndFromCountryId", 2, IsUnique = true)]
        [Index("IX_FromCountryIdAndToCountryId", 1, IsUnique = true)]
        public int? FromCountryId { get; set; }
        public virtual Country FromCountry { get; set; }

        [Index("IX_FromCountryIdAndToCountryId", 2, IsUnique = true)]
        public int ToCountryId { get; set; }
        public virtual Country ToCountry { get; set; }
    }
}
