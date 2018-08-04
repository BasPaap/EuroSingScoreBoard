using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.Model
{
    public class Vote
    {
        public int Id { get; set; }

        [Index("IX_NumPointsAndFromCountryId", 1, IsUnique = true)] //The combination of NumPoints and FromCountryId has to be unique, because a country can only issue each number of points once.
        public int NumPoints { get; set; }

        private int? fromCountryId;

        [Index("IX_NumPointsAndFromCountryId", 2, IsUnique = true)] //The combination of NumPoints and FromCountryId has to be unique, because a country can only issue each number of points once.
        [Index("IX_FromCountryIdAndToCountryId", 1, IsUnique = true)] // The combination of FromCountry and ToCountry has to be unique, because a country can only issue votes to each country once.
        public int? FromCountryId { get { return fromCountryId; } set { fromCountryId = value; } }
        public virtual Country FromCountry { get; set; }

        [Index("IX_FromCountryIdAndToCountryId", 2, IsUnique = true)] // The combination of FromCountry and ToCountry has to be unique, because a country can only issue votes to each country once.
        public int ToCountryId { get; set; }
        public virtual Country ToCountry { get; set; }

        public override string ToString()
        {
            return $"{{Vote}} ({NumPoints} points from {FromCountry.Name} to {ToCountry.Name}}}";
        } 
    }
}
