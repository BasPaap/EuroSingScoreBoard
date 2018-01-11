﻿using Bas.EuroSing.ScoreBoard.Messages;
using Bas.EuroSing.ScoreBoard.Model;
using Bas.EuroSing.ScoreBoard.Services;
using EnvDTE;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class VoteViewModel : ViewModelBase
    {
        private IDataService dataService;
        private List<int> validPointValues = new List<int>(new int[] { 12, 10, 8, 7, 6, 5, 4, 3, 2, 1 });

        public RelayCommand SettingsCommand { get; set; }

        public ObservableCollection<CountryListItemViewModel> Countries { get; set; }
        public ObservableCollection<int> VotesToCast { get; set; }

        private CountryListItemViewModel countryIssuingVotes;

        public CountryListItemViewModel CountryIssuingVotes
        {
            get { return countryIssuingVotes; }
            set
            {
                Set(ref countryIssuingVotes, value);

                UpdateCountriesToVoteOn();
            }
        }

        private void UpdateCountriesToVoteOn()
        {
            CountriesToVoteOn.Clear();

            if (CountryIssuingVotes != null)
            {
                var votes = this.dataService.GetVotes(CountryIssuingVotes.Id);

                foreach (var vote in votes.OrderBy(v => v.ToCountry.Name))
                {
                    CountriesToVoteOn.Add(new CountryVoteViewModel(vote, this.dataService, VotesToCast));                    
                }

                PopulateVotesToCast();
            }
        }

        private void PopulateVotesToCast()
        {
            var pointsCast = new List<int>();

            foreach (var country in CountriesToVoteOn)
            {
                if (int.TryParse(country.NumPoints, out int points) &&  // If country.Numpoints contains a number and
                    validPointValues.Contains(points))              // if the number is one of the valid point values (1-8, 10 and 12)
                {
                    pointsCast.Add(points);
                }
            }

            VotesToCast.Clear();
            foreach (var value in validPointValues)
            {
                if (!pointsCast.Contains(value))
                {
                    VotesToCast.Add(value);
                }
            }

            Messenger.Default.Send(new VotesToCastUpdatedMessage(VotesToCast));
        }

        public ObservableCollection<CountryVoteViewModel> CountriesToVoteOn { get; set; }

        public VoteViewModel(IDataService dataService)
        {
            this.dataService = dataService;

            CountriesToVoteOn = new ObservableCollection<CountryVoteViewModel>();
            Countries = new ObservableCollection<CountryListItemViewModel>();
            VotesToCast = new ObservableCollection<int>();

            SettingsCommand = new RelayCommand(() => MessengerInstance.Send(new GenericMessage<Message>(Message.ShowSettings)));

            UpdateCountries();

            Messenger.Default.Register<CountriesUpdatedMessage>(this, (m) =>
            {
                UpdateCountries();
                UpdateCountriesToVoteOn();
                VotesToCast.Clear();
                Messenger.Default.Send(new VotesToCastUpdatedMessage(VotesToCast));
            });

            Messenger.Default.Register<VoteCastMessage>(this, (m) =>
            {
                PopulateVotesToCast();

                CountryIssuingVotes.IsComplete = (VotesToCast.Count == 0);                
            });

            if (ViewModelBase.IsInDesignModeStatic)
            {
                CountryIssuingVotes = Countries.First();
                UpdateCountriesToVoteOn();
            }
        }
        
        private void UpdateCountries()
        {
            Countries.Clear();

            var allCountries = from c in dataService.GetAllCountries()
                               orderby c.Name
                               select new CountryListItemViewModel(c, this.dataService);

            foreach (var country in allCountries)
            {
                Countries.Add(country);
            }

            CountryIssuingVotes = null;
        }

    }
}
