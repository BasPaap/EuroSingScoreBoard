using Bas.EuroSing.ScoreBoard.Messages;
using Bas.EuroSing.ScoreBoard.Model;
using Bas.EuroSing.ScoreBoard.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class ResultsViewModel : ViewModelBase
    {
        private IDataService dataService;
        public ObservableCollection<CountryResultsViewModel> Countries { get; set; }

        private string currentCountryName;

        public string CurrentCountryName
        {
            get { return currentCountryName; }
            set { Set(ref currentCountryName, value); }
        }

        private int currentCountryNumber;

        public int CurrentCountryNumber
        {
            get { return currentCountryNumber; }
            set { Set(ref currentCountryNumber, value); }
        }

        private BitmapImage currentCountryFlagImage;

        public BitmapImage CurrentCountryFlagImage
        {
            get { return currentCountryFlagImage; }
            set { Set(ref currentCountryFlagImage, value); }
        }

        private string nextCountryName;

        public string NextCountryName
        {
            get { return nextCountryName; }
            set { Set(ref nextCountryName, value); }
        }
        
        private BitmapImage nextCountryFlagImage;

        public BitmapImage NextCountryFlagImage
        {
            get { return nextCountryFlagImage; }
            set { Set(ref nextCountryFlagImage, value); }
        }

        private BitmapImage winningCountryFlagImage;

        public BitmapImage WinningCountryFlagImage
        {
            get { return winningCountryFlagImage; }
            set { Set(ref winningCountryFlagImage, value); }
        }

        private string winningCountryName;

        public string WinningCountryName
        {
            get { return winningCountryName; }
            set { Set(ref winningCountryName, value); }
        }

        private string winningCountryPointsText;

        public string WinningCountryPointsText
        {
            get { return winningCountryPointsText; }
            set { Set(ref winningCountryPointsText, value); }
        }

        private Dictionary<int, IEnumerable<Vote>> votesByIssuingCountry;

        private int? currentCountryId;

        public RelayCommand EntranceAnimationCompletedCommand { get; set; }

        public ResultsViewModel(IDataService dataService)
        {
            this.dataService = dataService;

            Countries = new ObservableCollection<CountryResultsViewModel>(from c in dataService.GetAllCountries()
                                                                           orderby c.Name
                                                                           select new CountryResultsViewModel(c));

            EntranceAnimationCompletedCommand = new RelayCommand(OnEntranceAnimationCompleted);
            Messenger.Default.Register<ChangeStateMessage>(this, OnChangeStateMessage);
            Messenger.Default.Register<SetNextCountryMessage>(this, (message) =>
            {
                if (message.Country != null)
                {
                    NextCountryFlagImage = message.Country.FlagImage;
                    NextCountryName = message.Country.Name;
                }
            });

            Messenger.Default.Register<GenericMessage<Message>>(this, (message) =>
            {
                if (message.Content == Message.ShowResultsControlPanel)
                {
                    votesByIssuingCountry = this.dataService.GetAllVotes();
                }
            });

            Messenger.Default.Register<LateVoteCastMessage>(this, (message) =>
            {
                var fromCountryId = message.FromCountryId;

                    var votesFromCountry = this.votesByIssuingCountry[fromCountryId].ToList();

                var vote = votesFromCountry.FirstOrDefault(v => v.NumPoints == message.NumPoints);

                if (vote != null)
                {
                    votesFromCountry.Remove(vote);
                }

                votesFromCountry.Add(new Vote()
                {
                    FromCountryId = message.FromCountryId,
                    ToCountryId = message.ToCountryId,
                    NumPoints = message.NumPoints
                });

                votesByIssuingCountry[fromCountryId] = votesFromCountry.AsEnumerable();
                
            });

            if (IsInDesignMode)
            {
                var bitmapImage = new BitmapImage(new Uri(@"C:\Users\baspa\documents\visual studio 2017\Projects\Bas.EuroSing.ScoreBoard\Bas.EuroSing.ScoreBoard\Assets\Wyoming.png"));
                CurrentCountryFlagImage = bitmapImage;
                CurrentCountryNumber = 7;
                CurrentCountryName = "Wyoming";
                WinningCountryFlagImage = bitmapImage;
                WinningCountryName = "Wyoming";
                WinningCountryPointsText = "1138 points";
            }
        }

        private void OnEntranceAnimationCompleted()
        {
            for (int i = 1; i < 8; i++)
            {
                RevealPoints(i);
            }

            Messenger.Default.Send(new ReorderCountriesMessage());
        }

        private void RevealPoints(int numPoints)
        {
            var vote = GetVoteForNumPoints(numPoints);
            var country = GetReceivingCountryForVote(vote);

            if (country != null && vote != null)
            {
                country.CurrentPoints = vote.NumPoints;
                country.TotalPoints += vote.NumPoints;
            }
        }

        private CountryResultsViewModel GetReceivingCountryForVote(Vote vote)
        {
            return vote != null ? Countries.Single(c => c.Id == vote.ToCountryId) : null;
        }

        private Vote GetVoteForNumPoints(int numPoints)
        {
            int index;
            if (numPoints < 9)
            {
                index = numPoints - 1;
            }
            else if (numPoints == 10)
            {
                index = 8;
            }
            else
            {
                index = 9;
            }

            var votes = votesByIssuingCountry[currentCountryId.Value];
            var orderedVotes = votes.OrderBy(v => v.NumPoints);
            var voteArray = orderedVotes.ToArray();
            //return index >= 0 ? votesByIssuingCountry[currentCountryId.Value].OrderBy(v => v.NumPoints).ToArray()[index] : null;
            return index >= 0 ? voteArray[index] : null;
        }

        private void OnChangeStateMessage(ChangeStateMessage message)
        {
            if (message.State == ResultsState.RevealCountry)
            {

            }

            if (message.State == ResultsState.FirstGroupOfPoints)
            {
                CurrentCountryNumber++;
                currentCountryId = message.CurrentCountry.Id;
                CurrentCountryName = message.CurrentCountry.Name;

                if (message.CurrentCountry.FlagImage != null)
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CreateOptions = BitmapCreateOptions.None;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = new MemoryStream(message.CurrentCountry.FlagImage);
                    bitmapImage.EndInit();

                    CurrentCountryFlagImage = bitmapImage;
                }

                foreach (var country in Countries)
                {
                    country.CurrentPoints = 0;
                    country.RaisePropertyChanged("CurrentPoints");
                }                
            }

            if (message.State == ResultsState.EightPoints)
            {
                RevealPoints(8);
                Messenger.Default.Send(new ReorderCountriesMessage());
            }            

            if (message.State == ResultsState.TenPoints)
            {
                RevealPoints(10);
                Messenger.Default.Send(new ReorderCountriesMessage());
            }

            if (message.State == ResultsState.TwelvePoints)
            {
                RevealPoints(12);
                Messenger.Default.Send(new ReorderCountriesMessage());
            }

            if (message.State == ResultsState.RevealWinner)
            {
                var winner = Countries.OrderByDescending(c => c.TotalPoints).First();

                WinningCountryName = winner.Name;
                WinningCountryFlagImage = winner.FlagImage;
                WinningCountryPointsText = $"{winner.TotalPoints} points";
            }
        }
    }
}
