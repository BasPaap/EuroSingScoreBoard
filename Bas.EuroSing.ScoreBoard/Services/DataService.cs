using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bas.EuroSing.ScoreBoard.Model;
using System.Diagnostics;

namespace Bas.EuroSing.ScoreBoard.Services
{
    internal sealed class DataService : IDataService
    {
        public async Task<int> AddCountryAsync(Country country)
        {
            var db = new ScoreBoardDbContext();

            db.Countries.Add(country);
            await db.SaveChangesAsync();

            return country.Id;
        }

        public async Task ChangeCountryNameAsync(int id, string name)
        {
            var db = new ScoreBoardDbContext();

            var country = db.Countries.Find(id);
            country.Name = name;
            await db.SaveChangesAsync();
        }

        public async Task DeleteAllVotesAsync()
        {
            var db = new ScoreBoardDbContext();

            db.Votes.RemoveRange(db.Votes);
            await db.SaveChangesAsync();
        }

        public async Task DeleteCountryAsync(int id)
        {
            var db = new ScoreBoardDbContext();
            var country = await db.Countries.FindAsync(id);
            var votes = db.Votes.Where(v => v.FromCountryId == id).ToList();

            db.Votes.RemoveRange(votes);
            db.Countries.Remove(country);

            await db.SaveChangesAsync();
        }

        public Collection<Country> GetAllCountries()
        {
            var db = new ScoreBoardDbContext();

            return new Collection<Country>(db.Countries.ToList());
        }

        public Collection<Vote> GetVotes(int countryIssuingVotesId)
        {
            var db = new ScoreBoardDbContext();

            var issuedVotes = (from v in db.Votes
                               where v.FromCountryId == countryIssuingVotesId
                               select v).ToList();

            var toCountryIds = issuedVotes.Select(i => i.ToCountryId).ToList();
            var countriesToIssueVotesFor = from c in db.Countries
                                           where c.Id != countryIssuingVotesId &&
                                                 !toCountryIds.Contains(c.Id)
                                           select c;

            var votesToIssue = (from c in countriesToIssueVotesFor.ToList()
                                select new Vote()
                                {
                                    FromCountryId = countryIssuingVotesId,
                                    FromCountry = db.Countries.Find(countryIssuingVotesId),
                                    ToCountryId = c.Id,
                                    ToCountry = c
                                }).ToList();

            Debug.Assert(issuedVotes.Select(i => i.ToCountryId).Intersect(votesToIssue.Select(v => v.ToCountryId)).Count() == 0, "issuedVotes and votesToIssue have overlapping countries");
            Debug.Assert(issuedVotes.Count(i => i.FromCountryId != countryIssuingVotesId) == 0, "issuedVotes contains vote from wrong country");
            Debug.Assert(votesToIssue.Count(i => i.FromCountryId != countryIssuingVotesId) == 0, "votesToIssue contains vote from wrong country");
            Debug.Assert(issuedVotes.Count(i => i.ToCountryId == countryIssuingVotesId) == 0, "issuedVotes contains vote to wrong country");
            Debug.Assert(votesToIssue.Count(i => i.ToCountryId == countryIssuingVotesId) == 0, "votesToIssue contains vote to wrong country");
            Debug.Assert(votesToIssue.Count() + issuedVotes.Count() == db.Countries.Count() - 1, "wrong amount of votes in total.");

            return new Collection<Vote>(issuedVotes.Concat(votesToIssue).ToList());
        }

        public void SaveVote(Vote vote, string numPoints)
        {
            var db = new ScoreBoardDbContext();

            vote.NumPoints = int.TryParse(numPoints, out int points) ? points : 0;

            if (vote.Id == 0)
            {
                if (vote.NumPoints > 0)
                {
                    db.Countries.Attach(vote.FromCountry);  // Laat EF weten dat FromCountry al bestaat (zodat hij 'm niet opnieuw invoegt)
                    db.Countries.Attach(vote.ToCountry);    // Laat EF weten dat ToCountry al bestaat (zodat hij 'm niet opnieuw invoegt)
                    db.Votes.Add(vote);
                }
            }
            else
            {
                var existingVote = db.Votes.Find(vote.Id);
                if (existingVote != null)
                {
                    db.Votes.Remove(existingVote);
                }

                if (vote.NumPoints > 0)
                {
                    db.Countries.Attach(vote.FromCountry);  // Laat EF weten dat FromCountry al bestaat (zodat hij 'm niet opnieuw invoeg
                    db.Countries.Attach(vote.ToCountry);    // Laat EF weten dat ToCountry al bestaat (zodat hij 'm niet opnieuw invoegt)
                    db.Votes.Add(vote);
                }
            }

            db.SaveChanges();
        }
    }
}
