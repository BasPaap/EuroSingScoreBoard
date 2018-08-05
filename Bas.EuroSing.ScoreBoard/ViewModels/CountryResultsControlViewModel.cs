using Bas.EuroSing.ScoreBoard.Extensions;
using Bas.EuroSing.ScoreBoard.Messages;
using Bas.EuroSing.ScoreBoard.Model;
using Bas.EuroSing.ScoreBoard.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class CountryResultsControlViewModel : ViewModelBase
    {
        private IDataService dataService;
        private Country country;

        private bool isReadyForLateVotes = false;

        public int Id { get; set; }

        private string name;

        public string Name
        {
            get { return name; }
            set { Set(ref name, value); }
        }

        private BitmapImage flagImage;

        public BitmapImage FlagImage
        {
            get { return flagImage; }
            set { Set(ref flagImage, value); }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                Set(ref isSelected, value);
                Messenger.Default.Send(new CountryResultClickedMessage());
            }
        }

        private bool isInQueue;

        public bool IsInQueue
        {
            get { return isInQueue; }
            set { Set(ref isInQueue, value); }
        }

        public RelayCommand ClickCommand { get; set; }

        public ObservableCollection<CountryListItemViewModel> CountriesToGiveEightPointsTo { get; set; }
        public ObservableCollection<CountryListItemViewModel> CountriesToGiveTenPointsTo { get; set; }
        public ObservableCollection<CountryListItemViewModel> CountriesToGiveTwelvePointsTo { get; set; }

        private CountryListItemViewModel eightPointsVote;
                
        public CountryListItemViewModel EightPointsVote
        {
            get { return eightPointsVote; }
            set
            {
                SetPointsVote(ref eightPointsVote, 8, value);                
            }
        }

        private void SetPointsVote(ref CountryListItemViewModel pointsVote, int numPoints, CountryListItemViewModel value)
        {
            var oldValue = pointsVote;
            Set(ref pointsVote, value);

            if (oldValue == null)
            {
                Messenger.Default.Send(new UpdateCountriesToGivePointsToMessage(), this.Id);
            }

            if (isReadyForLateVotes)
            {
                Messenger.Default.Send(new LateVoteCastMessage(Id, value.Id, numPoints));
            }
        }

        private CountryListItemViewModel tenPointsVote;
        
        public CountryListItemViewModel TenPointsVote
        {
            get { return tenPointsVote; }
            set
            {
                SetPointsVote(ref tenPointsVote, 10, value);
            }
        }

        private CountryListItemViewModel twelvePointsVote;        

        public CountryListItemViewModel TwelvePointsVote
        {
            get { return twelvePointsVote; }
            set
            {
                SetPointsVote(ref twelvePointsVote, 10, value);
            }
        }

        public CountryResultsControlViewModel(Country country, IDataService dataService)
        {
            this.dataService = dataService;
            this.country = country;

            Id = country.Id;
            Name = country.Name;
            IsInQueue = true;

            ClickCommand = new RelayCommand(() => IsSelected = true );
            FlagImage = country.FlagImage?.ToBitmapImage();
            
            var votes = dataService.GetIssuedVotes(country.Id);
            var eightPoints = votes.FirstOrDefault(v => v.NumPoints == 8);
            var tenPoints = votes.FirstOrDefault(v => v.NumPoints == 10);
            var twelvePoints = votes.FirstOrDefault(v => v.NumPoints == 12);

            OnUpdateCountriesToGivePointsTo(null);

            EightPointsVote = eightPoints != null ? CountriesToGiveEightPointsTo.First(c => c.Id == eightPoints.ToCountryId) : null;
            TenPointsVote = tenPoints != null ? CountriesToGiveTenPointsTo.First(c => c.Id == tenPoints.ToCountryId) : null;
            TwelvePointsVote = twelvePoints != null ? CountriesToGiveTwelvePointsTo.First(c => c.Id == twelvePoints.ToCountryId) : null;

            Messenger.Default.Register<ReadyForLateVotesMessage>(this, (message) => this.isReadyForLateVotes = true);
            Messenger.Default.Register<UpdateCountriesToGivePointsToMessage>(this, Id, OnUpdateCountriesToGivePointsTo);
            Messenger.Default.Register<VoteCastMessage>(this, (message) =>
            {
                if (message.CountryId == Id)
                {
                    OnUpdateCountriesToGivePointsTo(null);
                    EightPointsVote = eightPoints != null ? CountriesToGiveEightPointsTo.First(c => c.Id == eightPoints.ToCountryId) : null;
                    TenPointsVote = tenPoints != null ? CountriesToGiveTenPointsTo.First(c => c.Id == tenPoints.ToCountryId) : null;
                    TwelvePointsVote = twelvePoints != null ? CountriesToGiveTwelvePointsTo.First(c => c.Id == twelvePoints.ToCountryId) : null;
                }
            });
        }

        private void OnUpdateCountriesToGivePointsTo(UpdateCountriesToGivePointsToMessage message)
        {
            CountriesToGiveEightPointsTo = new ObservableCollection<CountryListItemViewModel>(GetCountriesToGivePointsTo(8));
            CountriesToGiveTenPointsTo = new ObservableCollection<CountryListItemViewModel>(GetCountriesToGivePointsTo(10));
            CountriesToGiveTwelvePointsTo = new ObservableCollection<CountryListItemViewModel>(GetCountriesToGivePointsTo(12));
        }

        private List<CountryListItemViewModel> GetCountriesToGivePointsTo(int numPoints)
        {
            Collection<Country> countries = dataService.GetCountriesToGiveVotesTo(Id, numPoints );

            var countryViewmodels = from c in countries
                                    where c.Id != this.Id
                                    select new CountryListItemViewModel(c, dataService);

            return new List<CountryListItemViewModel>(countryViewmodels);
        }
    }
}
