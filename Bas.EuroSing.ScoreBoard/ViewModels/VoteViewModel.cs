using Bas.EuroSing.ScoreBoard.Messages;
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
            PopulateVotesToCast();

            if (CountryIssuingVotes != null)
            {
                var votes = this.dataService.GetVotes(CountryIssuingVotes.Id);

                foreach (var vote in votes.OrderBy(v => v.ToCountry.Name))
                {
                    CountriesToVoteOn.Add(new CountryVoteViewModel(vote, this.dataService));
                    if (VotesToCast.Contains(vote.NumPoints))
                    {
                        VotesToCast.Remove(vote.NumPoints);
                    }
                }
            }
        }

        private void PopulateVotesToCast()
        {
            VotesToCast.Clear();
            foreach (var value in new[] { 12, 10, 8, 7, 6, 5, 4, 3, 2, 1 })
            {
                VotesToCast.Add(value);
            }
        }

        public ObservableCollection<CountryVoteViewModel> CountriesToVoteOn { get; set; }

        public VoteViewModel(IDataService dataService)
        {
            this.dataService = dataService;

            CountriesToVoteOn = new ObservableCollection<CountryVoteViewModel>();
            Countries = new ObservableCollection<CountryListItemViewModel>();
            VotesToCast = new ObservableCollection<int>();
            PopulateVotesToCast();

            SettingsCommand = new RelayCommand(() => MessengerInstance.Send(new GenericMessage<Message>(Message.ShowSettings)));

            UpdateCountries();

            Messenger.Default.Register<CountriesUpdatedMessage>(this, (m) =>
            {
                UpdateCountries();
                UpdateCountriesToVoteOn();
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
