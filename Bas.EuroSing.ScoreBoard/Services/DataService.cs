﻿using System;
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
        private ScoreBoardDbContext db = new ScoreBoardDbContext();
        public async Task<int> AddCountryAsync(Country country)
        {
            db.Countries.Add(country);
            await db.SaveChangesAsync();

            return country.Id;
        }

        public async Task ChangeCountryNameAsync(int id, string name)
        {
            var country = db.Countries.Find(id);
            country.Name = name;
            await db.SaveChangesAsync();
        }

        public async Task DeleteAllVotesAsync()
        {
            db.Votes.RemoveRange(db.Votes);
            await db.SaveChangesAsync();
        }

        public async Task DeleteCountryAsync(int id)
        {
            var country = await db.Countries.FindAsync(id);
            var votes = db.Votes.Where(v => v.FromCountryId == id).ToList();

            db.Votes.RemoveRange(votes);
            db.Countries.Remove(country);

            await db.SaveChangesAsync();
        }

        public Collection<Country> GetAllCountries()
        {
            return new Collection<Country>(db.Countries.ToList());
        }

        // Returns a list of votes containing both votes that have been issued by this country 
        // and votes that this country has yet to issue.
        public Collection<Vote> GetVotes(int countryIssuingVotesId)
        {
            // Get the votes already issued by this country.
            var issuedVotes = (from v in db.Votes
                               where v.FromCountryId == countryIssuingVotesId
                               select v).ToList();

            // Get the countries to which this country has not yet issued votes.
            var toCountryIds = issuedVotes.Select(i => i.ToCountryId).ToList();
            var countriesToIssueVotesFor = from c in db.Countries
                                           where c.Id != countryIssuingVotesId &&
                                                 !toCountryIds.Contains(c.Id)
                                           select c;

            // Get a list of Votes for each country to which this country has not yet issued votes.
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

            // Return a list containing both cast votes and votes that are yet to be cast by this country.
            return new Collection<Vote>(issuedVotes.Concat(votesToIssue).ToList());
        }

        public void SaveVote(Vote vote, bool force = false)
        {
            if (vote.NumPoints > 0)
            {
                if (vote.Id == 0)
                {
                    //if (force)
                    //{
                    //    var existingVote = db.Votes.FirstOrDefault(v => v.FromCountryId == vote.FromCountryId && v.NumPoints == vote.NumPoints);

                    //    if (existingVote != null)
                    //    {
                    //        db.Votes.Remove(existingVote);
                    //    }
                    //}

                    db.Countries.Attach(vote.FromCountry);
                    db.Countries.Attach(vote.ToCountry);
                    db.Votes.Add(vote);


                }

                db.SaveChanges();
            }
        }

        public void DeleteVote(Vote vote)
        {
            var existingVote = db.Votes.Find(vote.Id);

            if (existingVote != null)
            {
                db.Votes.Remove(vote);
                db.SaveChanges();
            }
        }

        public Country GetCountry(int countryId)
        {
            return db.Countries.Find(countryId);
        }

        public Dictionary<int, IEnumerable<Vote>> GetAllVotes()
        {
            var dictionary = new Dictionary<int, IEnumerable<Vote>>();

            foreach (var country in db.Countries)
            {
                dictionary.Add(country.Id, db.Votes.Where(v => v.FromCountryId == country.Id).OrderBy(v => v.NumPoints));
            }

            return dictionary;
        }

        
        public Collection<Country> GetCountriesToGiveVotesTo(int countryIssuingVotesId, int numPoints)
        {
            var issuedVotes = GetIssuedVotes(countryIssuingVotesId);

            var votedForCountryIds = issuedVotes.Select(v => v.ToCountryId);
            var currentVote = issuedVotes.SingleOrDefault(v => v.NumPoints == numPoints);
            var countries = (from c in db.Countries
                            where c.Id != countryIssuingVotesId &&
                                  !votedForCountryIds.Contains(c.Id)                             
                            orderby c.Name
                            select c).ToList();

            if (currentVote != null)
            {
                countries.Add(currentVote.ToCountry);
            }

            return new Collection<Country>(countries);
        }

        public Collection<Vote> GetIssuedVotes(int countryIssuingVotesId)
        {
            return new Collection<Vote>(db.Votes.Where(v => v.FromCountryId == countryIssuingVotesId).ToList());
        }
    }
}
